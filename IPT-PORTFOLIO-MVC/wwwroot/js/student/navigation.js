// Dito gumagana ang mobile navbar at active section links ng student pages.
(() => {
    const navbar = document.querySelector(".student-navbar");
    if (!navbar) return;

    const toggle = navbar.querySelector(".student-navbar-toggle");
    const panel = navbar.querySelector(".student-navbar-panel");
    const mobile = window.matchMedia("(max-width: 1050px)");
    const sections = [...navbar.querySelectorAll('.student-navbar-links a[href^="#"]')]
        .map(link => ({ link, section: document.getElementById(link.getAttribute("href").slice(1)) }))
        .filter(item => item.section);
    let activeLink;
    let updateQueued = false;

    document.body.classList.add("student-nav-ready");
    toggle.hidden = false;

    // Isara ang mobile menu at ibalik ang button sa closed state.
    function closeMenu() {
        toggle.setAttribute("aria-expanded", "false");
        navbar.classList.remove("is-menu-open");
    }

    // Markahan ang link ng section na kasalukuyang binabasa.
    function setActiveLink(link) {
        if (link === activeLink) return;
        activeLink = link;
        sections.forEach(item => {
            const selected = item.link === link;
            item.link.classList.toggle("is-active", selected);
            if (selected) item.link.setAttribute("aria-current", "location");
            else item.link.removeAttribute("aria-current");
        });
    }

    // Hanapin ang section na dumaan na sa ilalim ng fixed navbar.
    function updateSectionState() {
        updateQueued = false;
        // Sa ilalim ng fixed header ang basehan kung aling section ang active.
        const offset = navbar.getBoundingClientRect().bottom + Math.min(96, window.innerHeight * .14);
        let current;
        sections.forEach(item => {
            if (item.section.getBoundingClientRect().top <= offset) current = item;
        });
        if (window.scrollY + window.innerHeight >= document.documentElement.scrollHeight - 4) {
            current = sections[sections.length - 1];
        }
        setActiveLink(current?.link);
    }

    // Isang update lang kada animation frame para magaan ang scroll handling.
    function scheduleSectionState() {
        if (updateQueued) return;
        updateQueued = true;
        window.requestAnimationFrame(updateSectionState);
    }

    toggle.addEventListener("click", () => {
        const open = toggle.getAttribute("aria-expanded") !== "true";
        toggle.setAttribute("aria-expanded", String(open));
        navbar.classList.toggle("is-menu-open", open);
    });

    document.addEventListener("click", event => {
        if (!navbar.contains(event.target)) closeMenu();
        const link = event.target.closest('a[href^="#"]');
        if (!link || !panel?.contains(link)) return;
        const hash = link.getAttribute("href");
        const section = hash && hash.length > 1 ? document.getElementById(hash.slice(1)) : null;
        if (!section) return;
        if (sections.length) setActiveLink(link);
        if (mobile.matches) closeMenu();
        if (event.detail === 0 || mobile.matches) {
            section.tabIndex = -1;
            section.focus({ preventScroll: true });
        }
    });

    // Kapag Escape ang pinindot, isara ang menu at ibalik ang focus sa toggle.
    document.addEventListener("keydown", event => {
        if (event.key === "Escape" && toggle.getAttribute("aria-expanded") === "true") {
            closeMenu();
            toggle.focus();
        }
    });

    mobile.addEventListener("change", closeMenu);

    if (sections.length) {
        window.addEventListener("scroll", scheduleSectionState, { passive: true });
        window.addEventListener("resize", scheduleSectionState);
        window.addEventListener("hashchange", scheduleSectionState);
        window.addEventListener("pageshow", scheduleSectionState);
        updateSectionState();
    }
})();

