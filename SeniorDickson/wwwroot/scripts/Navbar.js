const navbar = document.querySelector("header");
const check = navbar.querySelector("#nav-toggle");
const body = document.querySelector("body")
changeNavColour();
window.addEventListener("scroll", changeNavColour);
checkViewportWidth();
window.addEventListener("resize", checkViewportWidth);

function changeNavColour() {
    if ((window.pageYOffset > 0)||(check.checked == true)) {
        navbar.classList.add("white")
    }else if(window.pageYOffset <= 0){
        navbar.classList.remove("white")
    }
    if (check.checked == true) {
        body.style.overflow = "hidden";
    } else {
        body.style.overflow = "visible"
    }
}



$(document).ready(function () {
    var droppers = $(".dropper");
    var dropdowns = $(".dropdown");

    droppers.on("click", DropdownController);

    function DropdownController() {
        console.log("click");
        var target = $(this).closest("li").find(".dropdown");
        dropdowns.not(target).removeClass("active");
        target.toggleClass("active");
        droppers.not(this).removeClass("dropdown-active");
        $(this).toggleClass("dropdown-active");
    }
});

function checkViewportWidth() {
    if (window.innerWidth >= 1170) {
        // Do something when viewport width is 1170px or larger
        console.log("Viewport width is 1170px or larger");

        // Add hover event listener to droppers with related dropdowns
        $(navbar).find("li").each(function () {
            const dropper = $(this);
            const dropdown = dropper.closest("li").find(".dropdown");

            // Check if dropper has a related dropdown
            if (dropdown.length > 0) {
                dropper.hover(
                    function () {
                        navbar.classList.add("white");
                    },
                    function () {
                        navbar.classList.remove("white");
                    }
                );
            }
        });
    } else {
        // Remove hover event listener from droppers
        $(".dropper").off("mouseenter mouseleave");
    }
}

