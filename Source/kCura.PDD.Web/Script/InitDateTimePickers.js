function InitDateTimePickers(start, onClose) {
    var minDate = new Date();
    var dateFormat = $("#DateFormatString").val() || 'm/d/Y';
    var timeFormat = $("#TimeFormatString").val() || 'g A';
    $(".datetimepicker").datetimepicker({
        format: dateFormat + ' ' + timeFormat,
        formatTime: timeFormat,
        validateOnBlur: false,
        yearStart: minDate.getFullYear(),
        //yearEnd: maxDate.getFullYear(),
        //maxDate: maxDate,
        minDate: minDate,
        //minTime: minDate.getTime(),
        onClose: onClose
    });
}