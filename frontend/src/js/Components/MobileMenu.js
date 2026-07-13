const DESKTOP_QUERY = '(min-width: 768px)'; // matches the SCSS `tablet` breakpoint

export default class MobileMenu {
    container;
    toggleButton = null;
    drawer = null;
    overlay = null;
    closeButton = null;
    desktopQuery = null;
    isOpen = false;

    constructor(element) {
        this.container = element;

        this.toggleButton = this.container.querySelector('.c-mobile-menu__toggle');
        this.drawer = this.container.querySelector('.c-mobile-menu__drawer');
        this.overlay = this.container.querySelector('[data-mobile-menu-overlay]');
        this.closeButton = this.container.querySelector('.c-mobile-menu__close');
        this.desktopQuery = window.matchMedia(DESKTOP_QUERY);

        // Bound once so add/removeEventListener reference the same handler.
        this.onToggleClick = () => this.toggle();
        this.onCloseClick = () => this.close();
        this.onOverlayClick = () => this.close();
        this.onKeydown = (event) => this.handleKeydown(event);
        this.onDrawerClick = (event) => this.handleDrawerClick(event);
        this.onViewportChange = () => this.handleViewportChange();

        this.bindEvents();
    }

    bindEvents() {
        this.toggleButton?.addEventListener('click', this.onToggleClick);
        this.closeButton?.addEventListener('click', this.onCloseClick);
        this.overlay?.addEventListener('click', this.onOverlayClick);
        this.drawer?.addEventListener('click', this.onDrawerClick);
        this.desktopQuery.addEventListener('change', this.onViewportChange);
    }

    toggle() {
        this.isOpen ? this.close() : this.open();
    }

    open() {
        if (this.isOpen) return;
        this.isOpen = true;

        this.drawer?.classList.add('is-open');
        this.overlay?.classList.add('is-open');
        this.toggleButton?.setAttribute('aria-expanded', 'true');
        document.body.classList.add('u-no-scroll');
        document.addEventListener('keydown', this.onKeydown);

        this.closeButton?.focus();
    }

    close() {
        if (!this.isOpen) return;
        this.isOpen = false;

        this.drawer?.classList.remove('is-open');
        this.overlay?.classList.remove('is-open');
        this.toggleButton?.setAttribute('aria-expanded', 'false');
        document.body.classList.remove('u-no-scroll');
        document.removeEventListener('keydown', this.onKeydown);

        this.toggleButton?.focus();
    }

    handleKeydown(event) {
        if (event.key === 'Escape') this.close();
    }

    handleDrawerClick(event) {
        // Close after navigating so the drawer isn't left open on the next page
        // (also covers in-page anchor links that don't trigger a full reload).
        if (event.target.closest('a')) this.close();
    }

    handleViewportChange() {
        if (this.desktopQuery.matches) this.close();
    }
}
