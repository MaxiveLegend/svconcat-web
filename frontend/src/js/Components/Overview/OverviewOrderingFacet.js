export default class OverviewOrderingFacet {
    select;
    onChange;

    constructor(select, onChange) {
        this.select = select;
        this.onChange = onChange;

        this.bindEvents();
    }

    bindEvents() {
        this.select.addEventListener("change", () => {
            this.onChange(this.select.value);
        });
    }
}
