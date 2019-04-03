(function () {
    const $ = this.$;
    const api = '/relativity/custompages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/api/configuration/';

    const vm = new Vue({
        el: '#notifications',
        data: {
            configuration: {
                NotificationSettings: {
                    WeeklyScoreAlert: {},
                    SystemLoadForecast: {},
                    UserExperienceForecast: {},
                    QuarterlyScoreAlert: {},
                    QuarterlyScoreStatus: {},
                    BackupDBCCAlert: {},
                    ConfigurationChangeAlert: {}
                }
            },
            isSuccess: '',
            isError: '',
        },
        mounted: function () {
            let vm = this;
            vm.getSettings();

            $('.checkbox').bootstrapSwitch({
                onSwitchChange: function (event, state) {
                    vm.updateEnabled(event.currentTarget, state);
                }
            });
        },
        methods: {
            getSettings: function () {
                $.get(api + 'GetSettings', function(res) {
                    vm.configuration = res;

                    $('#WeeklyScoreAlert').bootstrapSwitch('state', vm.configuration.NotificationSettings.WeeklyScoreAlert.Enabled);
                    $('#SystemLoadForecast').bootstrapSwitch('state', vm.configuration.NotificationSettings.SystemLoadForecast.Enabled);
                    $('#UserExperienceForecast').bootstrapSwitch('state', vm.configuration.NotificationSettings.UserExperienceForecast.Enabled);
                    $('#QuarterlyScoreAlert').bootstrapSwitch('state', vm.configuration.NotificationSettings.QuarterlyScoreAlert.Enabled);
                    $('#QuarterlyScoreStatus').bootstrapSwitch('state', vm.configuration.NotificationSettings.QuarterlyScoreStatus.Enabled);
                    $('#BackupDBCCAlert').bootstrapSwitch('state', vm.configuration.NotificationSettings.BackupDBCCAlert.Enabled);
                    $('#ConfigurationChangeAlert').bootstrapSwitch('state', vm.configuration.NotificationSettings.ConfigurationChangeAlert.Enabled);

                })
                    .fail(function (err) {
                        vm.isError = true;
                        vm.status = err.Message;

                    });
                  
            },
            saveSettings: function () {
                vm.clearMessages();
                $.post(api + 'SaveSettings', JSON.stringify(vm.configuration), null, 'application/json')
                    .always(function(res) {
                        var resp = JSON.parse(res.responseText);
                        if (resp.Valid) {
                            vm.isSuccess = 'Your notification settings have been saved!';
                            setTimeout(function() { vm.clearMessages() }, 3500);
                        }
                        else {
                            vm.isError = resp.Details;
                        }
                    });
            },
            clearMessages: function () {
                vm.isError = false;
                vm.isSuccess = false;
            },
            updateEnabled: function (target, state) {
                vm.configuration.NotificationSettings[target.id].Enabled = state;
            }
        }
    });
    $(".vue-wait").removeClass("vue-wait");
}());