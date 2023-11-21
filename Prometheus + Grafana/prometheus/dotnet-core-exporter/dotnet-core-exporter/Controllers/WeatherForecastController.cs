using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Prometheus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_core_exporter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly Counter count = Metrics.CreateCounter("weatherforecast_get_request_count", "Total number of requests");

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            count.Inc();
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("PushGateway")]
        public async Task<string> PushGateway()
        {
            string msg = "";
            try
            {
                string job = "job test";
                using (var st = new MemoryStream())
                {
                    await Metrics.DefaultRegistry.CollectAndExportAsTextAsync(st);

                    string metricsText = Encoding.UTF8.GetString(st.ToArray());

                    var myCounterText = GetMetricText(metricsText, "weatherforecast_get_request_count");

                    using (var httpClient = new HttpClient())
                    {
                        var pushUrl = $"http://pushgateway:9091/metrics/job/{job}/instance/dotnet-core-exporter";
                        // prometheus pushgateway 傳送的參數 最後要斷行
                        // 否則會回傳 text format parsing error in line 3: unexpected end of input stream 的錯誤
                        var content = new StringContent(myCounterText + "\n");

                        var response = httpClient.PostAsync(pushUrl, content).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("weatherforecast_get_request_count pushed successfully!");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to push weatherforecast_get_request_count. Status code: {response.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }

            return string.IsNullOrEmpty(msg) ? "Success" : msg;
        }

        private static string GetMetricText(string allMetricsText, string metricName)
        {
            List<string> metricsList = allMetricsText.Split('\n').ToList();

            return string.Join('\n', metricsList.Where(x => x.Contains(metricName)));
        }
    }
}