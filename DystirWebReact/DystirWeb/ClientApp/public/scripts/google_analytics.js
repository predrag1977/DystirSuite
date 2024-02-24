/*Global site tag(gtag.js) - Google Analytics*/
function loadGoogleAnalytics(g_number_from_layout) {
    var g_number = "DWCGT486L9";
    var ga = document.createElement('script');
    ga.type = 'text/javascript';
    ga.async = true;
    ga.src = 'https://www.googletagmanager.com/gtag/js?id=G-'+ g_number;

    var s = document.getElementsByTagName('script')[0];
    s.parentNode.insertBefore(ga, s);
    sendTrack(g_number);
}

function sendTrack(g_number) {
    window.dataLayer = window.dataLayer || [];
    function gtag() { dataLayer.push(arguments); }
    gtag('js', new Date());
    gtag('config', 'G-DWCGT486L9');
    //gtag('config', 'G-' + g_number, { cookie_flags: 'max-age=7200;secure;samesite=none' });
    //gtag('config', 'G-' + , { cookie_flags: 'SameSite=None;Secure' });
}
/*End Google Analytics*/