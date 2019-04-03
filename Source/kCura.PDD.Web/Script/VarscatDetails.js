var varscatDetails = varscatDetails || {};
varscatDetails.sources = {
    chart: "",
    grid: "",
    csv: ""
}
varscatDetails.emptyTableMessage = "No sample data available with current filters";

varscatDetails.makeChart = function () {
    var parameters = getParameters();
    setupScoreCharts(this.sources.chart + parameters,
        function (hour) {
            $(".datepicker").val(hour);
            updatePageFilters();
        });
}

varscatDetails.makeGrid = function() {
	return varscatDetails.makeGridTable("#varscat-table", null, null, {});
}

varscatDetails.makeGridTable = function (tableId, drawCallback, initCallback, additionalParameters) {
	var parameters = getParameters();

	for (prop in additionalParameters)
	{
		parameters += '&' + prop + '=' + additionalParameters[prop];
	}
	var dt = setupDataTables(tableId, this.sources.grid + parameters, this.sources.csv + parameters, this.emptyTableMessage, drawCallback, initCallback);
	return dt;
}

varscatDetails.makeServerSelect = function () {
    $("#pageServerSelect").multiselect({
        buttonText: function (options, select) {
            if (options.length === 0) {
                return '(All)';
            }
            else if (options.length > 1) {
                return options.length + ' servers selected';
            }
            else {
                var labels = [];
                options.each(function () {
                    if ($(this).attr('label') !== undefined) {
                        labels.push($(this).attr('label'));
                    }
                    else {
                        labels.push($(this).html());
                    }
                });
                return labels.join(', ') + '';
            }
        },
        buttonTitle: function (options, select) {
            return '';
        }
    });
}

function getParameters() {
    var sd = $("#startDate").val();
    var ed = $("#endDate").val();
    var serverId = $("#pageServerFilter").val();
    var serverSelection = $("#serverSelection").val();
    var offset = $("#TimezoneOffset").val() || '0';
    return "?TimezoneOffset=" + offset + "&StartDate=" + sd + "&EndDate=" + ed + "&ServerArtifactId=" + serverId + "&ServerSelection=" + serverSelection;
}

$(document).ready(function () {
    shiftGridPosition();

    $(".img-hide-pivot").click(function () {
        $(".pivot-bar").hide();
        $(".chart-bar").hide();
        $(".img-hide-pivot").hide();
        $(".img-show-pivot").show();

        //By Relativity's convention, hiding pivot controls ALWAYS shows the grid (and turning pivot back on will continue to show it)
        $(".img-show-grid").hide();
        $(".img-hide-grid").show();
        $(".table-container").show();
        shiftGridPosition();
    });

    $(".img-show-pivot").click(function () {
        $(".pivot-bar").show();
        if ($(".img-hide-chart:visible").length > 0)
            $(".chart-bar").show();
        $(".img-show-pivot").hide();
        $(".img-hide-pivot").show();
        shiftGridPosition();
    });

    $(".img-hide-chart").click(function () {
        $(".chart-bar").hide();
        $(".img-hide-chart").hide();
        $(".img-show-chart").show();
        shiftGridPosition();
    });

    $(".img-show-chart").click(function () {
        $(".chart-bar").show();
        $(".img-show-chart").hide();
        $(".img-hide-chart").show();
        varscatDetails.makeChart();
        shiftGridPosition();
    });

    $(".img-hide-grid").click(function () {
        $(".table-container").hide();
        $(".img-hide-grid").hide();
        $(".img-show-grid").show();
    });

    $(".img-show-grid").click(function () {
        $(".table-container").show();
        $(".img-show-grid").hide();
        $(".img-hide-grid").show();
    });

    $(".updatePageLevelFilters").click(updatePageFilters);
})

function shiftGridPosition() {
    var pivotHeight = $(".pivot-bar:visible").outerHeight() || 0;
    var padding = 20 + pivotHeight;
    $(".page-container").css('padding-top', padding + 'px');
}

function initializeDatePickers(start) {
    //Determine information about the acceptable input range
    var maxDate = new Date();
    var sampleStart = new Date(start);
    var minDate = Object.prototype.toString.call(sampleStart) != "[object Date]" || isNaN(sampleStart)
        ? null
        : sampleStart;
    var yearStart = minDate ? minDate.getFullYear() : maxDate.getFullYear();
    var sVal = $(".startdatepicker").val();
    var eVal = $(".enddatepicker").val();
    var dateFormat = $("#DateFormatString").val() || 'm/d/Y';

    //Initialize datepicker controls
    var sdp = $(".startdatepicker").datetimepicker({
        format: dateFormat,
        allowBlank: false,
        closeOnDateSelect: true,
        validateOnBlur: true,
        yearStart: yearStart,
        yearEnd: maxDate.getFullYear(),
        timepicker: false,
        defaultDate: minDate,
        maxDate: maxDate,
        minDate: minDate
    });
    var edp = $(".enddatepicker").datetimepicker({
        format: dateFormat,
        allowBlank: false,
        closeOnDateSelect: true,
        validateOnBlur: true,
        yearStart: yearStart,
        yearEnd: maxDate.getFullYear(),
        timepicker: false,
        defaultDate: maxDate,
        maxDate: maxDate,
        minDate: minDate
    });

    //When start/end update, check whether the other filter needs to be restricted as well
    $(".startdatepicker").change(function () {
        var sd = $(".startdatepicker").val();
        var s = sdp.getCurrentValue();
        var e = edp.getCurrentValue();

        if (s && e && sd) {
            var min = new Date(minDate);
            min.setHours(0);
            var max = new Date();
            if (s < min || s > max)
                $(".startdatepicker").val(sVal);
            else if (s > e)
                $(".enddatepicker").val(sd);
        }
    });
    $(".enddatepicker").change(function () {
        var ed = $(".enddatepicker").val();
        var s = sdp.getCurrentValue();
        var e = edp.getCurrentValue();

        if (s && e && ed) {
            var min = new Date(minDate);
            min.setHours(0);
            var max = new Date();
            if (e < min || e > max)
                $(".enddatepicker").val(eVal);
            else if (e < s)
                $(".startdatepicker").val(ed);
        }
    });
}

function updateSelectedServers() {
    var selection = $("#pageServerSelect").val()

    if (selection) {
        var selectedServers = selection.join();
        $("#serverSelection").val(selectedServers);
    } else {
        $("#serverSelection").val('');
    }
}