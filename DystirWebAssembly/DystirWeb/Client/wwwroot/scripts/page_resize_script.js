function onPageResize() {
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
        mainContainerVerticalList.style.height = (window.innerHeight - 50) + "px";
    }
    if (mainContainerVerticalListStandings != null) {
        mainContainerVerticalListStandings.style.height = (window.innerHeight - 50) + "px";
    }
}

function setMatchDetailsHeight() {
    var matchDetailsContainer = document.getElementById('match_details_container');
    if (matchDetailsContainer != null) {
        var matchDetailsStaticPartHeight = 0;
        var matchDetailsStaticPart = document.getElementById('match-details_static_part');
        if (matchDetailsStaticPart != null) {
            matchDetailsStaticPartHeight = matchDetailsStaticPart.offsetHeight;
        }
        matchDetailsContainer.style.height = (window.innerHeight - 80 - matchDetailsStaticPartHeight) + "px";
    }
}

function goBack() {
    history.back();
}

function getMatchItemWidth(numberOfMatches) {
    var items = document.getElementsByClassName("match_item_same_day_share_details");
    for (var i = 0; i < items.length; i++) {
        items[i].style.width = "" + 100 / numberOfMatches - numberOfMatches * 0.1 + "%";
    }
}