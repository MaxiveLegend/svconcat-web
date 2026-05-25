/**
 * Thin fetch wrapper for the overview list endpoint.
 *
 * Expected response shape (to be confirmed when the backend is in place):
 * {
 *   items: Array<{
 *     title: string,
 *     subtitle?: string,
 *     description?: string,
 *     image?: { url: string, alt?: string, sources?: Array<{ src: string, mediaQuery: string }> },
 *     cta?:   { url: string, name: string, target?: string }
 *   }>,
 *   page: number,
 *   pageSize: number,
 *   totalPages: number,
 *   totalItems: number
 * }
 */
export default class OverviewApi {
    endpoint;
    abortController = null;

    constructor(endpoint) {
        this.endpoint = endpoint;
    }

    async fetchItems({ year, ordering, page, pageSize }) {
        // Cancel any in-flight request so the most recent user interaction wins.
        if (this.abortController) {
            this.abortController.abort();
        }
        this.abortController = new AbortController();

        const url = new URL(this.endpoint, window.location.origin);
        url.searchParams.set("year", String(year));
        url.searchParams.set("ordering", ordering);
        url.searchParams.set("page", String(page));
        url.searchParams.set("pageSize", String(pageSize));

        const response = await fetch(url.toString(), {
            method: "GET",
            headers: { Accept: "application/json" },
            signal: this.abortController.signal,
        });

        if (!response.ok) {
            throw new Error(`Overview API responded with ${response.status}`);
        }

        return await response.json();
    }
}
