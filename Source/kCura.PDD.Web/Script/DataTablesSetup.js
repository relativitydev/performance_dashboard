

function setupDataTables(tableId, dataSource, dataSourceCSV, emptyTableMsg, drawCallback, initCallback) {
	if (typeof tableId === 'undefined' || !dataSource || !dataSourceCSV) {
		return;
	}

	if (window.dtInternal)
		window.dtInternal.destroy(); 

	$.fn.dataTable.ext.errMode = 'throw';
	var table = $(tableId).DataTable({
		"serverSide": true,
		"bServerSide": true,
		"sAjaxSource": dataSource,
		"bProcessing": true,
		"processing": true,
		"pagingType": "full",
		"initComplete": function () {
			if (typeof initCallback != 'undefined' && initCallback != null) {
				initCallback();
			}
		},
		"bSortCellsTop": true,
		"aaSorting": [],
		"dom": '<"table-controls"pi <"divider"> <"clear-filters disabled"> <"hide-filters"> <"export-excel">><"scrollable-table"t><"bottom"l>',
		"drawCallback": function () {
			$('[title][title!=""]').tooltip({ html: true });
			if (typeof drawCallback != 'undefined' && drawCallback != null) {
				drawCallback();
			}
		},
		"language": {
			"emptyTable": emptyTableMsg,
			"info": "Items _START_ - _END_ (of _TOTAL_)",
			"infoFiltered": "",
			"infoEmpty": "&nbsp;",
			"lengthMenu": "Items per page: _MENU_",
			"paginate": {
				"first": "&nbsp;",
				"last": "&nbsp;",
				"next": "&nbsp;",
				"previous": "&nbsp;"
			}
		}
	});

	table.initDatePickers = function (start) {
		var maxDate = new Date();
		var sampleStart = new Date(start);
		var minDate = Object.prototype.toString.call(sampleStart) != "[object Date]" || isNaN(sampleStart)
            ? null
            : sampleStart;
		var dateFormat = $("#DateFormatString").val() || 'm/d/Y';
		var timeFormat = $("#TimeFormatString").val() || 'g A';
		$(".datetimepicker").datetimepicker({
			format: dateFormat + ' ' + timeFormat,
			formatTime: timeFormat,
			validateOnBlur: false,
			yearStart: minDate ? minDate.getFullYear() : maxDate.getFullYear(),
			yearEnd: maxDate.getFullYear(),
			maxDate: maxDate,
			minDate: minDate
		});
	}

	table.assignFilterValue = function (index, value, operand) {
		var target = $(".searchRow th").children(".search")[index];
		if (value) {
			target.value = value;
			if (operand) {
				$(target).siblings('.operand').val(operand);
			}
			tableWrapper.find('.clear-filters').removeClass('disabled');
			table.column(index).search(value, operand);
		} else {
			target.value = '';
			table.column(index).search('', '0');
		}
	}

	table.refreshFilterValues = function () {
		var searchItems = $(".searchRow th").children(".search");
		for (var i = 0; i < searchItems.length; i++) {
			var target = searchItems[i];
			if (target.value != "(All)") {
				var operand = $(target).siblings('.operand').val();
				table.column(i).search(target.value, operand);
			}
		}
	}

	table.assignSort = function (index, direction) {
		if (direction)
			table.column(index).order(direction);
	}

	table.assignStartIndex = function (index) {
		if (index > 0)
			table.settings()[0]._iDisplayStart = index;
	}

	var tableWrapper = $(tableId + "_wrapper");

	//Setup search information
	var searchHeaders = $(tableId + ' .searchRow th');
	table.columns().eq(0).each(function (colIdx) {
		var header = searchHeaders[colIdx];
		//$('#test_filter input').unbind();
		//This handles the enter key and most changes to search inputs
		$('.search', header).on('keyup change', function (e) {
			if (e.type != 'keyup' || e.keyCode == 13) {
				var operand = $(this).siblings('.operand').val();
				table
                    .column(colIdx)
                    .search(this.value, operand)
                    .draw();

				tableWrapper.find('.clear-filters').removeClass('disabled');
			}
		});
		//This handles changes to filter operands
		$('.operand', header).on('change', function (e) {
			var search = $(this).siblings('.search').val();
			table
                .column(colIdx)
                .search(search, this.value)
                .draw();
		});
		//This is here to handle IE's input clear (X) icon
		$('input.search', header).on('mouseup', function (e) {
			var field = $(this);
			setTimeout(function () {
				var operand = $(this).siblings('.operand').val();
				var updatedVal = field.val();
				if (updatedVal === '') {
					table
                        .column(colIdx)
                        .search(updatedVal, operand)
                        .draw();

					tableWrapper.find('.clear-filters').removeClass('disabled');
				}
			}, 100);
		});

		if ($(header).hasClass("optionSelect")) {
			var select = $(header).find('select');
			table.column(colIdx).data().unique().sort().each(function (value) {
				select.append('<option value="' + value + '">' + value + '</option>');
			});
		}
	});

	tableWrapper.on('click', '.hide-filters', function () {
		var button = $(this);
		button.removeClass("hide-filter").addClass("show-filters");
		tableWrapper.find('.searchRow').hide();
	});

	tableWrapper.on('click', '.show-filters', function () {
		var button = $(this);
		button.removeClass("show-filters").addClass("hide-filter");
		tableWrapper.find('.searchRow').show();
	});

	tableWrapper.on('click', '.clear-filters', clearFilters);

	tableWrapper.on('click', '.export-excel', function () {
		var myTableSettings = $(tableId).dataTable().dataTableSettings[0];
		var data = $.param(myTableSettings.oAjaxData);
		var inputs = '';
		$.each(data.split('&'), function () {
			var pair = this.split('=');
			inputs += '<input type="hidden" name="' + pair[0] + '" value="' + pair[1] + '" />';
		});
		var myForm = $("<form>")
			.attr("method", "POST")
			.attr("action", dataSourceCSV)
			.append(inputs)
    		.appendTo("body")
    		.submit()
    		.remove();

		Placeholders.enable();
	});

	window.dtInternal = table;
	return table;
};

var clearFilters = function (e) {
	var button = $(this);
	if (button.hasClass('disabled'))
		return;

	button.addClass("disabled");

	var tableWrapper = $(this).closest('.dataTables_wrapper');
	tableWrapper.find('.searchRow').find('select, input').val("");

	Placeholders.enable();

	tableWrapper.find('table').DataTable().columns().search('').draw();
}