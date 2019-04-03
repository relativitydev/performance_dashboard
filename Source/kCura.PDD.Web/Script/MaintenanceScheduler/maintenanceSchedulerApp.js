(function () {
    const $ = this.$;
    const api = '/relativity/custompages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/api/MaintenanceWindowScheduler/';
    let firstAvailableDate = new Date();
    let minDate = new Date();

    const vm = new Vue({
        el: '#scheduler',
        data: {
            isSaving: false,
            hasWarnings: false,
            error: null,
            currentTime: '',
            reasons: [],
            mwModel: {
                starttime: null,
                endtime: null,
                reason: -1,
                comments: '',
            }
        },
        mounted: function () {
            let vm = this;
            vm.getReasons();

            setInterval(function() { vm.updateTime() }, 1000);
            vm.updateTime();

            //Set minimums to 48 hrs ahead.
            firstAvailableDate.setDate(firstAvailableDate.getDate() + 2);
            minDate.setDate(minDate.getDate() + 2);

            var datetimeFormat = 'm/d/Y H:i';

            $('#startTimePicker').datetimepicker({
                format: datetimeFormat,
                minDate: firstAvailableDate,
                startDate: firstAvailableDate,
                onClose: function () {
                    vm.datePickerClosed(this, 'start');
                }
            });
            $('#endTimePicker').datetimepicker({
                format: datetimeFormat,
                minDate: firstAvailableDate,
                startDate: firstAvailableDate,
                onShow: function () {
                    vm.setMinimumFromStartDate(this);
                },
                onClose: function () {
                    vm.datePickerClosed(this, 'end');
                }
            });
        },
        methods: {
            datePickerClosed: function (picker, type) {
                if (type === 'start') {
                    vm.mwModel.starttime = $('#startTimePicker').val();
                    picker.setOptions({ minDate: minDate });
                }
                else if (type === 'end') {
                    vm.mwModel.endtime = $('#endTimePicker').val();
                    var min = vm.mwModel.starttime != null ? firstAvailableDate : minDate;
                    picker.setOptions({ minDate: min });
                }

                vm.checkDates();
            },
            setMinimumFromStartDate: function (picker) {
                if (vm.mwModel.starttime != null) {
                    picker.setOptions({ minDate: firstAvailableDate });
                }
                else {
                    picker.setOptions({ minDate: minDate });
                }
            },
            createMaintenanceWindow: function () {
                if (!vm.isValidForm()) {
                    vm.hasWarnings = true;
                    return;
                }

                vm.hasWarnings = false;
                vm.error = '';

                var csrfToken = $("input[name='__RequestVerificationToken']").val(); 

                $.ajax({
                    type: "POST",
                    url: api + 'CreateMaintenanceWindow',
                    data: vm.mwModel,
                    headers: { "X-CSRF-Header": csrfToken },
                })
                .always(function (result) {
                    if (result.Valid) {
                        window.location.href = '/Relativity/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/MaintenanceWindows.aspx';
                    }
                    else {
                        vm.hasWarnings = true;
                        vm.error = result.Errors.join('<br/>');
                    }
                })
                .fail(function (error) {
                    console.log("Failed to creat maintenance window. Message: " + error.Message);
                });

            },
            isValidForm: function () {
                var valid = true;
                if (vm.mwModel.starttime === null || vm.mwModel.starttime === '') {
                    vm.error = "You must enter a value for the Start Period";
                    valid = false;
                }
                else if (vm.mwModel.endtime === null || vm.mwModel.endtime === '') {
                    vm.error = "You must enter a value for the End Period";
                    valid = false;
                }
                else if (vm.mwModel.reason < 0) {
                    vm.error = "You must select a valid reason";
                    valid = false;
                }
                else if (firstAvailableDate < minDate) {
                    vm.error = "Maintenance window must be created at least 48 hours in advance";
                    valid = false;
                }
                return valid;
            },
            getReasons: function () {
                $.get(api + 'GetMaintenanceWindowReasons', function (result) {
                    vm.reasons = result;
                })
                    .fail(function () {
                        console.log('Failed to get reasons')
                    })
            },
            checkDates: function () {
                if ((vm.mwModel.starttime > vm.mwModel.endtime) && vm.mwModel.endtime != null) {
                    vm.hasWarnings = true;
                    vm.error = "Start period cannot be after end period";
                }
                else {
                    vm.hasWarnings = false;
                    vm.error = '';
                }
            },
            updateTime: function () {
                this.currentTime = new Date().toUTCString().replace(/GMT/, "UTC");
            }
        }
    });
    $(".vue-wait").removeClass("vue-wait");
}());