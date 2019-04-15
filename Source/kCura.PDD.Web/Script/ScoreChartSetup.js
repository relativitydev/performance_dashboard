function setupScoreCharts(dataSource, onDataPointClick) {
    if (window.scChartInternal)
        window.scChartInternal.destroy();
    var canvas = $(".scoreChart")[0];
    $.ajax({
        dataType: 'json',
        url: dataSource,
        success: function (json) {
            var ctx = canvas.getContext("2d");
            var data = {
                labels: json.Labels,
                datasets: json.DataSets
            };
            if (data.labels.length < 1) {
                ctx.font = "14px 'Open Sans'";
                ctx.textAlign = "center";
                ctx.textBaseline = "middle";
                ctx.fillText("No ratings available with current filters", ctx.canvas.clientWidth / 2, ctx.canvas.clientHeight / 2);
                return;
            }

            var chart = new Chart(ctx).Line(data, {
                animation: data.labels.length < 6,
                responsive: true,
                maintainAspectRatio: false,
                bezierCurve: false,
                pointHitDetectionRadius: 2,
                datasetFill: false,
                showScale: true,
                pointDot: false,
                pointDotRadius: 2,
                scaleOverride: true,
                scaleIntegersOnly: true,
                scaleSteps: json.ScaleSteps,
                scaleStepWidth: json.ScaleStepWidth,
                scaleStartValue: json.MinDisplay,
                skipXLabels: 24,
                tooltipFontSize: 12,
                legendTemplate: "<div class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><div class=\"legend-item\"><div style=\"background-color:<%=datasets[i].strokeColor%>;\"></div><%if(datasets[i].label){%><%=datasets[i].label%><%}%></div><%}%></div>"
            });
            $(".legend").html(chart.generateLegend());
            canvas.onclick = function (evt) {
                var activePoints = chart.getPointsAtEvent(evt);
                if (activePoints.length) {
                    var hour = activePoints[0].label.split(' ')[0];
                    onDataPointClick(hour);
                }
            };
            window.scChartInternal = chart;
            shiftGridPosition();
        },
        error: function (e) {
            //alert('Uh oh!');
        }
    });
}