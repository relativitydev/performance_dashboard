(function () {
	const $ = this.$;
	const api = '/relativity/custompages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/api/configuration/';
	let vm = new Vue({
		el: '#configuration',
		data: {
			configuration: {
				LastModifiedBy: -1,
				BackupDbccSettings: {}
			},
			hasSyncErrors: false,
			hasError: false,
			configError: false,
			configSuccess: '',
			exportError: '',
			error: ''
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
				$.get(api + 'GetSettings', function (res) {
					vm.configuration = res;

					$('#enableDbccMonitoring').bootstrapSwitch('state', vm.configuration.BackupDbccSettings.EnableDbccMonitoring || vm.configuration.BackupDbccSettings.UseCommandBasedMonitoring);
					$('#useDbccViewMonitoring').bootstrapSwitch('state', vm.configuration.BackupDbccSettings.UseViewBasedMonitoring);
					$('#useDbccCommandMonitoring').bootstrapSwitch('state', vm.configuration.BackupDbccSettings.UseCommandBasedMonitoring);
					$('#showInvariantHistory').bootstrapSwitch('state', vm.configuration.BackupDbccSettings.ShowInvariantHistory);

				})
				.fail(function (err) {
					vm.hasError = true;
					vm.error = err.Message;
				});
			},
			saveSettings: function () {
				let vm = this;
				//Ignore read only properties before sending to save
				var refConfig = {
					LastModifiedBy: vm.configuration.LastModifiedBy,
					BackupDbccSettings: vm.configuration.BackupDbccSettings,
					NotificationSettings: vm.configuration.NotificationSettings
				};

				$.post(api + 'SaveSettings', JSON.stringify(refConfig), null, "application/json")
					.always(function (res) {
						var result = JSON.parse(res.responseText);
						if (result.Valid) {
							vm.configError = '';
							vm.showSuccessMessage("Settings have been saved!");
						} else {
							vm.configError = result.Details;
						}
					});

				//If the latest scripts aren't installed, send the user to install them
				$.get(api + 'ElevatedScriptsInstalled',
					function (res) {
						if (res === false) {
							window.location.href =
								"/Relativity/CustomPages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/AdministrationInstall.aspx";
						}
					});
			},
			updateEnabled: function (target, state) {
				let vm = this;
				switch (target.id) {
					case 'enableDbccMonitoring':
						vm.configuration.BackupDbccSettings.EnableDbccMonitoring = state;
						if (state === false) { //Set Command Based Monitoring off as well
							vm.configuration.BackupDbccSettings.UseCommandBasedMonitoring = state;
							vm.configuration.BackupDbccSettings.UseViewBasedMonitoring = state;
							$('#useDbccCommandMonitoring').bootstrapSwitch('state', state);
							$('#useDbccViewMonitoring').bootstrapSwitch('state', state);
						}
						break;
					case 'useDbccCommandMonitoring':
						vm.configuration.BackupDbccSettings.UseCommandBasedMonitoring = state;
						break;
					case 'showInvariantHistory':
						vm.configuration.BackupDbccSettings.ShowInvariantHistory = state;
						break;
					case 'useDbccViewMonitoring':
						vm.configuration.BackupDbccSettings.UseViewBasedMonitoring = state;
						break;
					default:
						break;
				}
			},
			showSuccessMessage: function (msg) {
				vm.configSuccess = msg;
				setTimeout(function () { vm.configSuccess = '' }, 3500);

			}
		}


	});
	$(".vue-wait").removeClass("vue-wait");
}());