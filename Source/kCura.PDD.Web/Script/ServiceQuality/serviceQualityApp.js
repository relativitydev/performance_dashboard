(function () {
	const $ = this.$;
	const api = '/relativity/custompages/60a1d0a3-2797-4fb3-a260-614cbfd3fa0d/api/servicequality/';
	const chartOptions = { cutoutPercentage: 75, responsive: true, legend: { display: false, }, tooltips: { enabled: false }, elements: { arc: { borderWidth: 0 } } };
	const qty = "Quarterly";
	const wky = "Weekly";

	// Chart.JS plugin to draw values in center of donut
	Chart.pluginService.register({
		beforeDraw: function (chart) {
			if (chart.config.centerText !== null &&
				typeof chart.config.centerText !== 'undefined' &&
				chart.config.centerText) {
				drawValue(chart);
			}

		}
	});

	function drawValue(chart) {
		var width = chart.chart.width,
			height = chart.chart.height,
			ctx = chart.chart.ctx;

		ctx.restore();
		var fontSize = (height / 88).toFixed(2);
		ctx.font = fontSize + "em sans-serif";
		ctx.textBaseline = "middle";
		ctx.fillStyle = chart.config.textColor;

		var text = chart.config.centerText,
			textX = Math.round((width - ctx.measureText(text).width) / 2),
			textY = height / 2.5;

		//var lowerXDivisor = 
		var lowerText = chart.config.lowerText,
			lowerTextX = Math.round((width - ctx.measureText(lowerText).width) / 2), //4 for Quarterly
			lowerTextY = height / 1.65;

		ctx.fillText(text, textX, textY);
		ctx.fillText(lowerText, lowerTextX, lowerTextY);
		ctx.save();
	}
	//End plugin
	const emptyColor = '#c1c1c0'
	let generatedDate = new Date();

	const vm = new Vue({
		el: '#qosApp',
		data: {
			report: {
				DateGenerated: null,
				PartnerName: 'your organization',
				InstanceName: 'this Relativity instance',
				SystemLoad: {
					Servers: []
				},
				UserExperience: {
					Servers: []
				},
				Backup: {
					MaxDataLossMinutes: 0,
					TimeToRecoverHours: 0,
					RTOScore: null,
					RPOScore: null,
					DBCCMonitoringEnabled: true,
					MissedBackups: '',
					BackupFrequencyScore: null,
					BackupCoverageScore: null,
					MissedIntegrityChecks: '',
					DbccFrequencyScore: null,
					DbccCoverageScore: null

				},
				Uptime: {
					DatesToReview: [],
					UptimePercentage: 0,
					WeeklyUptimePercentage: 0
				}
			},
			IsFraudDetected: false,
			IndicatorLevels: {
				PassScore: 90,
				WarnScore: 80
			},
			hasError: false,
			error: '',
			loading: true,
			hasWarning: false,
			hasAlert: false,
			notification: {
				Message: '',
				Type: ''
			}
		},
		mounted: function () {
			let vm = this;
			vm.getQualityStats();
		},
		methods: {
			getQualityStats: function () {
				let vm = this;
				$.get(api + 'GetIndicatorLevels',
					function (res) {
						vm.indicatorLevels = res;
						$.get(api + 'GetServiceQualityIndicators',
							function (res) {
								vm.report = res;
								vm.report.DateGenerated = generatedDate.toUTCString();
								vm.buildCharts();
								vm.loading = false;
							})
							.fail(function (err) {
								vm.loading = false;
								vm.hasError = true;
								vm.error = err.Message;
							});
					})
					.fail(function (err) {
						vm.loading = false;
						vm.hasError = true;
						vm.error = err.Message;
					});

				$.get(api + 'GetFraudDetection', function (res) {
					vm.IsFraudDetected = res;
				})
					.fail(function (err) {
						vm.hasError = true;
						vm.error = err.Message;
					});

				$.get(api + 'GetNotifications', function (res) {
					vm.notification = res;
					if (vm.notification.Type === 0) {
						vm.hasAlert = true;
					}
					else if (vm.notification.Type === 1) {
						vm.hasWarning = true;
					}
				})
					.fail(function (err) {
						vm.hasError = true;
						vm.error = err.Message;
					});

			},
			goToServer: function (serverName, pageName) {
				var url = '%25ApplicationPath%25%2fCustomPages%2f60a1d0a3-2797-4fb3-a260-614cbfd3fa0d%2' + pageName + '.aspx%3fStandardsCompliance%3dtrue&';
				window.parent.location = '/Relativity/External.aspx?AppID=-1&ArtifactID=-1&DirectTo=' + url;

			},
			buildCharts: function () {
				let vm = this;
				// Quarterly Overall
				var qOverallChartData = {
					label: 'qocd',
					datasets: [{
						data: [vm.report.OverallScore, (100 - vm.report.OverallScore)],
						backgroundColor: [vm.getBackgroundColor(vm.report.OverallScore), emptyColor]
					}]
				}
				vm.drawChart('qOverallChart', qOverallChartData, vm.report.OverallScore, qty, true);

				// Weekly Overall
				var wOverallChartData = {
					label: 'wocd',
					datasets: [{
						data: [vm.report.WeeklyScore, (100 - vm.report.WeeklyScore)],
						backgroundColor: [vm.getBackgroundColor(vm.report.WeeklyScore), emptyColor]
					}]
				}
				vm.drawChart('wOverallChart', wOverallChartData, vm.report.WeeklyScore, wky, true);

				// Quarterly UX
				var qUXChartData = {
					label: 'quxcd',
					datasets: [{
						data: [vm.report.UserExperience.QuarterlyScore, (100 - vm.report.UserExperience.QuarterlyScore)],
						backgroundColor: [vm.getBackgroundColor(vm.report.UserExperience.QuarterlyScore), emptyColor]
					}]
				}
				vm.drawChart('qUXChart', qUXChartData, vm.report.UserExperience.QuarterlyScore, qty);

				// Weekly UX
				var wUXChartData = {
					label: 'wuxcd',
					datasets: [{
						data: [vm.report.UserExperience.WeeklyScore, (100 - vm.report.UserExperience.WeeklyScore)],
						backgroundColor: [vm.getBackgroundColor(vm.report.UserExperience.WeeklyScore), emptyColor]
					}]
				}
				vm.drawChart('wUXChart', wUXChartData, vm.report.UserExperience.WeeklyScore, wky);

				// Quarterly Infrastructure
				var qInfraChartData = {
					label: 'qicd',
					datasets: [{
						data: [vm.report.SystemLoad.QuarterlyScore, (100 - vm.report.SystemLoad.QuarterlyScore)],
						backgroundColor: [vm.getBackgroundColor(vm.report.SystemLoad.QuarterlyScore), emptyColor]
					}]
				}
				vm.drawChart('qInfraChart', qInfraChartData, vm.report.SystemLoad.QuarterlyScore, qty);

				// Weekly Infrastructure
				var wInfraChartData = {
					label: 'wicd',
					datasets: [{
						data: [vm.report.SystemLoad.WeeklyScore, (100 - vm.report.SystemLoad.WeeklyScore)],
						backgroundColor: [vm.getBackgroundColor(vm.report.SystemLoad.WeeklyScore), emptyColor]
					}]
				}
				vm.drawChart('wInfraChart', wInfraChartData, vm.report.SystemLoad.WeeklyScore, wky);

				// Quarterly Recovery
				var qRecoveryChartData = {
					label: 'qrcd',
					datasets: [{
						data: [vm.report.Backup.Score, (100 - vm.report.Backup.Score)],
						backgroundColor: [vm.getBackgroundColor(vm.report.Backup.Score), emptyColor]
					}]
				}
				vm.drawChart('qRecoveryChart', qRecoveryChartData, vm.report.Backup.Score, qty);

				// Weekly Recovery
				var wRecoveryChartData = {
					label: 'wrcd',
					datasets: [{
						data: [vm.report.Backup.WeeklyScore, (100 - vm.report.Backup.WeeklyScore)],
						backgroundColor: [vm.getBackgroundColor(vm.report.Backup.WeeklyScore), emptyColor]
					}]
				}
				vm.drawChart('wRecoveryChart', wRecoveryChartData, vm.report.Backup.WeeklyScore, wky);

				// Quarterly Uptime
				var qUptimeChartData = {
					label: 'qucd',
					datasets: [{
						data: [vm.report.Uptime.Score, (100 - vm.report.Uptime.Score)],
						backgroundColor: [vm.getBackgroundColor(vm.report.Uptime.Score), emptyColor]
					}]
				}
				vm.drawChart('qUptimeChart', qUptimeChartData, vm.report.Uptime.Score, qty);

				// Weekly Uptime
				var wUptimeChartData = {
					label: 'wucd',
					datasets: [{
						data: [vm.report.Uptime.WeeklyScore, (100 - vm.report.Uptime.WeeklyScore)],
						backgroundColor: [vm.getBackgroundColor(vm.report.Uptime.WeeklyScore), emptyColor]
					}]
				}
				vm.drawChart('wUptimeChart', wUptimeChartData, vm.report.Uptime.WeeklyScore, wky);

			},
			drawChart: function (element, chartData, scoreText, lowerText, isHeader) {
				new Chart(element, {
					type: 'doughnut',
					data: chartData,
					options: chartOptions,
					centerText: scoreText + '%',
					lowerText: lowerText,
					textColor: isHeader ? '#ffffff' : '#000000'
				});
			},
			getBackgroundColor: function (score) {
				//See site.css fail/warn/passBackground classes
				//We can't set background class on canvas elements so we have to hard code the colors
				if (score >= vm.IndicatorLevels.PassScore) {
					return '#008000';
				}
				else if (score >= vm.IndicatorLevels.WarnScore) {
					return '#CACD00';
				}
				else {
					return '#ff0000';
				}
			},
			saveReport: function (reportType) {
				if (reportType === 'image') {
					html2canvas(document.getElementById('mainReport')).then(function (canvas) {
						var now = new Date();
						var fileName = 'QualityReport' + now.toLocaleDateString().replace(/\//g, '-') + '.png';
						if (window.navigator && window.navigator.msSaveOrOpenBlob) {
							//MSBS Edge
							window.navigator.msSaveOrOpenBlob(canvas.msToBlob(), fileName);
						}
						else {
							var a = document.createElement('a');
							var reportImage = canvas.toDataURL("image/png");
							a.href = reportImage.replace('image/png', 'image/octet-stream');
							a.download = fileName;
							document.body.appendChild(a);
							a.click();
							document.body.removeChild(a);
						}
					});
				}
				else if (reportType === 'pdf') {
					html2canvas(document.getElementById('mainReport')).then(function (canvas) {
						var reportImage = canvas.toDataURL("image/jpeg");
						var img = reportImage.replace(/^data:image\/(png|jpg);base64,/, "");
						var doc = new jsPDF('l', 'mm', [canvas.width, canvas.height]);
						//doc.addImage(img, 'JPEG', 15, 40, 180, 160);
						doc.addImage(img, 'JPEG', 5, 0, canvas.width, canvas.height);
						var now = new Date();
						var fileName = 'QualityReport' + now.toLocaleDateString().replace(/\//g, '-') + '.pdf';
						doc.save(fileName);
					});
				}
			},
			printReport: function () {
				html2canvas(document.getElementById('mainReport')).then(function (canvas) {
					var reportImage = canvas.toDataURL("image/png");
					var tWindow = window.open("");
					$(tWindow.document.body)
						.html("<img id='Image' src=" + reportImage + " style='width:100%;'></img>")
						.ready(function () {
							tWindow.focus();
							tWindow.print();
						});
					tWindow.onafterprint = function () {
						setTimeout(function () { tWindow.close(); }, 100);
					}
				});
			},
			goToUrl: function (serverUrl) {
				window.parent.location.href = serverUrl;
			}

		}
    });
    $(".vue-wait").removeClass("vue-wait");
}());