// Dito humihingi ng confirmation bago tuluyang mag-logout ang user.
(() => {
    const form = document.querySelector("[data-signout]");
    const dialog = document.getElementById("signout-dialog");
    if (!form || !dialog) return;
    let confirmed = false;
    // I-pause muna ang logout at ipakita ang confirmation dialog.
    form.addEventListener("submit", event => {
        if (confirmed) return;
        event.preventDefault();
        event.stopImmediatePropagation();
        dialog.showModal();
    }, true);
    dialog.querySelector("[data-stay-signed-in]").addEventListener("click", () => dialog.close());
    // Kapag confirmed, ituloy ang normal na logout form submission.
    dialog.querySelector("[data-confirm-signout]").addEventListener("click", () => {
        confirmed = true;
        dialog.close();
        form.requestSubmit();
    });
})();
