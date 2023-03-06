﻿function onPageResize() {
    scrollButtonVisibility("");
    scrollButtonVisibility("_bottom");

    setMainContainerHeight();
    setMatchDetailsHeight();
}

function scrollOnMouseOver(direction) {
    var horizontalMenu = document.getElementById('horizontal_menu');
    if (direction == 'left') {
        horizontalMenu.scrollLeft -= horizontalMenu.scrollWidth / (horizontalMenu.scrollWidth / horizontalMenu.offsetWidth + 1);
    } else {
        horizontalMenu.scrollLeft += horizontalMenu.scrollWidth / (horizontalMenu.scrollWidth / horizontalMenu.offsetWidth + 1);
    }
}

function scrollOnMouseOverBottom(direction) {
    var horizontalMenu = document.getElementById('horizontal_menu_bottom');
    if (direction == 'left') {
        horizontalMenu.scrollLeft -= horizontalMenu.scrollWidth / (horizontalMenu.scrollWidth / horizontalMenu.offsetWidth + 1);
    } else {
        horizontalMenu.scrollLeft += horizontalMenu.scrollWidth / (horizontalMenu.scrollWidth / horizontalMenu.offsetWidth + 1);
    }
}

function scrollButtonVisibility(position) {
    var horizontalMenu = document.getElementById('horizontal_menu' + position);
    var horizontalMenuScroll = document.getElementById('horizontal_menu_wrapper' + position);
    if (horizontalMenu == null) {
        return;
    }
    var scrollButtonLeft = document.getElementById('scroll_button_left' + position);
    var scrollButtonRight = document.getElementById('scroll_button_right' + position);

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
    var mainContainerVerticalList = document.getElementById('main_container_vertical_list');
    var mainContainerVerticalListStandings = document.getElementById('main_container_vertical_list_standings');
    if (mainContainer != null) {
        mainContainer.style.height = (window.innerHeight - 140) + "px";
    }
    if (mainContainerVerticalList != null) {
        mainContainerVerticalList.style.height = (window.innerHeight - 100) + "px";
    }
    if (mainContainerVerticalListStandings != null) {
        mainContainerVerticalListStandings.style.height = (window.innerHeight - 50) + "px";
    }
}

function setMatchDetailsHeight() {
    var matchDetailsContainer = document.getElementById('match_details_container');
    if (matchDetailsContainer != null) {
        var matchDetailsStaticPart = document.getElementById('match-details_static_part');
        matchDetailsContainer.style.height = (window.innerHeight - 80 - matchDetailsStaticPart.offsetHeight) + "px";
    }
}

function goBack() {
    history.back();
}