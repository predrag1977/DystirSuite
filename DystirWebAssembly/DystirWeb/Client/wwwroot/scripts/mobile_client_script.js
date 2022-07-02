function setMainContainerMobileClientHeight() {
    scrollButtonVisibilityMobileClient();
    var mainContainerMobileClient = document.getElementById('main_container_mobile_client');
    if (mainContainerMobileClient != null) {
        mainContainerMobileClient.style.height = (window.innerHeight - 170) + "px";
    }
}

function setMatchDetailsMobileClientHeight() {
    scrollButtonVisibilityMobileClient();
    var matchDetailsContainerMobileClient = document.getElementById('match_details_container_mobile_client');
    if (matchDetailsContainerMobileClient != null) {
        var matchDetailsStaticPart = document.getElementById('match-details_static_part');
        matchDetailsContainerMobileClient.style.height = (window.innerHeight - 110 - matchDetailsStaticPart.offsetHeight) + "px";
    }
}

function scrollButtonVisibilityMobileClient() {
    var horizontalMenu = document.getElementById('horizontal_menu_mobile_client');
    var horizontalMenuScroll = document.getElementById('horizontal_menu_wrapper_mobile_client');
    if (horizontalMenu == null) {
        return;
    }
    var scrollButtonLeft = document.getElementById('scroll_button_left');
    var scrollButtonRight = document.getElementById('scroll_button_right');

    if (horizontalMenu.offsetWidth >= horizontalMenuScroll.offsetWidth) {
        scrollButtonLeft.style.visibility = "hidden";
        scrollButtonRight.style.visibility = "hidden";
    }
    else {
        scrollButtonLeft.style.visibility = "visible";
        scrollButtonRight.style.visibility = "visible";
    }
}