
function ExecuteOnSuccess(x) {
    $('.hidden-on-success').hide();
    $('#displaySuccessMessage').fadeIn();
    ShowErrorExport();
    hideLoading();
}

function ExecuteOnWarning() {
    $('#displayWarningMessage').fadeIn();
    hideLoading();
}

function ExecuteOnError() {
    $('.hidden-on-error').hide();
    $('#displayErrorMessage').fadeIn();
    ShowErrorExport();
    hideLoading();
}

function ShowErrorExport() {
    $('.show-export').slideDown('slow');
    $('.show-export').click(function () {
        $('#InstallationProgressPane').toggle();
    });
}

$(document).ready(function() {
    $('[title][title!=""]').tooltip();
    $('.checkbox').bootstrapSwitch();

    toggleUserPasswordFields();
    $(".containsWinAuthToggle").click(function () {
        toggleUserPasswordFields();
    });

    $("form").submit(function () {
        var runButton = $("#scriptInstallationSubmitButton");
        runButton.addClass('btn-disabled');
        runButton.removeClass('btn-primary');
        $("form").onsubmit = function () { return false; };
    });
})

function toggleUserPasswordFields() {
    if ($("#useWinAuth")[0].checked) {
        $(".formsAuth").hide();
        $(".winAuth").show();
    } else {
        $(".formsAuth").show();
        $(".winAuth").hide();
    }
}

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
    $(".loading-pane").hide();
    $(".dimmer").hide();
}