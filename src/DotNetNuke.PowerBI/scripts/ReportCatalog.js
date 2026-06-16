(function () {
    "use strict";

    // Report Catalog: client-side search and category filtering.
    // The cards rendered by the server are already restricted to pages the current user is
    // allowed to view, so filtering here can never reveal an unauthorized report.

    function init() {
        var grid = document.getElementById("pbiCatalogGrid");
        if (!grid) { return; }

        var searchInput = document.getElementById("pbiCatalogSearch");
        var noResults = document.getElementById("pbiCatalogNoResults");
        var filterWrap = grid.parentNode.querySelector(".pbi-cat-filter");
        var cards = Array.prototype.slice.call(grid.querySelectorAll(".pbi-report-card"));

        var activeCategory = "all";

        function normalize(value) {
            return (value || "").toLowerCase().trim();
        }

        function applyFilter() {
            var search = normalize(searchInput ? searchInput.value : "");
            var visible = 0;

            cards.forEach(function (card) {
                var categories = card.getAttribute("data-categories") || "";
                var matchCat = (activeCategory === "all") ||
                    categories.indexOf("|" + activeCategory.toLowerCase() + "|") !== -1;

                var matchText = !search ||
                    (card.getAttribute("data-title") || "").indexOf(search) !== -1 ||
                    (card.getAttribute("data-desc") || "").indexOf(search) !== -1 ||
                    categories.indexOf(search) !== -1;

                var show = matchCat && matchText;
                card.style.display = show ? "" : "none";
                if (show) { visible++; }
            });

            if (noResults) {
                noResults.style.display = visible === 0 ? "" : "none";
            }
        }

        if (searchInput) {
            searchInput.addEventListener("input", applyFilter);
        }

        if (filterWrap) {
            filterWrap.addEventListener("click", function (e) {
                var btn = e.target.closest ? e.target.closest("button[data-cat]") : null;
                if (!btn) { return; }
                activeCategory = btn.getAttribute("data-cat");
                filterWrap.querySelectorAll("button").forEach(function (b) { b.classList.remove("active"); });
                btn.classList.add("active");
                applyFilter();
            });
        }

        applyFilter();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();
