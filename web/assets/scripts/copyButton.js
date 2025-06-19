
const copyButtons = document.querySelectorAll('.copy-btn');

copyButtons.forEach(btn => {
    const tooltip = new bootstrap.Tooltip(btn);

    btn.addEventListener('click', () => {
        const text = btn.parentElement.querySelector('.copy-text').textContent.trim();

        navigator.clipboard.writeText(text).then(() => {
            btn.classList.remove('bi-clipboard');
            btn.classList.add('bi-check');
            btn.setAttribute('data-bs-original-title', 'Zkopírováno');
            tooltip.show();

            setTimeout(() => {
                tooltip.hide();
                btn.classList.remove('bi-check');
                btn.classList.add('bi-clipboard');
                btn.setAttribute('data-bs-original-title', 'Kopírovat');
            }, 1500);
        });
    });
});
