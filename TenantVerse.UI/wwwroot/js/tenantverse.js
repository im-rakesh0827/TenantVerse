window.tenantVerse = {

    saveDrawerState: function (isOpen) {
        localStorage.setItem("tv_drawer_open", isOpen);
    },

    getDrawerState: function () {

        const state = localStorage.getItem("tv_drawer_open");

        if (state === null)
            return true;

        return state === "true";
    }

};