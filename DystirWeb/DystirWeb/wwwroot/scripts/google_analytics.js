/*Global site tag(gtag.js) - Google Analytics*/
function loadGoogleAnalytics(ua_number) {
    var ga = document.createElement('script');
    ga.type = 'text/javascript';
    ga.async = true;
    ga.src = 'https://www.googletagmanager.com/gtag/js?id=UA-170595956-1'+ ua_number;

    var s = document.getElementsByTagName('script')[0];
    s.parentNode.insertBefore(ga, s);
    sendTrack(ua_number);
}

function sendTrack(ua_number) {
    window.dataLayer = window.dataLayer || [];
    function gtag() { dataLayer.push(arguments); }
    gtag('js', new Date());
    gtag('config', 'UA-' + ua_number, { cookie_flags: 'SameSite=None;Secure' });
}
/*End Google Analytics*/