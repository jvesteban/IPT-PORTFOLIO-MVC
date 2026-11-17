(() => {
    const form = document.querySelector("[data-signout]");
    const dialog = document.getElementById("signout-dialog");
    if (!form || !dialog) return;
    let confirmed = false;
    form.addEventListener("submit", event => {
        if (confirmed) return;
        event.preventDefault();
        event.stopImmediatePropagation();
        dialog.showModal();
    }, true);
    dialog.querySelector("[data-stay-signed-in]").addEventListener("click", () => dialog.close());
    dialog.querySelector("[data-confirm-signout]").addEventListener("click", () => {
        confirmed = true;
        dialog.close();
        form.requestSubmit();
    });
})();
