(() => {
    const sidebar = document.querySelector(".admin-sidebar");
    const toggle = sidebar?.querySelector(".admin-menu-toggle");
    const mobile = window.matchMedia("(max-width: 820px)");

    function closeMenu() {
        sidebar.classList.remove("is-menu-open");
        toggle.setAttribute("aria-expanded", "false");
    }

    if (toggle) {
        document.body.classList.add("admin-menu-ready");
        toggle.hidden = false;
        toggle.addEventListener("click", () => {
            const open = toggle.getAttribute("aria-expanded") !== "true";
            toggle.setAttribute("aria-expanded", String(open));
            sidebar.classList.toggle("is-menu-open", open);
        });
        document.addEventListener("click", event => {
            if (!sidebar.contains(event.target)) closeMenu();
        });
        document.addEventListener("keydown", event => {
            if (event.key === "Escape" && toggle.getAttribute("aria-expanded") === "true") {
                closeMenu();
                toggle.focus();
            }
        });
        mobile.addEventListener("change", closeMenu);
    }

    document.querySelectorAll("[data-table-scroll]").forEach(region => {
        const hint = document.getElementById(region.getAttribute("aria-describedby"));
        if (!hint) return;
        function updateOverflow() {
            const overflows = region.scrollWidth > region.clientWidth + 1;
            hint.hidden = !overflows;
            region.tabIndex = overflows ? 0 : -1;
        }
        if ("ResizeObserver" in window) new ResizeObserver(updateOverflow).observe(region);
        else window.addEventListener("resize", updateOverflow);
        updateOverflow();
    });

    const forms = [...document.querySelectorAll("form[data-admin-submit]")];
    const feedback = document.getElementById("admin-submit-feedback");
    forms.forEach(form => {
        const button = form.querySelector('button[type="submit"]');
        if (!button) return;
        const label = button.textContent;
        form.addEventListener("submit", event => {
            if (event.defaultPrevented) return;
            if (form.getAttribute("aria-busy") === "true") {
                event.preventDefault();
                return;
            }
            form.setAttribute("aria-busy", "true");
            button.disabled = true;
            button.textContent = button.dataset.loadingText || "Saving…";
            if (feedback) feedback.textContent = button.textContent;
        });
        // Restore controls when a completed navigation is restored from browser history.
        window.addEventListener("pageshow", () => {
            form.removeAttribute("aria-busy");
            button.disabled = false;
            button.textContent = label;
            if (feedback) feedback.textContent = "";
        });
    });
})();
