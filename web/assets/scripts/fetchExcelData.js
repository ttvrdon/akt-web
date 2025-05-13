// fetchExcelData.js

async function fetchExcelData() {
    try {
        const response = await fetch('/api/ReadExcelData');

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        const data = await response.json();

        // Find the container element and display the data.
        const dataContainer = document.getElementById('data');
        dataContainer.innerHTML = '<pre>' + JSON.stringify(data, null, 2) + '</pre>';
    } catch (error) {
        console.error('Error invoking the API:', error);
    }
}

// Ensure the DOM is loaded before binding the event listener
document.addEventListener('DOMContentLoaded', () => {
    const invokeBtn = document.getElementById('invokeBtn');
    if (invokeBtn) {
        invokeBtn.addEventListener('click', fetchExcelData);
    }
});