///Document loading setting
$(document).ready(function () {
    SetUTCMinute();
    ///Hide CurrentPageText_EC control
    $("td[id*='CurrentPageText_EC']").hide();
    $("input[id*='CurrentPageText']").blur(function () {
        $("input[id*='CurrentPageHidden']").val($(this).val());
        ShowCurrentPageData();
    });

    $("table[id*='" + HealthGridView + "'] tr.dxgvFilterRow input").each(function () {
        var _keyUP = $(this).attr("onkeyup");
        $(this).attr("onChange", "return false;");
        $(this).attr("onkeyup", "return false");
        //$(this).attr("onkeydown", "if(e.keyCode == 13){aspxEKeyDown('ApplicationHealthSplitter_ApplicationPerformanceGridView_DXFREditorcol1', e); }");
    });

    //Gird Call Back Setting
    GridCallBackSetting();

    //Select All Checkbox setting
    SelectAllCheckBoxSetting();

});
// Set Timezone date to display
function SetTimeZoneDate() {
    eraseCookie("TimeZoneOffset","",-1);
   }

// Change VARSCAT grid properties
function toggleGridExpand() {
  var cells = $(".dxgv");
	var currentState = cells.css("white-space");
	if (currentState === "nowrap") {
		cells.css("white-space", "normal");
	} else {
		cells.css("white-space", "nowrap");
	}
}

//Grid selection changed
function GridSelectionChanged(s, e) {
    if (typeof (chart) == 'object') {
        chart.PerformCallback("ChartTypeChangedForVisibility");
    }

    SelectAllCheckBoxClient.SetChecked(grid.GetSelectedKeysOnPage().length == grid.GetVisibleRowsOnPage());
}

///Grid Loading Setting
function GridLoadSetting() {
    $("input[id*='CurrentPageHidden']").val(1);
    SetPageingNavigation();
}

///Grid Callback Setting
function GridCallBackSetting() {
    $("span[id*='TotalPageLabel']").html(grid.GetPageCount());

    SetPageingNavigation();

    SelectAllCheckBoxSetting();
		if (typeof splitter != "undefined")
			SetGridHeight(splitter.GetPaneByName("GridContainer"));
}

//Set Paging Navigation
function SetPageingNavigation() {
    var _totalPageNo = grid.GetPageCount();
    var _currentPageNo = (grid.GetPageIndex() + 1);

    if (_totalPageNo == 0) {
        _currentPageNo = 0;
    }

    ///Paging Navigation
    $("input[id*='CurrentPageText']").val(_currentPageNo);
    $("span[id*='CurrentPageLabel']").html(_currentPageNo);

    ///First & Previous Button Setting
    if (_currentPageNo <= 1) {
        $("a[id*='FirstDataLink'] img").addClass("transparant");
        $("a[id*='PreviousDataLink'] img").addClass("transparant");

        $("a[id*='FirstDataLink']").attr("NavigatePageNo", 0);
        $("a[id*='PreviousDataLink']").attr("NavigatePageNo", 0);
    }
    else {
        $("a[id*='FirstDataLink'] img").removeClass("transparant");
        $("a[id*='PreviousDataLink'] img").removeClass("transparant");

        $("a[id*='FirstDataLink']").attr("NavigatePageNo", 1);
        $("a[id*='PreviousDataLink']").attr("NavigatePageNo", _currentPageNo - 1);
    }

    ///Last & Next Button Setting
    if (_currentPageNo == _totalPageNo) {
        $("a[id*='NextDataLink'] img").addClass("transparant");
        $("a[id*='LastDataLink'] img").addClass("transparant");

        $("a[id*='NextDataLink']").attr("NavigatePageNo", 0);
        $("a[id*='LastDataLink']").attr("NavigatePageNo", 0);
    }
    else {
        $("a[id*='NextDataLink'] img").removeClass("transparant");
        $("a[id*='LastDataLink'] img").removeClass("transparant");

        $("a[id*='NextDataLink']").attr("NavigatePageNo", _currentPageNo + 1);
        $("a[id*='LastDataLink']").attr("NavigatePageNo", _totalPageNo);
    }

}

