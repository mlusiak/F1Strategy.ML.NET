using System;
using System.Linq;
using Microsoft.ML.AutoML;
using Microsoft.ML.Data;

namespace Tyres.ViewHelpers
{
    public class TrainingHelper
    {
        public static void PrintTopModels(ExperimentResult<RegressionMetrics> experimentResult)
        {
            // Get top few runs ranked by R-Squared.
            // R-Squared is a metric to maximize, so OrderByDescending() is correct.
            // For RMSE and other regression metrics, OrderByAscending() is correct.
            var topRuns = experimentResult.RunDetails
                .Where(r => r.ValidationMetrics != null && !double.IsNaN(r.ValidationMetrics.RSquared))
                .OrderByDescending(r => r.ValidationMetrics.RSquared).Take(3);

            Console.WriteLine("Top models ranked by R-Squared --");
            PrintRegressionMetricsHeader();
            for (var i = 0; i < topRuns.Count(); i++)
            {
                var run = topRuns.ElementAt(i);
                PrintIterationMetrics(i + 1, run.TrainerName, run.ValidationMetrics, run.RuntimeInSeconds);
            }
        }

        public static void PrintRegressionMetrics(string name, RegressionMetrics metrics)
        {
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Metrics for {name} regression model      ");
            Console.WriteLine($"*------------------------------------------------");
            Console.WriteLine($"*       LossFn:        {metrics.LossFunction:0.##}");
            Console.WriteLine($"*       R2 Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"*       Absolute loss: {metrics.MeanAbsoluteError:#.##}");
            Console.WriteLine($"*       Squared loss:  {metrics.MeanSquaredError:#.##}");
            Console.WriteLine($"*       RMS loss:      {metrics.RootMeanSquaredError:#.##}");
            Console.WriteLine($"*************************************************");
        }

        internal static void PrintRegressionMetricsHeader()
        {
            CreateRow($"{"",-4} {"Trainer",-35} {"RSquared",8} {"Absolute-loss",13} {"Squared-loss",12} {"RMS-loss",8} {"Duration",9}", 114);
        }

        internal static void PrintIterationMetrics(int iteration, string trainerName, RegressionMetrics metrics, double? runtimeInSeconds)
        {
            CreateRow($"{iteration,-4} {trainerName,-35} {metrics?.RSquared ?? double.NaN,8:F4} {metrics?.MeanAbsoluteError ?? double.NaN,13:F2} {metrics?.MeanSquaredError ?? double.NaN,12:F2} {metrics?.RootMeanSquaredError ?? double.NaN,8:F2} {runtimeInSeconds.Value,9:F1}", 114);
        }

        private static void CreateRow(string message, int width)
        {
            Console.WriteLine("|" + message.PadRight(width - 2) + "|");
        }
    }
}
