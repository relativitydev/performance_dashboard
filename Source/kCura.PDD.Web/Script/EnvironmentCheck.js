function setpage(dataGridType) {
	hidetables();

	$('.table-container-' + dataGridType).show();

	var gridResource = 'recommendations' == dataGridType ? 'Recommendations/' : dataGridType + 'info/';
	varscatDetails.sources.grid = 'api/EnvironmentCheck/' + gridResource;
	varscatDetails.sources.csv = 'api/EnvironmentCheck/GenerateCSV/';
	varscatDetails.emptyTableMessage = "No environment analysis information available with current filters";

	showLoadingAfterTimeout(1000);
	varscatDetails.makeServerSelect();

	var dt = varscatDetails.makeGridTable('#varscat-table-' + dataGridType,
		function () {
			$('.moreless').moreless();
		}
		, null, { gridType: dataGridType });

	//Bind events
	dt.on('xhr', hideLoading);
	dt.on('preXhr', showLoadingAfterTimeout(1000));

	dt.initDatePickers('1900-01-01T00:00:00Z');

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

function hidetables() {
	var types = ['recommendations', 'server', 'database'];
	for (var i = 0; i < types.length; i++) {
		$('.table-container-' + types[i]).hide();
	}
}

function updatePageFilters() {
	updateSelectedServers();

	if ($(".scoreChart:visible").length > 0)
		varscatDetails.makeChart();
	var dt = varscatDetails.makeGrid();
	dt.refreshFilterValues();
	var pageServerId = $("#serverSelection").val();
	dt.assignFilterValue(1, pageServerId.split(',').length == 1 ? pageServerId : '');
	Placeholders.enable();

	dt.draw();
}


function SetLastRunTime(lastRun) {
	var date = new Date(Date.parse(lastRun + 'Z'));
	if (date.getFullYear() == 1900) {
		$("#lblLastRun").html('Last Run: Pending');
	} else {
		var dateStr = date.toLocaleString();
		$('#lblLastRun').html('Last Run: ' + dateStr);
	}
}

function GetLastRunTime(config) {
	var configValue = config ? 'true' : 'false';
	$.get('api/EnvironmentCheck/TuningForkLastRun?config=' + configValue, function (response) {
		SetLastRunTime(response);
		setTimeout(GetLastRunTime, 1000 * 20);
	});
}

function ResetTuningFork(config)
{
	var configValue = config ? 'true' : 'false';
	$('#btnResetTuningFork').prop("disabled", true);

	var csrfToken = $("input[name='__RequestVerificationToken']").val();
	$.ajax({
		type: "POST",
		url: 'api/EnvironmentCheck/ResetTuningForkLastRun?config=' + configValue,
        headers: { "X-CSRF-Header": csrfToken },
		success: function (response) {
			SetLastRunTime(response);
			$('#btnResetTuningFork').prop("disabled", false);
		}
	});
}