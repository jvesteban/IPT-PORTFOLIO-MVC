// Dito gumagana ang edit form validation, education autofill, uploads, at unsaved changes dialog.
(() => {
    const form = document.querySelector(".edit-profile form");
    if (!form) return;
    const feedback = document.querySelector("#education-feedback");
    const hadServerErrors = !!form.querySelector(".validation-summary-errors");
    let clientValidationAttempted = false;
    let refreshingRecords = false;

    // Hindi puwedeng mas maaga ang end year kaysa sa start year.
    $.validator.addMethod("educationEndYear", (value, input) => {
        const start = input.closest("[data-school-record]").querySelector('[data-field="StartYear"]').value;
        return !value || !start || Number(value) >= Number(start);
    }, "End year must be the same as or later than start year.");

    // HTTP o HTTPS lang ang valid na project link, at puwedeng blanko ang optional link.
    $.validator.addMethod("projectWebUrl", value => {
        if (!value.trim()) return true;
        try {
            const url = new URL(value.trim());
            return (url.protocol === "https:" || url.protocol === "http:") && !!url.hostname;
        } catch {
            return false;
        }
    }, "Use a full http:// or https:// project link, or leave it blank.");

    // I-refresh ang validation kapag nagdagdag o nag-alis ng project card.
    function updateValidation() {
        $(form).data("validator")?.destroy();
        $(form).removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse(form);
        const validator = $(form).data("validator");
        const showErrors = validator.settings.showErrors;
        validator.settings.showErrors = function (errorMap, errorList) {
            if (showErrors) showErrors.call(this, errorMap, errorList);
            else this.defaultShowErrors();
            normalizeDescriptions();
        };
        const requiredMessages = {
            SchoolName: "Enter the school name, or leave the whole record blank.",
            StartYear: "Enter the start year for this school record.",
            EndYear: "Enter the end year for this school record."
        };
        form.querySelectorAll("[data-school-record]").forEach(row => {
            const hasDetails = () => Array.from(row.querySelectorAll("input[data-field]"))
                .some(input => input.value.trim() !== "");
            ["SchoolName", "StartYear", "EndYear"].forEach(field => {
                $(row.querySelector(`[data-field="${field}"]`)).rules("add", {
                    required: hasDetails, messages: { required: requiredMessages[field] }
                });
            });
            $(row.querySelector('[data-field="EndYear"]')).rules("add", { educationEndYear: true });
        });
        form.querySelectorAll("[data-project-record]").forEach(row => {
            const hasDetails = () => [...row.querySelectorAll("[data-project-field]")]
                .some(input => input.value.trim() !== "") || (!!row.querySelector("[data-project-image]")?.dataset.savedSrc && row.querySelector('[data-project-meta="RemoveImage"]').value !== "true");
            $(row.querySelector('[data-project-field="Title"]')).rules("add", {
                required: hasDetails, messages: { required: "Enter a title for this project." }
            });
            $(row.querySelector('[data-project-field="ProjectUrl"]')).rules("add", { projectWebUrl: true });
        });
        $(form).off("invalid-form.editor").on("invalid-form.editor", () => {
            clientValidationAttempted = true;
            if (!refreshingRecords) focusErrorSummary();
        });
        if (clientValidationAttempted) {
            refreshingRecords = true;
            const valid = validator.form();
            refreshingRecords = false;
            if (valid && !hadServerErrors) {
                const summary = form.querySelector(".validation-summary");
                summary.classList.replace("validation-summary-errors", "validation-summary-valid");
                summary.querySelector("ul")?.replaceChildren();
            }
        }
        normalizeDescriptions();
    }

    // Panatilihing valid ang links ng fields sa help text at validation messages.
    function normalizeDescriptions() {
        // Puwedeng mawala ang generated error message kapag valid na ang field.
        // Alisin ang lumang references para tama pa rin ang screen reader help text.
        form.querySelectorAll("[aria-describedby]").forEach(input => {
            const ids = input.getAttribute("aria-describedby").split(/\s+/)
                .filter(id => document.getElementById(id));
            if (ids.length) input.setAttribute("aria-describedby", [...new Set(ids)].join(" "));
            else input.removeAttribute("aria-describedby");
        });
    }

    // Ilipat ang keyboard focus sa error summary para madaling makita ang kailangang ayusin.
    function focusErrorSummary() {
        window.requestAnimationFrame(() => {
            const summary = form.querySelector(".validation-summary-errors");
            if (summary) summary.focus();
        });
    }


    document.querySelectorAll("[data-education-level]").forEach(setupEducationAutofill);
    // I-set up ang autofill checkbox para sa isang education level.
    function setupEducationAutofill(level) {
        const controls = level.querySelector("[data-autofill-controls]");
        const checkbox = controls.querySelector("[data-autofill]");
        const status = controls.querySelector("[data-autofill-status]");
        const rows = [...level.querySelectorAll("[data-school-record]")];
        const fields = row => [...row.querySelectorAll("input[data-field]")];
        const values = row => Object.fromEntries(fields(row).map(input => [input.dataset.field, input.value.trim()]));
        const normalize = value => value.trim().replace(/\s+/g, " ").toLowerCase();
        const generated = new Map();

        // Enable lang ang autofill kung pareho ang schools, course o strand at valid ang unang card.
        function eligibility() {
            for (const field of ["SchoolName", "CollegeCourse", "StrandTrack"]) {
                const distinct = new Set(rows.map(row => normalize(values(row)[field] || "")).filter(Boolean));
                if (distinct.size > 1) return "Autofill is unavailable because the school, course or strand differs. Fill each year individually.";
            }
            const first = values(rows[0]);
            if (!first.SchoolName || !first.StartYear || !first.EndYear)
                return "Enter the school and both years in the first card to enable autofill.";
            if (fields(rows[0]).some(input => !input.validity.valid) ||
                +first.EndYear < +first.StartYear || +first.EndYear + rows.length - 1 > 2100)
                return "Check the first card. Use valid years from 1900 to 2100.";
            return "";
        }
        // I-update ang checkbox at paliwanag kung ready o blocked ang autofill.
        function refreshEligibility() {
            const reason = eligibility();
            checkbox.disabled = Boolean(reason);
            status.textContent = reason || "Ready to fill blank fields using the first card.";
        }
        controls.hidden = false;
        level.addEventListener("input", event => {
            if (!event.target.matches("input[data-field]")) return;
            checkbox.checked = false;
            generated.clear();
            refreshEligibility();
        });
        checkbox.addEventListener("change", () => {
            // Sa uncheck, ibalik ang dating values ng fields na pinunan ng autofill.
            if (!checkbox.checked) {
                for (const [input, original] of generated) input.value = original;
                generated.clear();
                updateValidation();
                refreshEligibility();
                status.textContent = "Autofill undone.";
                return;
            }
            if (eligibility()) {
                checkbox.checked = false;
                refreshEligibility();
                return;
            }
            generated.clear();
            const first = values(rows[0]);
            rows.forEach((row, index) => {
                if (index === 0) return;
                const current = values(row);
                const duration = +first.EndYear - +first.StartYear;
                const start = current.StartYear || (current.EndYear ? +current.EndYear - duration : +first.StartYear + index);
                const end = current.EndYear || +start + duration;
                if (+start < 1900 || +end > 2100 || +end < +start ||
                    fields(row).some(input => !input.validity.valid)) return;
                // Unang card ang basehan ng school at year range; blankong fields lang ang pupunan.
                const proposed = { ...first, StartYear: start, EndYear: end };
                fields(row).forEach(input => {
                    const value = proposed[input.dataset.field];
                    if (!input.value.trim() && value) {
                        generated.set(input, input.value);
                        input.value = value;
                    }
                });
            });
            checkbox.checked = generated.size > 0;
            updateValidation();
            status.textContent = generated.size
                ? "Blank fields filled. Review before saving. Uncheck to undo. Editing a field keeps the autofilled values."
                : "All fields are filled, or the remaining years need correction.";
        });
        refreshEligibility();
    }
    const projectImageUrls = new Map();
    const projectImageVersions = new WeakMap();
    const projectList = form.querySelector("[data-project-records]");
    const addProject = form.querySelector("[data-add-project]");
    const projectFeedback = form.querySelector("#project-feedback");
    const projectEmpty = form.querySelector("[data-project-empty]");


    // Bitawan ang temporary preview URL para hindi manatili sa browser memory.
    function releaseProjectImage(row) {
        const url = projectImageUrls.get(row);
        if (url) URL.revokeObjectURL(url);
        projectImageUrls.delete(row);
    }
    // I-set up ang preview, upload checks, remove, at undo ng isang project image.
    function initializeProjectImage(row) {
        const editor = row.querySelector("[data-project-image]");
        const input = row.querySelector('[data-project-field="Image"]');
        const flag = row.querySelector('[data-project-meta="RemoveImage"]');
        const image = editor.querySelector("[data-image-preview]");
        const placeholder = editor.querySelector("[data-image-placeholder]");
        const remove = editor.querySelector("[data-remove-image]");
        const restore = editor.querySelector("[data-restore-image]");
        const feedback = editor.querySelector("[data-image-feedback]");
        // Ipakita ang saved image o placeholder ayon sa kasalukuyang remove flag.
        function showSaved() {
            releaseProjectImage(row);
            const src = flag.value === "true" ? "" : editor.dataset.savedSrc;
            if (src) image.src = src; else image.removeAttribute("src");
            image.hidden = !src;
            placeholder.hidden = !!src;
            remove.hidden = !src;
            restore.hidden = !(editor.dataset.savedSrc && flag.value === "true");
        }
        showSaved();
        input.addEventListener("change", async () => {
            // Alamin ang pinakabagong selection para hindi mapalitan ng naunang image preview.
            const version = (projectImageVersions.get(row) || 0) + 1;
            projectImageVersions.set(row, version);
            const file = input.files[0];
            feedback.textContent = "";
            feedback.classList.remove("is-error");
            if (!file) { showSaved(); return; }
            if (!["image/jpeg", "image/png", "image/webp"].includes(file.type)
                || file.size === 0 || file.size > 3 * 1024 * 1024) {
                input.value = "";
                showSaved();
                feedback.textContent = "Choose a non-empty JPG, PNG, or WebP image up to 3 MB.";
                feedback.classList.add("is-error");
                return;
            }
            const url = URL.createObjectURL(file);
            const probe = new Image();
            probe.src = url;
            try {
                await probe.decode();
                if (!row.isConnected || projectImageVersions.get(row) !== version) { URL.revokeObjectURL(url); return; }
                releaseProjectImage(row);
                projectImageUrls.set(row, url);
                flag.value = "false";
                image.src = url;
                image.hidden = false;
                placeholder.hidden = true;
                remove.hidden = false;
                restore.hidden = true;
                feedback.textContent = "Image selected. Save changes to upload it.";
            } catch {
                URL.revokeObjectURL(url);
                if (projectImageVersions.get(row) !== version) return;
                input.value = "";
                showSaved();
                feedback.textContent = "This image could not be opened. Choose another JPG, PNG, or WebP.";
                feedback.classList.add("is-error");
            }
        });
        remove.addEventListener("click", () => {
            projectImageVersions.set(row, (projectImageVersions.get(row) || 0) + 1);
            input.value = "";
            flag.value = "true";
            showSaved();
            feedback.classList.remove("is-error");
            feedback.textContent = "Image removed from this draft. Save changes to apply.";
        });
        restore.addEventListener("click", () => {
            flag.value = "false";
            input.value = "";
            showSaved();
            feedback.classList.remove("is-error");
            feedback.textContent = "Saved image restored.";
        });
    }
    window.addEventListener("pagehide", () => {
        projectImageUrls.forEach(url => URL.revokeObjectURL(url));
        projectImageUrls.clear();
    });

    // Gamitin ang typed title bilang heading ng project card.
    function updateProjectHeading(row) {
        const title = row.querySelector('[data-project-field="Title"]').value.trim();
        row.querySelector("[data-project-heading]").textContent = title || "New project";
        const remove = row.querySelector("[data-remove-project]");
        remove.hidden = false;
        remove.setAttribute("aria-label", title ? "Remove project " + title : "Remove empty project");
    }

    // Ayusin ang field names at IDs para tama ang pag-bind ng project list sa server.
    function reindexProjects() {
        projectList.querySelectorAll("[data-project-record]").forEach((row, index) => {
            row.querySelectorAll("[data-project-field]").forEach(input => {
                const label = input.labels[0];
                const name = `Projects[${index}].${input.dataset.projectField}`;
                input.name = name;
                input.id = name.replace(/[^a-zA-Z0-9_-]/g, "_");
                if (label) label.htmlFor = input.id;
                const error = row.querySelector(`[data-project-error-for="${input.dataset.projectField}"]`);
                error.dataset.valmsgFor = name;
                error.id = input.id + "-validation";
                const helpIds = (input.getAttribute("aria-describedby") || "").split(/\s+/)
                    .filter(id => id && !id.endsWith("-validation") && !id.endsWith("-error"));
                input.setAttribute("aria-describedby", [...helpIds, error.id].join(" "));
                error.querySelectorAll('[id$="-error"]').forEach(message => message.id = input.id + "-error");
            });
            row.querySelectorAll("[data-project-meta]").forEach(input => {
                input.name = `Projects[${index}].${input.dataset.projectMeta}`;
                input.id = input.name.replace(/[^a-zA-Z0-9_-]/g, "_");
            });
            const heading = row.querySelector("[data-project-heading]");
            heading.id = row.querySelector('[data-project-field="Title"]').id + "-heading";
            row.setAttribute("aria-labelledby", heading.id);
            updateProjectHeading(row);
        });
        projectEmpty.hidden = projectList.children.length > 0;
    }

    addProject.hidden = false;
    projectList.querySelectorAll("[data-project-record]").forEach(row => { updateProjectHeading(row); initializeProjectImage(row); });
    // Kopyahin ang project template at i-setup ang bagong card bago mag-input.
    addProject.addEventListener("click", () => {
        const fragment = form.querySelector("[data-project-template]").content.cloneNode(true);
        const row = fragment.querySelector("[data-project-record]");
        row.classList.add("is-new");
        projectList.append(fragment);
        reindexProjects();
        initializeProjectImage(row);
        updateValidation();
        row.querySelector('[data-project-field="Title"]').focus();
        projectFeedback.textContent = "Project card added. Save changes to include it in your portfolio.";
    });
    projectList.addEventListener("input", event => {
        if (event.target.dataset.projectField === "Title") updateProjectHeading(event.target.closest("[data-project-record]"));
    });
    // Alisin ang napiling project card at ayusin ulit ang field indexes.
    projectList.addEventListener("click", event => {
        const remove = event.target.closest("[data-remove-project]");
        if (!remove) return;
        const row = remove.closest("[data-project-record]");
        const next = row.nextElementSibling || row.previousElementSibling;
        releaseProjectImage(row);
        row.remove();
        reindexProjects();
        updateValidation();
        (next?.querySelector('[data-project-field="Title"]') || addProject).focus();
        projectFeedback.textContent = "Project removed. Save changes to update your portfolio.";
    });
    updateValidation();
    focusErrorSummary();

    // Magpakita ng Saving state at pigilan ang paulit-ulit na save habang may request.
    const saveButton = form.querySelector("[data-save-button]");
    const saveLabel = saveButton.textContent;
    const saveFeedback = document.getElementById("save-feedback");
    form.addEventListener("submit", event => {
        if (event.defaultPrevented || !$(form).valid()) return;
        if (form.getAttribute("aria-busy") === "true") {
            event.preventDefault();
            return;
        }
        form.setAttribute("aria-busy", "true");
        saveButton.disabled = true;
        saveButton.textContent = "Saving…";
        saveFeedback.textContent = "Saving your portfolio changes.";
    });
    window.addEventListener("pageshow", () => {
        form.removeAttribute("aria-busy");
        saveButton.disabled = false;
        saveButton.textContent = saveLabel;
        saveFeedback.textContent = "";
    });

    const sectionLinks = [...document.querySelectorAll(".edit-section-nav a")];
    const sections = sectionLinks.map(link => ({
        link, heading: document.getElementById(link.hash.slice(1))
    })).filter(item => item.heading);
    let currentLink;
    let queued = false;
    // I-highlight ang kasalukuyang section sa edit form navigation.
    function setCurrentSection(link) {
        if (link === currentLink) return;
        currentLink = link;
        sections.forEach(item => {
            const current = item.link === link;
            item.link.classList.toggle("is-active", current);
            if (current) item.link.setAttribute("aria-current", "location");
            else item.link.removeAttribute("aria-current");
        });
    }
    // Hanapin ang form section na malapit sa ilalim ng page header.
    function updateCurrentSection() {
        queued = false;
        const header = document.querySelector(".workspace-header").getBoundingClientRect().height;
        let current = sections[0];
        sections.forEach(item => {
            if (item.heading.getBoundingClientRect().top <= header + 56) current = item;
        });
        if (window.scrollY + innerHeight >= document.documentElement.scrollHeight - 4) {
            current = sections[sections.length - 1];
        }
        setCurrentSection(current.link);
    }
    // Isang update lang kada frame para magaan ang scroll tracking.
    function scheduleSectionUpdate() {
        if (queued) return;
        queued = true;
        window.requestAnimationFrame(updateCurrentSection);
    }
    sectionLinks.forEach(link => link.addEventListener("click", () => {
        setCurrentSection(link);
        const heading = document.getElementById(link.hash.slice(1));
        heading.tabIndex = -1;
        heading.focus({ preventScroll: true });
    }));
    window.addEventListener("scroll", scheduleSectionUpdate, { passive: true });
    window.addEventListener("resize", scheduleSectionUpdate);
    updateCurrentSection();

    // I-preview ang bagong profile picture pagkatapos ma-check ang type at size.
    const photoInput = form.querySelector("#ProfilePicture");
    const preview = document.querySelector("#profile-photo-preview");
    const photoFeedback = document.querySelector("#photo-feedback");
    const savedAvatar = preview.firstElementChild.cloneNode(true);
    let previewUrl;
    let selection = 0;

    photoInput.addEventListener("change", async () => {
        const currentSelection = ++selection;
        const file = photoInput.files[0];
        photoFeedback.textContent = "";
        if (!file) {
            preview.replaceChildren(savedAvatar.cloneNode(true));
            if (previewUrl) URL.revokeObjectURL(previewUrl);
            return;
        }
        if (file.size > 3 * 1024 * 1024 || !["image/jpeg", "image/png", "image/webp"].includes(file.type)) {
            photoInput.value = "";
            photoFeedback.textContent = "Choose a JPG, PNG, or WebP image up to 3 MB.";
            return;
        }
        const url = URL.createObjectURL(file);
        const image = new Image();
        image.className = "profile-photo";
        image.alt = "";
        image.src = url;
        try {
            await image.decode();
            if (currentSelection !== selection) {
                URL.revokeObjectURL(url);
                return;
            }
            if (previewUrl) URL.revokeObjectURL(previewUrl);
            previewUrl = url;
            preview.replaceChildren(image);
            photoFeedback.textContent = "Photo selected. Save changes to upload it.";
        } catch {
            URL.revokeObjectURL(url);
            if (currentSelection !== selection) return;
            photoInput.value = "";
            photoFeedback.textContent = "This image could not be opened. Choose another photo.";
        }
    });
    window.addEventListener("pagehide", () => {
        if (previewUrl) URL.revokeObjectURL(previewUrl);
    });

    // Kumuha ng form values para maikumpara kung may unsaved edits.
    function editorSnapshot() {
        return JSON.stringify([...new FormData(form)].map(([name, value]) => [
            name, value instanceof File ? [value.name, value.size, value.lastModified && value.name ? value.lastModified : 0] : value
        ]));
    }
    const original = editorSnapshot();
    let leaving = false;
    const logout = document.querySelector("[data-signout]");
    document.addEventListener("submit", event => {
        if (event.target === logout) {
            document.getElementById("signout-detail").textContent = hadServerErrors || editorSnapshot() !== original
                ? "Signing out will discard your unsaved portfolio edits."
                : "You’ll return to the login page.";
        }
    }, true);
    logout.addEventListener("submit", event => { if (!event.defaultPrevented) leaving = true; });
    // Gamitin ang browser confirmation kapag may unsaved edits at nag-refresh o umalis.
    window.addEventListener("beforeunload", event => {
        if (leaving || form.getAttribute("aria-busy") === "true" ||
            (!hadServerErrors && editorSnapshot() === original)) return;
        event.preventDefault();
        event.returnValue = "";
    });
    window.addEventListener("pageshow", () => { leaving = false; });
    // Ipakita ang Keep editing at Discard changes kapag aalis sa edit page.
    const leaveDialog = document.querySelector("#leave-editor");
    let destination;
    document.addEventListener("click", event => {
        const link = event.target.closest("a[href]");
        if (!link || event.defaultPrevented || event.button !== 0 || event.ctrlKey || event.metaKey ||
            event.shiftKey || event.altKey || link.target === "_blank" || link.hasAttribute("download")) return;
        const target = new URL(link.href);
        if (target.hash && target.origin === location.origin && target.pathname === location.pathname &&
            target.search === location.search) return;
        if (!hadServerErrors && editorSnapshot() === original) return;
        event.preventDefault();
        destination = target.href;
        leaveDialog.showModal();
    });
    leaveDialog.querySelector("[data-keep-editing]").addEventListener("click", () => leaveDialog.close());
    leaveDialog.querySelector("[data-discard-changes]").addEventListener("click", () => {
        leaving = true;
        leaveDialog.close();
        location.assign(destination);
    });
})();

