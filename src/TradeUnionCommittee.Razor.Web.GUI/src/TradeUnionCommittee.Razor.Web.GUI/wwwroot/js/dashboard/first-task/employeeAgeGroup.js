﻿// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#292b2c';

// Task 1.4

$('#myBarChart-1-4').ready(function () {

    $('#bar-time-1-4').empty();
    $('#bar-container-1-4').empty();
    $('#bar-container-1-4').append('<div class="sbl-circ-dual"></div>');

    $.get("/Dashboard/EmployeeAgeGroup", function (result) {

        $('#bar-container-1-4').empty();
        $('#bar-container-1-4').append('<canvas id="myBarChart-1-4" width="100%" height="50"></canvas>');
        $('#bar-time-1-4').append(`Оновлено щойно - ${result.dateTime}`);

        var myLineChart = new Chart(document.getElementById("myBarChart-1-4"), {
            type: 'bar',
            data: {
                labels: result.chart.labels,
                datasets: [{
                    label: "Revenue",
                    backgroundColor: "rgba(2,117,216,1)",
                    borderColor: "rgba(2,117,216,1)",
                    data: result.chart.data,
                }],
            },
            options: {
                scales: {
                    xAxes: [{
                        time: {
                            unit: 'month'
                        },
                        gridLines: {
                            display: false
                        },
                        ticks: {
                            maxTicksLimit: result.chart.data.length
                        }
                    }],
                    yAxes: [{
                        ticks: {
                            min: 0,
                            max: Math.max.apply(Math, result.chart.data) + 50,
                            maxTicksLimit: result.chart.data.length
                        },
                        gridLines: {
                            display: true
                        }
                    }],
                },
                legend: {
                    display: false
                }
            }
        });
    });
});
