function getTimeZoneOffset() {
    let timeOffset = new Date().getTimezoneOffset();
    return timeOffset.toString();
}