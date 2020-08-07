using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.AvailabilityMonitoring;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.AvailabilityMonitoring;
using Microsoft.Extensions.Logging;

namespace serverlessmonitor
{
    public static class Function1
    {
        [FunctionName("SimpleBinding")]
        [return: AvailabilityTestResult]
        public static async Task<bool> Run([TimerTrigger(AvailabilityTestInterval.Minute01)] TimerInfo notUsed, ILogger log)
        {
            log.LogInformation($"**Availability func executed at: {DateTime.Now}");

            string responseContent;
            using (HttpClient http = AvailabilityTest.NewHttpClient())
            {
                using (HttpResponseMessage response = await http.GetAsync("https://availabilitymonitoring-extension-monitoredappsample.azurewebsites.net/Home/MonitoredPage"))
                {
                    HttpResponseMessage httpResponseMessage= response.EnsureSuccessStatusCode();

                    log.LogInformation($"**httpResponseMessage:" + httpResponseMessage.Content.ToString());

                    responseContent = await response.Content.ReadAsStringAsync();
                }
            }

            bool hasExpectedContent = responseContent.Contains("<title>Monitored Page</title>", StringComparison.OrdinalIgnoreCase)
                                        && responseContent.Contains("(App Version Id: 2)", StringComparison.OrdinalIgnoreCase);

            log.LogInformation($"**hasExpectedContent?" + hasExpectedContent.ToString());

            //AvailabilityTelemetry result = testInfo.DefaultAvailabilityResult;

            //result.Properties["UserProperty"] = "User Value";
            //result.Success = hasExpectedContent;
            //return result;

            return hasExpectedContent;
        }
    }
}