export default class OverviewPagination {
    container;
    previousButton;
    nextButton;
    pagesContainer;
    onChange;
    page = 1;
    totalPages = 1;

    constructor(container, onChange) {
        this.container = container;
        this.onChange = onChange;

        this.previousButton = container.querySelector("[data-overview-page-previous]");
        this.nextButton = container.querySelector("[data-overview-page-next]");
        this.pagesContainer = container.querySelector("[data-overview-pagination-pages]");

        this.bindEvents();
    }

    bindEvents() {
        this.previousButton.addEventListener("click", () => {
            if (this.page <= 1) return;
            this.onChange(this.page - 1);
        });
        this.nextButton.addEventListener("click", () => {
            if (this.page >= this.totalPages) return;
            this.onChange(this.page + 1);
        });

        // Event delegation for the numbered page buttons.
        this.pagesContainer.addEventListener("click", (event) => {
            const target = event.target.closest("[data-overview-page]");
            if (!target || target.disabled) return;

            const page = Number.parseInt(target.dataset.overviewPage, 10);
            if (!Number.isFinite(page) || page === this.page) return;

            this.onChange(page);
        });
    }

    update(pagination) {
        this.page = pagination.page;
        this.totalPages = Math.max(1, pagination.totalPages);

        this.previousButton.disabled = !pagination.hasPrevious;
        this.nextButton.disabled = !pagination.hasNext;

        this.renderPages();
    }

    renderPages() {
        const sequence = OverviewPagination.buildSequence(this.page, this.totalPages);
        this.pagesContainer.innerHTML = sequence.map((entry) => this.renderEntry(entry)).join("");
    }

    renderEntry(entry) {
        if (entry === "ellipsis") {
            return `<li><span class="c-overview__pagination__ellipsis" aria-hidden="true">...</span></li>`;
        }

        const isActive = entry === this.page;
        const attrs = [
            `type="button"`,
            `class="c-overview__pagination__page"`,
            `data-overview-page="${entry}"`,
        ];

        if (isActive) {
            attrs.push(`aria-current="page"`, `disabled`);
        }

        return `<li><button ${attrs.join(" ")}>${entry}</button></li>`;
    }

    /**
     * Build the visible pagination sequence for the given page/totalPages.
     * Always includes 1 and totalPages, plus the active page and its neighbours.
     * "ellipsis" entries are inserted whenever there's a gap > 1 between
     * consecutive shown numbers.
     *
     * Examples:
     *   buildSequence(1, 1)   -> [1]
     *   buildSequence(3, 5)   -> [1, 2, 3, 4, 5]
     *   buildSequence(1, 10)  -> [1, 2, "ellipsis", 10]
     *   buildSequence(5, 10)  -> [1, "ellipsis", 4, 5, 6, "ellipsis", 10]
     *   buildSequence(10, 10) -> [1, "ellipsis", 9, 10]
     */
    static buildSequence(page, totalPages) {
        const wanted = new Set([1, totalPages, page - 1, page, page + 1]);
        const valid = [...wanted]
            .filter((p) => p >= 1 && p <= totalPages)
            .sort((a, b) => a - b);

        const out = [];
        for (let i = 0; i < valid.length; i++) {
            if (i > 0 && valid[i] - valid[i - 1] > 1) {
                out.push("ellipsis");
            }
            out.push(valid[i]);
        }
        return out;
    }
}
