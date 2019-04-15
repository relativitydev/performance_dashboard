function showLoading() {
    var width = $("body").width();
    var height = $("body").height();
    $(".loading-pane").css({
        top: ((height / 2) - 25),
        left: ((width / 2) - 50)
    }).show();
    $(".dimmer").show();

    //Restart animation in IE on postback
    setTimeout(function () {
        var pane = $(".loading-pane img")[0];
        if (pane)
            pane.src = pane.src;
    }, 50);
}

function hideLoading() {
    window.pdbReady = true;
    $(".loading-pane").hide();
    $(".dimmer").hide();
}

function showLoadingAfterTimeout(ms) {
    window.pdbReady = false;
    setTimeout(function () {
        if (!window.pdbReady)
            showLoading();
    }, ms);
}

$(document).ready(function () {
    $("a").click(function (e) {
        if ($(this).hasClass('skip-loading'))
            return;

        //Show the loading panel after 1 second on click (unless middle mouse button is pressed)
        if (event.button != 4 && (!e || e.which != 2)) {
            showLoadingAfterTimeout(1000);
        }
    });
});