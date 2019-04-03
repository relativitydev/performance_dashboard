(function () {
	const $ = this.$;
	const api = '/relativity/custompages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/api/backfill/';
	const vm = new Vue({
		el: '#backfill',
		data: {
			status: {},
			loadingStatus: false,
			hasError: false,
			error: ''
		},
		mounted: function () {
			let vm = this;
			vm.getBackfillStatus();
			vm.checkNotificationStatus();
		},
		methods: {
			getBackfillStatus: function () {
				$.get(api + 'GetBackfillStatus',
					function (res) {
						vm.status = res;
                        var dt = Date.parse(res.LastEventExecuted);
                        if (isNaN(dt)) {
                            vm.status.LastEventExecuted = 'N/A'
                        } else {
                            vm.status.LastEventExecuted = new Date(dt).toLocaleString('en-US');
                        }						
					})
					.fail(function (err) {
						vm.hasError = true;
						vm.error = err.Message;
					});
			},
			checkNotificationStatus: function () {
				$.get(api + 'GetNotificationStatus',
					function (res) {
						if (res != null) {
							vm.hasError = true;
							if (res.Type == 0) {
								//Critical error
								vm.error = res.Message;
							} else {
								vm.error =
									"One or more EDDSQoS databases may have failed to be deployed correctly. The application may not run until the databases are deployed. You can retry deployment in the Backfill Console."
							}
						}
					});

			},
			retryErrorEvents: function () {
				vm.loadingStatus = true;
				$.post(api + 'RetryErrorEvents', null, function (res) {
					setTimeout(function () {
						vm.loadingStatus = false;
					}, 3000);
					vm.status = res;
					var dt = new Date(res.LastEventExecuted);
					vm.status.LastEventExecuted = dt.toLocaleString('en-US');
				});
			},
			downloadErrorLogs: function () {
				$("<form>")
					.attr("method", "POST")
					.attr("action", api + 'Log?logLevel=Errors')
					.appendTo("body")
					.submit()
					.remove();
			},
			downloadFullLogs: function () {
				$("<form>")
					.attr("method", "POST")
					.attr("action", api + 'Log?logLevel=Verbose')
					.appendTo("body")
					.submit()
					.remove();
			}
		}
	});
	$(".vue-wait").removeClass("vue-wait");
}());