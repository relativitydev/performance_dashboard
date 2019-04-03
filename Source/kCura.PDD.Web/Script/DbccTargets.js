function ExecuteOnSuccess() {
    $('#displaySuccessMessage').fadeIn();
}

function ExecuteOnWarning() {
    $('#displayWarningMessage').fadeIn();
}

function ExecuteOnError() {
    $('#displayErrorMessage').fadeIn();
}

function hideResponseMessages() {
    $('#displaySuccessMessage').hide();
    $('#displayWarningMessage').hide();
    $('#displayErrorMessage').hide();
}

function editServer(target) {
    $(".view-mode").hide();

    var formGroup = $(target).parent().parent();
    formGroup.find(".edit-mode").show();
}

function cancelServer(target) {
    $(".edit-mode").hide();
    $(".view-mode").show();

    var formGroup = $(target).parent().parent();
    revertServerFields(formGroup);
}

function revertServerFields(formGroup) {
    formGroup.find(".current-database").first().val(formGroup.children(".original-database").first().val());
    var currentActive = formGroup.find(".current-active");
    if (currentActive[0].checked != formGroup.children(".original-active")[0].checked)
        currentActive.click();
}

function initializeEvents() {
    $(".edit-button").click(function (e) {
        hideResponseMessages();
        editServer(e.target);
    });
    $(".cancel-button").click(function (e) {
        hideResponseMessages();
        cancelServer(e.target);
    });
    $(".save-button").click(function (e) {
        hideResponseMessages();
        $("form").submit();
    });
    $("form").submit(function () {
        showLoading();
        var formGroup = $(".cancel-button:visible").first().parent().parent();
        $("#targetId").val(formGroup.children(".server-id").first().val());
        $("#serverName").val(formGroup.children(".server-name").first().val());
        $("#databaseName").val(formGroup.find(".current-database").first().val());
        $("#isActive")[0].checked = formGroup.find(".current-active")[0].checked;
        $("form").onsubmit = function () { return false; };
    });
}

$(document).ready(function () {
    $('[title][title!=""]').tooltip();
    $('.checkbox').bootstrapSwitch();
    initializeEvents();

    var targetId = $("#targetId").val();
    if (targetId)
    {
        var formGroup = $("input.server-id[value='" + targetId + "']").parent();

        //Set edit mode fields
        var dbName = $("#databaseName").val();
        formGroup.find(".current-database").val(dbName);
        var currentActive = formGroup.find(".current-active");
        var isActive = $("#isActive")[0].checked;
        if (currentActive[0].checked != isActive)
            currentActive.click();

        if ($("#displaySuccessMessage:visible").length == 0) {
            //Validation or save failed -- display fields in edit mode
            editServer(formGroup.find(".edit-button")[0]);
        }
    }

    hideLoading();
});