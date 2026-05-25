/**
 * Renders an array of card payloads into the overview content container.
 * The HTML shape mirrors `Views/Partials/Components/Subcomponents/Card/Default.cshtml`
 * so the existing SCSS keeps working.
 */
export default class OverviewCardRenderer {
    container;

    constructor(container) {
        this.container = container;
    }

    renderLoading() {
        this.container.setAttribute("aria-busy", "true");
    }

    renderItems(items) {
        this.container.setAttribute("aria-busy", "false");

        if (!Array.isArray(items) || items.length === 0) {
            this.container.innerHTML =
                '<p class="c-overview__empty">Geen resultaten</p>'; // TODO: get from dictionary
            return;
        }

        this.container.innerHTML = items.map((item) => this.renderCard(item)).join("");
    }

    renderError() {
        this.container.setAttribute("aria-busy", "false");
        this.container.innerHTML =
            '<p class="c-overview__error">Er ging iets mis bij het laden.</p>'; // TODO: get from dictionary
    }

    renderCard(item) {
        const image = item.image ?? {};
        const cta = item.cta ?? {};
        const sources = Array.isArray(image.sources) ? image.sources : [];

        const sourceTags = sources
            .map(
                (source) =>
                    `<source srcset="${this.escape(source.src)}" media="${this.escape(source.mediaQuery)}">`,
            )
            .join("");

        return `
            <div class="c-card">
                <picture class="c-card__image">
                    ${sourceTags}
                    <img src="${this.escape(image.url ?? "")}" alt="${this.escape(image.alt ?? "")}">
                </picture>
                <div class="c-card__body">
                    <h3 class="c-card__title">${this.escape(item.title ?? "")}</h3>
                    <span class="c-card__subtitle">${this.escape(item.subtitle ?? "")}</span>
                    <p class="c-card__description">${this.escape(item.description ?? "")}</p>
                </div>
                <a  class="c-card__link" href="${this.escape(cta.url ?? "#")}" target="${this.escape(cta.target ?? "")}">${this.escape(cta.name ?? "")}</a>
            </div>
        `;
    }

    escape(value) {
        return String(value)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }
}
