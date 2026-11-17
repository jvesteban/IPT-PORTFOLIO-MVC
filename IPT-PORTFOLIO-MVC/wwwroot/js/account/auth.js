"use strict";

document.querySelectorAll(".password-toggle").forEach((button) => {
    button.addEventListener("click", () => {
        const input = document.getElementById(button.getAttribute("aria-controls"));
        if (!input) return;

        const show = input.type === "password";
        input.type = show ? "text" : "password";
        button.textContent = show ? "Hide" : "Show";
        button.setAttribute("aria-pressed", String(show));
        const label = input.id === "ConfirmPassword" ? "confirm password" : "password";
        button.setAttribute("aria-label", `${show ? "Hide" : "Show"} ${label}`);
    });
});
