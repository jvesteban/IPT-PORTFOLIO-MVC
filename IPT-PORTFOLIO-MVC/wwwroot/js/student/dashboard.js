(() => {
    const toolbar = document.querySelector(".directory-toolbar");
    const search = document.querySelector("#student-search");
    if (!toolbar || !search) return;

    const cards = [...document.querySelectorAll(".student-card")];
    const results = document.querySelector("#search-results");
    const empty = document.querySelector("#search-empty");
    toolbar.hidden = false;

    function filterStudents() {
        const query = search.value.trim().toLocaleLowerCase();
        let visible = 0;
        for (const card of cards) {
            const name = card.querySelector("h3").textContent.toLocaleLowerCase();
            card.hidden = !name.includes(query);
            if (!card.hidden) visible++;
        }
        results.textContent = `${visible} of ${cards.length} portfolios`;
        empty.hidden = visible !== 0;
    }

    search.addEventListener("input", filterStudents);
    document.querySelector("#clear-search").addEventListener("click", () => {
        search.value = "";
        filterStudents();
        search.focus();
    });
})();

