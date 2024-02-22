 export function scrollButtonVisibility() {
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

export function scrollOnClick(direction) {
    var horizontalMenu = document.getElementById('horizontal_menu');
    if (direction == 'left') {
        horizontalMenu.scrollTo({
            top: 0,
            left: horizontalMenu.scrollLeft - 80,
            behavior: 'smooth'
        });
    } else {
        horizontalMenu.scrollTo({
            top: 0,
            left: horizontalMenu.scrollLeft + 80,
            behavior: 'smooth'
        });
    }
}
