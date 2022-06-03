var scrollInterval;
var matchesSameDay = 0;

function scrollOnMouseOver(direction) {
    var horizontalMenu = document.getElementById('horizontal_menu');
    scrollInterval = setInterval(function () {
        if (direction == 'left') {
            horizontalMenu.scrollLeft -= 15;
        } else {
            horizontalMenu.scrollLeft += 15;
        }
    }, 70);
}

function scrollOnMouseOut() {
    window.clearInterval(scrollInterval);
}

function onPageResize() {
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

function setMatchDetailsHeight() {
    var matchDetailsContainer = document.getElementById('match_details_container');
    if (matchDetailsContainer == null) {
        return;
    }
    var matchDetailsStaticPart = document.getElementById('match-details_static_part');

    matchDetailsContainer.style.height = "calc(100vh - " + (90 + matchDetailsStaticPart.offsetHeight) + "px)";
}

function goBack() {
    history.back();
}