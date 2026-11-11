(() => {
    const key = "energytwin-theme";
    const modes = ["system", "light", "dark"];
    let current = "system";

    function apply(mode) {
        current = modes.includes(mode) ? mode : "system";
        document.documentElement.dataset.theme = current;
    }

    try {
        apply(localStorage.getItem(key));
    } catch {
        apply("system");
    }

    window.energyTwinTheme = {
        get: () => current,
        set(mode) {
            apply(mode);
            try {
                localStorage.setItem(key, current);
            } catch {
                // Das gewählte Theme gilt weiterhin in diesem Tab.
            }
        }
    };
})();
