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
    setMainContainerHeight();
    setMatchDetailsHeight();
    var horizontalMenu = document.getElementById('horizontal_menu');
    if (horizontalMenu == null) {
        return;
    }
    var horizontalMenuScroll = horizontalMenu.children[0];
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
    if (mainContainer == null) {
        return;
    }
    mainContainer.style.height = (window.innerHeight - 140) + "px";
}

function setMatchDetailsHeight() {
    var matchDetailsContainer = document.getElementById('match_details_container');
    if (matchDetailsContainer == null) {
        return;
    }
    var matchDetailsStaticPart = document.getElementById('match-details_static_part');
    matchDetailsContainer.style.height = (window.innerHeight - 80 - matchDetailsStaticPart.offsetHeight) + "px";
}

function goBack() {
    history.back();
}