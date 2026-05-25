import OverviewState from "./Overview/OverviewState.js";
import OverviewApi from "./Overview/OverviewApi.js";
import OverviewYearFacet from "./Overview/OverviewYearFacet.js";
import OverviewOrderingFacet from "./Overview/OverviewOrderingFacet.js";
import OverviewPagination from "./Overview/OverviewPagination.js";
import OverviewCardRenderer from "./Overview/OverviewCardRenderer.js";

export default class Overview {
    container;
    config;
    state;
    api;
    renderer;
    yearFacet;
    orderingFacet;
    pagination;

    constructor(element) {
        this.container = element;
        this.config = this.readConfig();

        if (!this.config.pageKey) {
            console.warn(
                "Overview: missing data-overview-page-key; the API has no way to scope the result to a page.",
                this.container,
            );
            return;
        }

        this.state = new OverviewState({
            pageKey: this.config.pageKey,
            year: this.config.currentYear,
            ordering: this.config.ordering,
            page: 1,
            pageSize: this.config.pageSize,
        });

        this.api = new OverviewApi(this.config.endpoint);

        this.renderer = new OverviewCardRenderer(
            this.container.querySelector("[data-overview-content]"),
        );

        this.yearFacet = new OverviewYearFacet(
            this.container.querySelector(".c-overview__facets__year"),
            {
                min: this.config.minYear,
                max: this.config.maxYear,
                current: this.config.currentYear,
            },
            (year) => this.state.set({ year, page: 1 }),
        );

        this.orderingFacet = new OverviewOrderingFacet(
            this.container.querySelector("[data-overview-ordering]"),
            (ordering) => this.state.set({ ordering, page: 1 }),
        );

        this.pagination = new OverviewPagination(
            this.container.querySelector("[data-overview-pagination]"),
            (page) => this.state.set({ page }),
        );

        this.state.onChange((snapshot) => this.fetchAndRender(snapshot));

        this.fetchAndRender(this.state.get());
    }

    readConfig() {
        const ds = this.container.dataset;
        const currentYear = Number.parseInt(ds.overviewCurrentYear ?? "", 10);
        const minYear = Number.parseInt(ds.overviewMinYear ?? "", 10);
        const maxYear = Number.parseInt(ds.overviewMaxYear ?? "", 10);
        const pageSize = Number.parseInt(ds.overviewPageSize ?? "", 10);
        const fallbackYear = new Date().getFullYear();

        return {
            endpoint: ds.overviewEndpoint || "/umbraco/api/overview/items",
            pageKey: ds.overviewPageKey ?? "",
            currentYear: Number.isFinite(currentYear) ? currentYear : fallbackYear,
            minYear: Number.isFinite(minYear) ? minYear : 2000,
            maxYear: Number.isFinite(maxYear) ? maxYear : fallbackYear,
            ordering: ds.overviewOrdering || "newest",
            pageSize: Number.isFinite(pageSize) && pageSize > 0 ? pageSize : 12,
        };
    }

    async fetchAndRender(snapshot) {
        this.renderer.renderLoading();

        try {
            const { items, pagination } = await this.api.fetchItems(snapshot);

            this.renderer.renderItems(items ?? []);
            this.pagination.update(pagination ?? this.emptyPagination(snapshot));
        } catch (error) {
            // A newer request superseded this one; the newer call owns the render.
            if (error.name === "AbortError") return;

            console.error("Overview: failed to fetch items", error);
            this.renderer.renderError();
            this.pagination.update(this.emptyPagination(snapshot));
        }
    }

    emptyPagination(snapshot) {
        return {
            page: snapshot.page,
            pageSize: snapshot.pageSize,
            totalPages: 1,
            totalItems: 0,
            hasPrevious: false,
            hasNext: false,
        };
    }
}
