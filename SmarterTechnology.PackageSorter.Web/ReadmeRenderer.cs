using System.Net;
using System.Text;
using System.Text.RegularExpressions;

internal static partial class ReadmeRenderer
{
    [GeneratedRegex(@"\[([^\]]+)\]\(([^)]+)\)")]
    private static partial Regex MarkdownLinkRegex();

    [GeneratedRegex("`([^`]+)`")]
    private static partial Regex InlineCodeRegex();

    [GeneratedRegex(@"\*\*([^*]+)\*\*")]
    private static partial Regex BoldRegex();

    public static string ToHtml(string markdown)
    {
        var lines = markdown.Replace("\r\n", "\n").Split('\n');
        var sb = new StringBuilder();
        var inList = false;
        var inParagraph = false;
        var inCodeBlock = false;

        void CloseParagraph()
        {
            if (inParagraph)
            {
                sb.AppendLine("</p>");
                inParagraph = false;
            }
        }

        void CloseList()
        {
            if (inList)
            {
                sb.AppendLine("</ul>");
                inList = false;
            }
        }

        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd();

            if (line.StartsWith("```", StringComparison.Ordinal))
            {
                CloseParagraph();
                CloseList();

                if (!inCodeBlock)
                {
                    sb.AppendLine("<pre><code>");
                    inCodeBlock = true;
                }
                else
                {
                    sb.AppendLine("</code></pre>");
                    inCodeBlock = false;
                }

                continue;
            }

            if (inCodeBlock)
            {
                sb.AppendLine(WebUtility.HtmlEncode(rawLine));
                continue;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                CloseParagraph();
                CloseList();
                continue;
            }

            if (line.StartsWith("- ", StringComparison.Ordinal))
            {
                CloseParagraph();
                if (!inList)
                {
                    sb.AppendLine("<ul>");
                    inList = true;
                }

                sb.Append("<li>")
                    .Append(FormatInline(line[2..].Trim()))
                    .AppendLine("</li>");
                continue;
            }

            CloseList();

            if (line.StartsWith("### ", StringComparison.Ordinal))
            {
                CloseParagraph();
                sb.Append("<h3>")
                    .Append(FormatInline(line[4..].Trim()))
                    .AppendLine("</h3>");
                continue;
            }

            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                CloseParagraph();
                sb.Append("<h2>")
                    .Append(FormatInline(line[3..].Trim()))
                    .AppendLine("</h2>");
                continue;
            }

            if (line.StartsWith("# ", StringComparison.Ordinal))
            {
                CloseParagraph();
                sb.Append("<h1>")
                    .Append(FormatInline(line[2..].Trim()))
                    .AppendLine("</h1>");
                continue;
            }

            if (line == "---")
            {
                CloseParagraph();
                sb.AppendLine("<hr />");
                continue;
            }

            if (!inParagraph)
            {
                sb.Append("<p>");
                inParagraph = true;
            }
            else
            {
                sb.Append(' ');
            }

            sb.Append(FormatInline(line.Trim()));
        }

        CloseParagraph();
        CloseList();

        if (inCodeBlock)
        {
            sb.AppendLine("</code></pre>");
        }

        return sb.ToString();
    }

    private static string FormatInline(string text)
    {
        var links = new List<string>();
        var withTokens = MarkdownLinkRegex().Replace(text, match =>
        {
            var label = WebUtility.HtmlEncode(match.Groups[1].Value.Trim());
            var target = ResolveLinkTarget(match.Groups[2].Value.Trim());
            var href = WebUtility.HtmlEncode(target);
            var token = $"__LINK_{links.Count}__";

            links.Add($"<a href=\"{href}\" target=\"_blank\" rel=\"noopener noreferrer\">{label}</a>");
            return token;
        });

        var encoded = WebUtility.HtmlEncode(withTokens);
        encoded = BoldRegex().Replace(encoded, "<strong>$1</strong>");
        encoded = InlineCodeRegex().Replace(encoded, "<code>$1</code>");

        for (var i = 0; i < links.Count; i++)
        {
            encoded = encoded.Replace($"__LINK_{i}__", links[i], StringComparison.Ordinal);
        }

        return encoded;
    }

    private static string ResolveLinkTarget(string rawTarget)
    {
        if (rawTarget.Equals("README.md", StringComparison.OrdinalIgnoreCase))
        {
            return "/readme";
        }

        if (rawTarget.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            rawTarget.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return rawTarget;
        }

        return rawTarget.StartsWith('/') ? rawTarget : $"/{rawTarget.TrimStart('.', '/')}";
    }
}
