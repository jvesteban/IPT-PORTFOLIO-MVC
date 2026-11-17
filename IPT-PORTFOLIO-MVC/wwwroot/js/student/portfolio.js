// Dito gumagana ang scroll reveals, active section links, at education accordion animations.
(() => {
    const portfolio = document.querySelector(".student-portfolio");
    if (!portfolio) return;

    const reducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)");
    const smallScreen = window.matchMedia("(max-width: 720px)");
    const targets = new Map();
    let observer;

    // Maikling pagitan lang ang reveal ng magkakatabing items.
    // I-group ang magkakatabing items para may maikling pagitan ang kanilang reveal.
    function collect(selector, step = 60, limit = 180) {
        const groups = new Map();
        portfolio.querySelectorAll(selector).forEach(item => {
            const index = groups.get(item.parentElement) ?? 0;
            groups.set(item.parentElement, index + 1);
            targets.set(item, { index, step, limit });
        });
    }

    collect(".section-heading .section-label, .section-heading h2, .projects-heading > .project-link");
    collect(".portfolio-card, .portfolio-empty-state");
    collect(".personal-information > div, .tag-list > li", 40, 120);
    collect(".academic-stage", 55, 160);
    collect(".introduction-copy", 35, 100);
    collect(".contact-heading > *, .contact-list > div");

    portfolio.querySelectorAll(".project-media").forEach(media => {
        if (!media.querySelector(".project-cover")) return;
        media.classList.add("reveal-image");
        const cardTiming = targets.get(media.closest(".project-card"));
        targets.set(media, { index: (cardTiming?.index ?? 0) + 1, step: 60, limit: 180 });
    });

    // Ipakita ang item at ihinto ang pag-observe para isang beses lang ang reveal.
    function reveal(item, immediate = false) {
        if (immediate) item.classList.add("reveal-immediate");
        item.classList.add("is-visible");
        observer?.unobserve(item);
    }

    // I-set ang observer at i-adjust ang animations sa screen size at motion preference.
    function configureReveals() {
        observer?.disconnect();
        const enabled = !reducedMotion.matches && "IntersectionObserver" in window;
        document.body.classList.toggle("portfolio-motion", enabled);

        if (!enabled) {
            targets.forEach((_, item) => reveal(item));
            return;
        }

        observer = new IntersectionObserver(entries => {
            entries.forEach(entry => {
                if (entry.isIntersecting) reveal(entry.target);
            });
        }, { threshold: 0, rootMargin: "-72px 0px -24px 0px" });

        targets.forEach(({ index, step, limit }, item) => {
            item.classList.add("scroll-reveal");
            const delay = Math.min(index * (smallScreen.matches ? Math.min(step, 40) : step),
                smallScreen.matches ? Math.min(limit, 120) : limit);
            item.style.setProperty("--reveal-delay", `${delay}ms`);
            if (item.getBoundingClientRect().bottom < 72) {
                reveal(item, true);
                return;
            }
            // Panatilihing visible ang tapos nang animation kahit mag-resize ang screen.
            if (!item.classList.contains("is-visible")) observer.observe(item);
        });
    }

    // I-highlight ang section link na tugma sa kasalukuyang scroll position.
    const sectionLinks = [...document.querySelectorAll('.student-navbar-links a[href^="#"]')];
    const trackedSections = sectionLinks
        .map(link => ({ link, target: document.getElementById(link.getAttribute("href").slice(1)) }))
        .filter(entry => entry.target);
    let activeSectionLink = null;
    let spyQueued = false;

    // I-update ang active look at screen reader state ng section link.
    function setActiveSectionLink(link) {
        if (link === activeSectionLink) return;
        activeSectionLink = link;
        sectionLinks.forEach(item => {
            const isActive = item === link;
            item.classList.toggle("is-active", isActive);
            if (isActive) item.setAttribute("aria-current", "location");
            else item.removeAttribute("aria-current");
        });
    }

    // Hanapin ang section na nasa reading area habang nag-scroll.
    function updateActiveSectionLink() {
        spyQueued = false;
        if (!trackedSections.length) return;
        const header = document.querySelector(".workspace-header");
        const offset = (header?.getBoundingClientRect().height ?? 76) + 56;
        if (window.scrollY < 140) {
            setActiveSectionLink(null);
            return;
        }
        let current = trackedSections[0].link;
        for (const entry of trackedSections) {
            if (entry.target.getBoundingClientRect().top <= offset) current = entry.link;
        }
        if (window.scrollY + window.innerHeight >= document.documentElement.scrollHeight - 4) {
            current = trackedSections[trackedSections.length - 1].link;
        }
        setActiveSectionLink(current);
    }

    // Pagsamahin ang scroll updates sa isang animation frame.
    function scheduleSectionSpy() {
        if (spyQueued) return;
        spyQueued = true;
        window.requestAnimationFrame(updateActiveSectionLink);
    }

    if (trackedSections.length) {
        window.addEventListener("scroll", scheduleSectionSpy, { passive: true });
        window.addEventListener("resize", scheduleSectionSpy);
        updateActiveSectionLink();
    }

    // Pareho ang style ng animation sa pagbukas at pagsara ng education panels.
    const supportsWaapi = typeof Element.prototype.animate === "function";

    portfolio.querySelectorAll(".academic-stage").forEach(stage => {
        const summary = stage.querySelector("summary");
        const content = stage.querySelector("dl");
        if (!summary || !content || !supportsWaapi) return;

        let heightAnim = null;
        let contentAnim = null;
        let isClosing = false;
        let isExpanding = false;

        // Linisin ang temporary styles pagkatapos ng accordion animation.
        function resetAnimationState() {
            stage.style.height = "";
            stage.style.overflow = "";
            stage.classList.remove("is-closing");
            heightAnim = null;
            contentAnim = null;
            isClosing = false;
            isExpanding = false;
        }

        // I-stop ang current animation bago magsimula ng panibagong open o close.
        function cancelRunning() {
            if (heightAnim) {
                heightAnim.onfinish = null;
                heightAnim.oncancel = null;
                heightAnim.cancel();
                heightAnim = null;
            }
            if (contentAnim) {
                contentAnim.cancel();
                contentAnim = null;
            }
        }

        // I-animate ang taas at content habang binubuksan ang education panel.
        function expand() {
            const wasClosing = isClosing;
            const startHeight = stage.getBoundingClientRect().height;
            const startOpacity = wasClosing ? (parseFloat(window.getComputedStyle(content).opacity) || 0) : 0;
            cancelRunning();
            isClosing = false;
            isExpanding = true;
            stage.classList.remove("is-closing");
            stage.style.overflow = "hidden";
            stage.style.height = "";
            stage.open = true;
            const endHeight = stage.getBoundingClientRect().height;
            stage.style.height = `${startHeight}px`;
            stage.getBoundingClientRect();

            heightAnim = stage.animate(
                { height: [`${startHeight}px`, `${endHeight}px`] },
                { duration: 280, easing: "cubic-bezier(.22, 1, .36, 1)" }
            );
            contentAnim = content.animate(
                [
                    { opacity: startOpacity, transform: wasClosing ? "translateY(-3px)" : "translateY(-6px)" },
                    { opacity: 1, transform: "translateY(0)" }
                ],
                { duration: 240, delay: wasClosing ? 0 : 40, fill: "backwards", easing: "cubic-bezier(.22, 1, .36, 1)" }
            );

            heightAnim.onfinish = () => {
                resetAnimationState();
            };
        }

        // I-animate ang pagsara at isara ang panel pagkatapos ng transition.
        function shrink() {
            const startHeight = stage.getBoundingClientRect().height;
            const startOpacity = parseFloat(window.getComputedStyle(content).opacity) || 1;
            cancelRunning();
            isExpanding = false;
            isClosing = true;
            stage.classList.add("is-closing");
            stage.style.overflow = "hidden";

            const styles = window.getComputedStyle(stage);
            const borders = (parseFloat(styles.borderTopWidth) || 0) + (parseFloat(styles.borderBottomWidth) || 0);
            const endHeight = summary.getBoundingClientRect().height + borders;

            stage.style.height = `${startHeight}px`;
            stage.getBoundingClientRect();

            heightAnim = stage.animate(
                { height: [`${startHeight}px`, `${endHeight}px`] },
                { duration: 280, easing: "cubic-bezier(.22, 1, .36, 1)" }
            );
            contentAnim = content.animate(
                [
                    { opacity: startOpacity, transform: "translateY(0)" },
                    { opacity: 0, transform: "translateY(-4px)" }
                ],
                { duration: 190, fill: "forwards", easing: "ease" }
            );

            heightAnim.onfinish = () => {
                stage.open = false;
                if (contentAnim) contentAnim.cancel();
                resetAnimationState();
            };
        }

        summary.addEventListener("click", event => {
            if (reducedMotion.matches) {
                cancelRunning();
                resetAnimationState();
                return;
            }
            event.preventDefault();
            if (isClosing || !stage.open) expand();
            else if (isExpanding || stage.open) shrink();
        });

        stage.addEventListener("toggle", () => {
            if (!isExpanding && !isClosing) {
                cancelRunning();
                resetAnimationState();
            }
        });
    });

    // Normal pa rin ang section links; observer ang nagpapakita ng content pagdating sa screen.
    // Ipakita agad ang focused content para magamit ito ng keyboard users.
    document.addEventListener("focusin", event => {
        if (!(event.target instanceof Element) || !portfolio.contains(event.target)) return;
        const block = event.target.closest(".scroll-reveal");
        let current = block;
        while (current) {
            reveal(current, true);
            current = current.parentElement?.closest(".scroll-reveal");
        }
    });

    reducedMotion.addEventListener("change", configureReveals);
    smallScreen.addEventListener("change", configureReveals);
    configureReveals();
})();

