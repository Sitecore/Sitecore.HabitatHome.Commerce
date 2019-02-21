function isMobile() {
    if (/Mobi/i.test(navigator.userAgent)) {
        return true;
    }

    return false;
}