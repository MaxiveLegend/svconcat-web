export default class OverviewPagination {
    container;
    previousButton;
    nextButton;
    onChange;
    page = 1;
    totalPages = 1;

    constructor(container, onChange) {
        this.container = container;
        this.onChange = onChange;

        this.previousButton = container.querySelector("[data-overview-page-previous]");
        this.nextButton = container.querySelector("[data-overview-page-next]");

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
    }

    update(page, totalPages) {
        this.page = page;
        this.totalPages = totalPages;
        this.previousButton.disabled = page <= 1;
        this.nextButton.disabled = page >= totalPages;
        this.container.hidden = totalPages <= 1;
    }
}
