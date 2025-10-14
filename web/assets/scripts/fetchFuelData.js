document.addEventListener("DOMContentLoaded", async () => {
    const fuelRemainingElement = document.getElementById("fuel-remaining");

    const loaderElement = document.getElementById("fuel-data-loader");
    const tableElement = document.getElementById("fuel-data-table");
    const dataErrorElement = document.getElementById("fuel-data-error");

    var data = await loadFuelData();
    // var data = {
    //     fuelRemaining: 1123
    // }
    loaderElement.classList.add("d-none");

    if (data !== null) {
        fuelRemainingElement.textContent = formatNumber(data.fuelRemaining);

        if (data.fuelRemaining < 100) {
            fuelRemainingElement.classList.add("text-danger");
        } else {
            fuelRemainingElement.classList.remove("text-danger");
        }

        tableElement.classList.remove("d-none");
    } else {
        dataErrorElement.classList.remove("d-none");
    }

});

function formatNumber(number) {
    const formatted = new Intl.NumberFormat('cs-CZ').format(number);
    return `${formatted} l`;
}

async function loadFuelData() {
    try {
        const response = await fetch("/api/GetFuelData");

        if (!response.ok) {
            console.error("Error loading data from API:", response.status, response.statusText);
            return null;
        }

        return await response.json();
    } catch (error) {
        console.error("Error loading data:", error);
        return null;
    }
}