export default class OverviewYearFacet {
    container;
    decrementButton;
    incrementButton;
    display;
    onChange;
    current;
    min;
    max;

    constructor(container, { min, max, current }, onChange) {
        this.container = container;
        this.onChange = onChange;
        this.min = min;
        this.max = max;
        this.current = current;

        this.decrementButton = container.querySelector("[data-overview-year-decrement]");
        this.incrementButton = container.querySelector("[data-overview-year-increment]");
        this.display = container.querySelector("[data-overview-year-display]");

        this.bindEvents();
        this.render();
    }

    bindEvents() {
        this.decrementButton.addEventListener("click", () => this.change(-1));
        this.incrementButton.addEventListener("click", () => this.change(1));
    }

    change(delta) {
        const next = this.current + delta;
        if (next < this.min || next > this.max) return;

        this.current = next;
        this.render();
        this.onChange(this.current);
    }

    render() {
        this.display.textContent = String(this.current);
        this.decrementButton.disabled = this.current <= this.min;
        this.incrementButton.disabled = this.current >= this.max;
    }
}
