function setpage() {

	$('.table-container').show();

	varscatDetails.sources.grid = 'api/MaintenanceWindowScheduler/GetListOfSchedules';
	varscatDetails.sources.csv = 'api/MaintenanceWindowScheduler/GenerateCSV/';
	varscatDetails.emptyTableMessage = "No Maintenance Windows available with current filters";

	showLoadingAfterTimeout(1000);
	varscatDetails.makeServerSelect();

	var dt = varscatDetails.makeGridTable('#varscat-table-MaintenanceWindows',
		function () {
			$('#varscat-table-MaintenanceWindows th:first()').css('width', '40px');
			$('#varscat-table-MaintenanceWindows tr td:nth-child(5)')
				.each(function (i, elm) {
				    var column = $(elm).html();
				    var newColumn = '<span class="moreless moreless-group-0">' + column + '</span>';
				    $(elm).html(newColumn);
				});
		    $('.moreless').moreless();
		}
		,
		function() {
			$('#varscat-table-MaintenanceWindows th:first()').css('width', '40px');
		},
		{});

	//Bind events
	dt.on('xhr', hideLoading);
	dt.on('preXhr', showLoadingAfterTimeout(1000));


	initMWDateFilterPickers();

	$(".resetPageLevelFilters").click(function () {
		$("#pageServerSelect option:selected").each(function () {
			$(this).prop('selected', false);
		});
		$("#pageServerSelect").multiselect('refresh');
		$(".datepicker").datetimepicker('reset');
		updatePageFilters();
	});

	//Grab the data
	dt.draw(false);
}

function initMWDateFilterPickers() {
    var sampleStart = new Date();
    var dateFormat = $("#DateFormatString").val() || 'm/d/Y';
    var timeFormat = $("#TimeFormatString").val() || 'H:i';
    $(".datetimepicker").datetimepicker({
        format: dateFormat + ' ' + timeFormat,
        formatTime: timeFormat,
        validateOnBlur: false,
        yearStart: sampleStart.getFullYear(),
        defaultTime: sampleStart.getUTCHours()+" AM"
    });
}

function updatePageFilters() {
	var dt = varscatDetails.makeGrid();
	dt.refreshFilterValues();
	Placeholders.enable();

	dt.draw();
}

