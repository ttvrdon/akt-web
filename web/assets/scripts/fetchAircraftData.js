document.addEventListener("DOMContentLoaded", async () => {
    const flightHoursTotalElement = document.getElementById("flight-hours-total");
    const flightHoursReconstructionElement = document.getElementById("flight-hours-reconstruction");
    const flightHoursAnnualElement = document.getElementById("flight-hours-annual");
    const flightHoursServiceElement = document.getElementById("flight-hours-service");

    const loaderElement = document.getElementById("aircraft-data-loader");
    const tableElement = document.getElementById("aircraft-data-table");
    const dataErrorElement = document.getElementById("aircraft-data-error");

    var data = await loadAircraftData();
    // var data = {
    //     totalHours: "6542",
    //     totalMinutes: "30",
    //     fromReconstructionHours: "350",
    //     fromReconstructionMinutes: "45",
    //     fromAnnualHours: "150",
    //     fromAnnualMinutes: "15",
    //     nextServiceInHours: "-10",
    //     nextServiceInMinutes: "5"
    // }
    loaderElement.classList.add("d-none");

    if (data !== null) {
        flightHoursTotalElement.textContent = formatTime(data.totalHours, data.totalMinutes);
        flightHoursReconstructionElement.textContent = formatTime(data.fromReconstructionHours, data.fromReconstructionMinutes);
        flightHoursAnnualElement.textContent = formatTime(data.fromAnnualHours, data.fromAnnualMinutes);
        flightHoursServiceElement.textContent = formatTime(data.nextServiceInHours, data.nextServiceInMinutes);

        if (data.nextServiceInHours < 0) {
            flightHoursServiceElement.classList.add("text-danger");
        } else {
            flightHoursServiceElement.classList.remove("text-danger");
        }

        tableElement.classList.remove("d-none");
    } else {
        dataErrorElement.classList.remove("d-none");
    }

});

function formatTime(hours, minutes) {
    const absMinutes = Math.abs(minutes);
    return `${hours}:${String(absMinutes).padStart(2, '0')}`;
}

async function loadAircraftData() {
    try {
        const response = await fetch("/api/GetAircraftData");

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