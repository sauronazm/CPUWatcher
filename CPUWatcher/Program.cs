using System;
using System.Diagnostics;
using System.IO;
using System.Configuration;

namespace CPUWatcher {
    class Program {

        const string DEFAULT_REPORT_FILE_NAME_TEMPLATE = "report.csv";
        const int DEFAULT_SLEEP_TIMEOUT = 10 * 1000;

        static void Main(string[] args) {

            string reportFileName = GetReportFileName();

            int sleepTimeout = GetSleepTimeout();

            float result;
            using (PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total")) {
                cpuCounter.NextValue();
                System.Threading.Thread.Sleep(sleepTimeout);
                using (StreamWriter reportFile = new StreamWriter(reportFileName, true)) {
                    reportFile.AutoFlush = true;
                    while (true) {
                        result = cpuCounter.NextValue();
                        reportFile.Write(DateTime.Now.ToString("MM/dd/yyyy HH:mm") + ", " + result.ToString("##.##") + Environment.NewLine);
                        System.Threading.Thread.Sleep(sleepTimeout);
                    }
                }
            }
        }

        private static string GetReportFileName() {
            string reportFileName = ConfigurationManager.AppSettings["ReportFileNameTemplate"];
            if (string.IsNullOrWhiteSpace(reportFileName)) {
                reportFileName = DEFAULT_REPORT_FILE_NAME_TEMPLATE;
            }
            reportFileName = DateTime.Now.ToString("MM_dd_yyyy-") + reportFileName;
            return reportFileName;
        }

        private static int GetSleepTimeout() {
            string sleepTimeoutString = ConfigurationManager.AppSettings["SleepTimeout"];
            int sleepTimeout = 0;
            if (!string.IsNullOrWhiteSpace(sleepTimeoutString)) {
                var isNum = int.TryParse(sleepTimeoutString, out sleepTimeout);
                if (!isNum) {
                    sleepTimeout = DEFAULT_SLEEP_TIMEOUT;
                }
            }
            return sleepTimeout;
        }
    }
}
