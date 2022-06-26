function setMainContainerMobileClientHeight() {
    var mainContainerMobileClient = document.getElementById('main_container_mobile_client');
    if (mainContainerMobileClient != null) {
        mainContainerMobileClient.style.height = (window.innerHeight - 240) + "px";
    }
}

function setMatchDetailsMobileClientHeight() {
    var matchDetailsContainerMobileClient = document.getElementById('match_details_container_mobile_client');
    if (matchDetailsContainerMobileClient != null) {
        var matchDetailsStaticPart = document.getElementById('match-details_static_part');
        matchDetailsContainerMobileClient.style.height = (window.innerHeight - 180 - matchDetailsStaticPart.offsetHeight) + "px";
    }
}