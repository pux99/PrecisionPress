mergeInto(LibraryManager.library, {
    IsMobileBrowser: function () {
        var ua = navigator.userAgent || navigator.vendor || window.opera;

        // Simple mobile detection
        if (/android/i.test(ua)) return 1;
        if (/iPad|iPhone|iPod/.test(ua)) return 1;
        if (/windows phone/i.test(ua)) return 1;

        return 0;
    }
});