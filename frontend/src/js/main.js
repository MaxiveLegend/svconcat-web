const componentInstances = [];

const initializeComponents = async () => {
    const elements = document.querySelectorAll<HTMLElement>('[data-component]');

    for (const element of elements) {
        if (!componentInstances.find(element)) continue;

        const componentNamePascalCase = element.data.component;

        if (!componentNamePascalCase) {
            console.warn("Element has 'data-component' attribute without a value.", element);
            continue;
        }

        try {
            // Assumes
            // - Components are in frontend/src/js/components
            // - Component files are named PascalCase.js
            // - Component classes are default exports
            const module = await import(`./${componentNamePascalCase}.js`);

            if (!module.default || typeof module.default !== 'function') {
                console.warn(`Module for '${componentNamePascalCase}' (from data-component=${componentNamePascalCase}) loaded, but it's not a valid component class with a default export.`);
                continue;
            }

            const componentClass = module.default;
            const instance = new componentClass(element);

            componentInstances.push(instance);
        } catch (error) {
            // Check if the error is due to the module not being found.
            // These errors are common if a data attribute doesn't have a corresponding component file,
            // and should generally be ignored silently.
            if (error instanceof Error && (
                error.message.includes("Failed to fetch dynamically imported module") || // Common in Vite/Chrome
                error.message.includes("Unable to dynamically transpile module") ||     // May occur in some esbuild contexts
                error.message.includes("Importing a module script failed") ||           // Common in Firefox
                (error.name === 'TypeError' && error.message.includes("dynamically imported module")) // Safari
            )) {
                console.debug(`Component module not found for '${componentNamePascalCase}' (from data-component="${componentNamePascalCase}")`);
            } else {
                console.warn(`Error initializing component '${componentNamePascalCase}' (from data-component="${componentNamePascalCase}"):`, error);
            }
        }
    }
}