///Get current page data
function GetCurrentPageData(pagingLink) {
    var _navigatePageNo = Number($("a[id*='" + pagingLink + "']").attr("NavigatePageNo"));

    if (_navigatePageNo > 0) {
        $("input[id*='CurrentPageHidden']").val(_navigatePageNo);
        ShowCurrentPageData();
    }
}

///Show current page data
function ShowCurrentPageData() {
    var _navigatePageNo = Number($("input[id*='CurrentPageHidden']").val());

    if (_navigatePageNo > 0) {
        SetPageingNavigation();

        _navigatePageNo--;

        aspxGVPagerOnClick(HealthGridView, "PN" + _navigatePageNo);

    }
}

function SetUTCMinute() {
    var date = new Date();
    var minute = -date.getTimezoneOffset();    
    $("input[id*='hdnTimeZoneMinutes']").val(minute);
}

///Checking the start date & end date data
function CheckStartEndDate() {
    SetUTCMinute();

    var useUkDateFormat = false;
    if (typeof useUkDateFormateForDateRanges === "undefined")
        useUkDateFormat = false;
    else
        useUkDateFormat = useUkDateFormateForDateRanges;
        

    var _startDateEditValue = $("input[id*='StartDateEdit_I']").val();
    var _endDateEditValue = $("input[id*='EndDateEdit_I']").val();
    var _isError = false;
    var _errorMessage = '';
    $("#ErrorDiv").empty();

    $("#StartDateLabel").removeClass("errorMessageLightBlueBackGround");
    $("#EndDateLabel").removeClass("errorMessageLightBlueBackGround");
    $("#tdStartDate").attr("title", "");
    $("#tdEndDate").attr("title", "");
    $("#StartDateLabel").attr("title", "");
    $("#EndDateLabel").attr("title", "");
    ///Checking blank for start date & end date 
    if (XOR((_startDateEditValue == ''), (_endDateEditValue == ''))) {
        if (_startDateEditValue == '') {
            _errorMessage = "Please select start date";
            $("#StartDateLabel").addClass("errorMessageLightBlueBackGround");
            $("#tdStartDate").attr("title", _errorMessage);
            $("#StartDateLabel").attr("title", _errorMessage);
        }
        else if (_endDateEditValue == '') {
            _errorMessage = "Please select end date";
            $("#EndDateLabel").addClass("errorMessageLightBlueBackGround");
            $("#tdEndDate").attr("title", _errorMessage);
            $("#EndDateLabel").attr("title", _errorMessage);
        }

        _isError = true;
    }
    ///Checking for Start date is not exceed to end date
    else if ((_startDateEditValue != '') && (_endDateEditValue != '')) {
        var _startDate = parseDate(_startDateEditValue, useUkDateFormat);
        var _endDate = parseDate(_endDateEditValue, useUkDateFormat);

        if (_startDate > _endDate) {
            _errorMessage = "Start date cannot exceed end date";
            $("#StartDateLabel").addClass("errorMessageLightBlueBackGround");
            $("#EndDateLabel").addClass("errorMessageLightBlueBackGround");
            $("#tdStartDate").attr("title", _errorMessage);
            $("#tdEndDate").attr("title", _errorMessage);
            $("#StartDateLabel").attr("title", _errorMessage);
            $("#EndDateLabel").attr("title", _errorMessage);
            _isError = true;
        }
    }

    if (_isError) {
        /*$("#ErrorDiv").html("<ul><li>" + _errorMessage + "</li></ul>");*/
        return false;
    }

    return true;
}

///XOR Function
function XOR(valueA, valueB) {
    return (valueA || valueB) && !(valueA && valueB);
}
function SetSize() {
    var windowHeight = $(window).height();
    var windowWidth = $(window).width();
    if (typeof splitter != "undefined") {
    	splitter.SetHeight(windowHeight - 70);
    	splitter.SetWidth(windowWidth);
    }
    else {
			//This code path should only occur when 
    	var myPane = {};
    	myPane.height = windowHeight - 70;
    	myPane.width = windowWidth;
    	myPane.GetClientHeight = function () { return this.height; };
    	myPane.GetClientWidth = function () { return this.width; };
    	SetGridHeight(myPane);
    }
}


function SelectAllCheckBox() {
    if (grid.GetSelectedRowCount() == 0) {
        SelectAllCheckBoxClient.SetChecked(false);
    }
}

