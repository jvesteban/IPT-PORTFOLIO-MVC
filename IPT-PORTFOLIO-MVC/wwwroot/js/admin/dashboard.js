// Escape dismisses a chart tooltip until the next hover or keyboard focus.
document.querySelectorAll("[data-chart-tooltip]").forEach(chart => {
    chart.addEventListener("keydown", event => {
        if (event.key === "Escape") chart.classList.add("tooltip-dismissed");
    });
    chart.addEventListener("pointerleave", () => chart.classList.remove("tooltip-dismissed"));
    chart.addEventListener("focusout", event => {
        if (!chart.contains(event.relatedTarget)) chart.classList.remove("tooltip-dismissed");
    });
});
