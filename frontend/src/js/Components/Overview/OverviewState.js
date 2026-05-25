export default class OverviewState {
    state;
    listeners = [];

    constructor(initial) {
        this.state = { ...initial };
    }

    get() {
        return { ...this.state };
    }

    set(patch) {
        const next = { ...this.state, ...patch };
        if (this.isShallowEqual(this.state, next)) return;

        this.state = next;
        this.notify();
    }

    onChange(listener) {
        this.listeners.push(listener);
    }

    notify() {
        const snapshot = this.get();
        for (const listener of this.listeners) {
            listener(snapshot);
        }
    }

    isShallowEqual(a, b) {
        const keys = new Set([...Object.keys(a), ...Object.keys(b)]);
        for (const key of keys) {
            if (a[key] !== b[key]) return false;
        }
        return true;
    }
}
