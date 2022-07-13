function matchDateFormat(date) {
    var correctDate = new Date(Date.parse(date));
    if (isNaN(correctDate.valueOf())) {
        return "";
    }
    
    return correctDate.toLocaleDateString("fo-FO", { weekday: 'short'});
}