function SelectUnSelectCheckBox(sender) {
    var _isChecked = sender.GetChecked();

    grid.SelectAllRowsOnPage(_isChecked);
    sender.SetChecked(_isChecked);
}

function SelectAllCheckBoxSetting() {
    SelectAllCheckBoxClient.SetChecked(grid.GetSelectedKeysOnPage().length == grid.GetVisibleRowsOnPage());
    //    var _pageSize = 10;

    //    if ($("tr[id*='DXDataRow'] input[id*='DXSelBtn'][value='C']").length == _pageSize) {
    //        $("tr[id*='DXHeadersRow'] input[id*='DXSelBtn']").SetChecked(true);
    //    }
}
function SetGridHeight(pane) {
    var currentPage = Number($("label[id*='CurrentPageLabel']").html());
    var totalPage = Number($("label[id*='TotalPageLabel']").html());
    var totalRecord = Number($("label[id*='TotalRecordsLabel']").html());
    var v = $("select[id*='PageDropDownList']").val();

    if ((currentPage == totalPage) && (eval(totalPage) > 1)) {
        v = totalRecord - (totalPage - 1) * v;
    }
    var countHeight = (eval(v) * 24) + 33;

    /* Settings related to Dynamic vertical scroll bars */
    if  ((typeof pane != "undefined") && (pane.GetClientHeight() < 292)) {
        if (countHeight < pane.GetClientHeight()) {
            grid.SetHeight(countHeight + 20);
            $('#' + HealthGridView + '_DXMainTable').parent().addClass("myClass");
            $('#' + HealthGridView + '_DXHeaderTable').parent().addClass("marginremove");
        }
        else {
            grid.SetHeight(pane.GetClientHeight() - 20);
            $('#' + HealthGridView + '_DXMainTable').parent().removeClass("myClass");
            $('#' + HealthGridView + '_DXHeaderTable').parent().removeClass("marginremove");
        }
    }
    else {
        $('#' + HealthGridView + '_DXMainTable').parent().addClass("myClass");
        $('#' + HealthGridView + '_DXHeaderTable').parent().addClass("marginremove");
        if (countHeight > 292) {
            if (countHeight > pane.GetClientHeight()) {
                grid.SetHeight(pane.GetClientHeight() - 20);
                $('#' + HealthGridView + '_DXMainTable').parent().removeClass("myClass");
                $('#' + HealthGridView + '_DXHeaderTable').parent().removeClass("marginremove");
            }
            else {
                var browser = BrowserDetect.browser;
                if (browser == 'Explorer') {
                    grid.SetHeight(countHeight);
                }
                else {
                    grid.SetHeight(countHeight - 10);
                }
            }
        }
        else {
            grid.SetHeight(countHeight);
        }
    }
    /*end*/

    /*Settings related to Dynamic Horizontal scroll bars*/
    //var computedHeight = countHeight < pane.GetClientHeight() ? countHeight : pane.GetClientHeight();
    if (pane.GetClientWidth() > 800) {
        grid.SetWidth(pane.GetClientWidth());
        //$('#GridParentDiv').css({ "width": "100%" });
    }
    else {
        grid.SetWidth(800);
        //        if (computedHeight < 295) {          
        //            $('#GridParentDiv').css({ "width": (pane.GetClientWidth() + "px").toString(), "height": "295px" });
        //        }
        //        else
        //            $('#GridParentDiv').css({ "width": (pane.GetClientWidth() + "px").toString(), "height": (computedHeight + "px").toString() });
    }
    /*end*/

}


function CheckForOneServerInstance() {

    var AllSelectedServerInstancesAreTheSame = true;

    grid.GetSelectedFieldValues('DatabaseLocation', function (selectedValues) {
        if(0 == selectedValues.length ) return;
        var firstValue = selectedValues[0];
        for (i = 1; i < selectedValues.length; i++) {
            if(firstValue != selectedValues[i]){
                AllSelectedServerInstancesAreTheSame = false;
            }
        }
    });

    if (false == AllSelectedServerInstancesAreTheSame) {
        $('#lblErrorMessage').show();
    } else {
        $('#lblErrorMessage').hide();
    }

    return AllSelectedServerInstancesAreTheSame;
}

function DisableGo() {
    var goButton = $("#ServerHealthControl1_ShowDataButton");
    goButton.addClass('dxbDisabled');
    goButton.click(function () { });
}