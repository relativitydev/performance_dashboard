function ExecuteOnSuccess(x) {
    $('.hidden-on-success').hide();
    $('#displaySuccessMessage').fadeIn();
    hideLoading();
}

function ExecuteOnWarning() {
    $('#displayWarningMessage').fadeIn();
    hideLoading();
}

function ExecuteOnError() {
    $('.hidden-on-error').hide();
    $('#displayErrorMessage').fadeIn();
    hideLoading();
}

$(document).ready(function () {
    $('[title][title!=""]').tooltip();

    $("form").submit(function () {
        showLoading();
        var runButton = $("#scriptInstallationSubmitButton");
        runButton.addClass('btn-disabled');
        runButton.removeClass('btn-primary');
        $("form").onsubmit = function () { return false; };
    });
});