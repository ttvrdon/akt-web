document.addEventListener("DOMContentLoaded", async () => {
    const flightHoursTotalElement = document.getElementById("flight-hours-total");
    const flightHoursGoElement = document.getElementById("flight-hours-go");
    const flightHoursServiceElement = document.getElementById("flight-hours-service");

    const loaderElement = document.getElementById("data-loader");
    const tableElement = document.getElementById("data-table");
    const dataErrorElement = document.getElementById("data-error");

    var data = await loadAircraftData();
    loaderElement.classList.add("d-none");

    if (data !== null) {
        flightHoursTotalElement.textContent = `${data.totalHours}:${data.totalMinutes}`;
        flightHoursGoElement.textContent = `${data.fromGoHours}:${data.fromGoMinutes}`;
        flightHoursServiceElement.textContent = `${data.nextServiceInHours}:${data.nextServiceInMinutes}`;

        tableElement.classList.remove("d-none");
    } else {
        dataErrorElement.classList.remove("d-none");
    }

});

async function loadAircraftData() {
    try {
        const response = await fetch("/api/GetAircraftData");

        if (!response.ok) {
            console.error("Chyba při načítání dat z API:", response.status, response.statusText);
            return null;
        }

        return await response.json();
    } catch (error) {
        console.error("Chyba při načítání dat:", error);
        return null;
    }

}