Date.prototype.AddDays = function (days) { this.setDate(this.getDate() + days); return this; }
Date.prototype.AddHours = function (hours) { this.setHours(this.getHours() + hours); return this; }
Date.prototype.AddMilliseconds = function (milliseconds) { this.setMilliseconds(this.getMilliseconds() + milliseconds); return this; }
Date.prototype.AddMinutes = function (minutes) { this.setMinutes(this.getMinutes() + minutes, this.getSeconds(), this.getMilliseconds()); return this; }
Date.prototype.AddMonths = function (months) { this.setMonth(this.getMonth() + months, this.getDate()); return this; }
Date.prototype.AddSeconds = function (seconds) { this.setSeconds(this.getSeconds() + seconds, this.getMilliseconds()); return this; }
Date.prototype.AddYears = function (years) { this.setFullYear(this.getFullYear() + years); return this; }  

function PreDefinedDateRangeSelected() {
    var ddlPredefinedDateRanges = $('select[id*="DdlPreDefined"]').last();

    //get the selected predefined date range index
    var dateRange = DateRangesComboBoxClient.GetValue();

    var today = new Date();
    var daysToAdd = 0;
    var monthsToAdd = 0;
    var yearsToAdd = 0;

    var setEndDateToYesterday = false;

    switch (dateRange) {
        case '1': /* today */
            //do nothing
            break;
        case '2': /* yesterday */
            daysToAdd = -1;
            setEndDateToYesterday = true;
            break;
        case '3': /* past 7 days */
            daysToAdd = -7;
            break;
        case '4': /* past month */
            monthsToAdd = -1;
            break;
        case '5': /* past 3 months */
            monthsToAdd = -3;
            break;
    }

    if ('0' != dateRange) {
        var today = new Date();
        //zero out the time and make it just the date
        today = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 0, 0, 0, 0); //new Date(year, month, day, hours, minutes, seconds, milliseconds);

        if (!setEndDateToYesterday) {
            //set the end date as today
            EndDateEditClient.SetDate(today);
        } 
        //change todays date to the beginning of the predefined date range (by subtracting the days/months)
        today.AddDays(daysToAdd);
        today.AddMonths(monthsToAdd);
        today.AddYears(yearsToAdd);
        //set the start date to the beginning of the date range

        if (setEndDateToYesterday) {
            EndDateEditClient.SetDate(today);
        }

        StartDateEditClient.SetDate(today);
    }

    UpdateAllowedDates();
}

function UpdateAllowedDates(s, e) {
    //Set min and max on date controls
    EndDateEditClient.SetMinDate(StartDateEditClient.GetDate());
    StartDateEditClient.SetMaxDate(EndDateEditClient.GetDate() || new Date());
}
