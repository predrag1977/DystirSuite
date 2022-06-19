var scrollInterval;
var matchesSameDay = 0;

function scrollOnMouseOver(direction) {
    var horizontalMenu = document.getElementById('horizontal_menu');
    if (direction == 'left') {
        horizontalMenu.scrollLeft -= horizontalMenu.scrollWidth / (horizontalMenu.scrollWidth / horizontalMenu.offsetWidth + 1);
    } else {
        horizontalMenu.scrollLeft += horizontalMenu.scrollWidth / (horizontalMenu.scrollWidth / horizontalMenu.offsetWidth + 1);
    }
}

function onPageResize() {
    scrollButtonVisibility();

    setMainContainerHeight();
    setMatchDetailsHeight();
    setMainContainerMobileClientHeight();
    setMatchDetailsMobileClientHeight();
}

function scrollButtonVisibility() {
    var horizontalMenu = document.getElementById('horizontal_menu');
    var horizontalMenuScroll = document.getElementById('horizontal_menu_wrapper');
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

function setMainContainerHeight() {
    var mainContainer = document.getElementById('main_container');
    if (mainContainer != null) {
        mainContainer.style.height = (window.innerHeight - 140) + "px";
    }
}

function setMatchDetailsHeight() {
    var matchDetailsContainer = document.getElementById('match_details_container');
    if (matchDetailsContainer != null) {
        var matchDetailsStaticPart = document.getElementById('match-details_static_part');
        matchDetailsContainer.style.height = (window.innerHeight - 80 - matchDetailsStaticPart.offsetHeight) + "px";
    }
}

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

function goBack() {
    history.back();
}