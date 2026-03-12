const manualForm = document.getElementById("manual-form");
const clearButton = document.getElementById("clear-all");
const statusElement = document.getElementById("status");

const widthInput = document.getElementById("width");
const heightInput = document.getElementById("height");
const lengthInput = document.getElementById("length");
const massInput = document.getElementById("mass");

const counts = {
    total: document.getElementById("count-total"),
    standard: document.getElementById("count-standard"),
    special: document.getElementById("count-special"),
    rejected: document.getElementById("count-rejected")
};

const lists = {
    STANDARD: document.getElementById("standard-list"),
    SPECIAL: document.getElementById("special-list"),
    REJECTED: document.getElementById("rejected-list")
};

const packages = [];
let nextId = 1;

manualForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    const data = {
        width: Number(widthInput.value),
        height: Number(heightInput.value),
        length: Number(lengthInput.value),
        mass: Number(massInput.value)
    };

    await addPackage(data);
});

clearButton.addEventListener("click", () => {
    packages.length = 0;
    nextId = 1;
    render();
    setStatus("Package list cleared.");
});

async function addPackage(input) {
    if ([input.width, input.height, input.length, input.mass].some((value) => !Number.isFinite(value))) {
        setStatus("All values must be valid numbers.", true);
        return;
    }

    const response = await fetch("/api/sort", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            width: input.width,
            height: input.height,
            length: input.length,
            mass: input.mass
        })
    });

    const body = await response.json();
    if (!response.ok) {
        setStatus(body.message ?? "Unable to sort package.", true);
        return;
    }

    packages.unshift({
        id: nextId++,
        width: input.width,
        height: input.height,
        length: input.length,
        mass: input.mass,
        stack: body.stack
    });

    manualForm.reset();
    render();
    setStatus(`Package added to ${body.stack}.`);
}

function render() {
    const grouped = {
        STANDARD: [],
        SPECIAL: [],
        REJECTED: []
    };

    for (const item of packages) {
        grouped[item.stack].push(item);
    }

    counts.total.textContent = String(packages.length);
    counts.standard.textContent = String(grouped.STANDARD.length);
    counts.special.textContent = String(grouped.SPECIAL.length);
    counts.rejected.textContent = String(grouped.REJECTED.length);

    renderList(lists.STANDARD, grouped.STANDARD);
    renderList(lists.SPECIAL, grouped.SPECIAL);
    renderList(lists.REJECTED, grouped.REJECTED);
}

function renderList(container, items) {
    if (items.length === 0) {
        container.innerHTML = `<li class="empty-state">No packages in this stack.</li>`;
        return;
    }

    container.innerHTML = items
        .map(
            (item) =>
                `<li class="stack-item">
                    <div class="stack-item-id">Package #${item.id}</div>
                    <div class="stack-item-values">${item.width} x ${item.height} x ${item.length} cm | ${item.mass} kg</div>
                </li>`
        )
        .join("");
}

function setStatus(message, isError = false) {
    statusElement.textContent = message;
    statusElement.classList.toggle("error", isError);
}

render();
