using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using Tyres.DataModels;
using Tyres.DomainModels;
using Tyres.StaticData;

namespace Tyres
{
    public class Program
    {
        private static string DatasetsLocation = @"../../../../../data/Tyres.csv";

        static void Main(string[] args)
        {
            var mlContext = new MLContext(seed: 0);

            // Load data
            var data = mlContext.Data.LoadFromTextFile<TyreStint>(DatasetsLocation, ';', true);

            // Filtering data
            var filtered = mlContext.Data.FilterByCustomPredicate(data, (TyreStint row) => !(row.Reason.Equals("Pit Stop") || row.Reason.Equals("Race Finish")) );
            var debug = mlContext.Data.CreateEnumerable<TyreStint>(filtered, reuseRowObject: false).Count();

            // Divide dataset into training and testing data
            var split = mlContext.Data.TrainTestSplit(filtered, testFraction: 0.1);
            var trainingData = split.TrainSet;
            var testingData = split.TestSet;

            // Build data pipeline
            var pipeline = mlContext.Transforms.CustomMapping((TyreStint input, CustomDistanceMapping output) => output.Distance = input.Laps * input.TrackLength, contractName: null)
                .Append(mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(TransformedTyreStint.Distance)))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "TeamEncoded", inputColumnName: nameof(TyreStint.Team)))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "CarEncoded", inputColumnName: nameof(TyreStint.Car)))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "DriverEncoded", inputColumnName: nameof(TyreStint.Driver)))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "CompoundEncoded", inputColumnName: nameof(TyreStint.Compound)))
                .Append(mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TyreStint.AirTemperature)))
                .Append(mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(TyreStint.TrackTemperature)))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "ReasonEncoded", inputColumnName: nameof(TyreStint.Reason)))
                .Append(mlContext.Transforms.Concatenate("Features", 
                    "TeamEncoded", "CarEncoded", "DriverEncoded", "CompoundEncoded", nameof(TyreStint.AirTemperature), nameof(TyreStint.TrackTemperature)));

            // Setting the training algorithm
            var trainer = mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features");
            var trainingPipeline = pipeline.Append(trainer);

            // Training the model
            Console.WriteLine("=============== Training the model ===============");
            var trainedModel = trainingPipeline.Fit(trainingData);

            // Evaluate the model on test data
            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====");

            var predictions = trainedModel.Transform(testingData);
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Label", scoreColumnName: "Score");

            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Metrics for {trainer.ToString()} regression model      ");
            Console.WriteLine($"*------------------------------------------------");
            Console.WriteLine($"*       LossFn:        {metrics.LossFunction:0.##}");
            Console.WriteLine($"*       R2 Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"*       Absolute loss: {metrics.MeanAbsoluteError:#.##}");
            Console.WriteLine($"*       Squared loss:  {metrics.MeanSquaredError:#.##}");
            Console.WriteLine($"*       RMS loss:      {metrics.RootMeanSquaredError:#.##}");
            Console.WriteLine($"*************************************************");


            // Run sample predictions
            var predictionEngine = mlContext.Model.CreatePredictionEngine<TyreStint, TyreStintPrediction>(trainedModel);

            var lh = new TyreStint() { Track = "Bahrain International Circuit", TrackLength = 5412f, Team = "Mercedes", Car = "W12", Driver = "Lewis Hamilton", Compound = "C3", AirTemperature = 20.5f, TrackTemperature = 28.3f, Reason = "Pit Stop" };
            var lhPred = predictionEngine.Predict(lh);
            var lhLaps = lhPred.Distance / 5412f;

            var mv = new TyreStint() { Track = "Bahrain International Circuit", TrackLength = 5412f, Team = "Red Bull", Car = "RB16B", Driver = "Max Verstappen", Compound = "C3", AirTemperature = 20.5f, TrackTemperature = 28.3f, Reason = "Pit Stop" };
            var mvPred = predictionEngine.Predict(mv);
            var mvLaps = mvPred.Distance / 5412f;


            // Run predictions for Bahrain
            var bahrain2021 = new Race()
            {
                Track = Season2021.Tracks["Bahrain"],
                Drivers = Season2021.Drivers,
                TyreCompounds = new List<string>() { "C2", "C3", "C4" },
                AirTemperature = 20.5f,
                TrackTemperature = 28.3f
            };
            PrintPredictions(predictionEngine, bahrain2021);
        }

        private static void PrintPredictions(PredictionEngine<TyreStint, TyreStintPrediction> predictionEngine, Race race)
        {
            foreach (var d in race.Drivers)
            {
                foreach (var c in race.TyreCompounds)
                {
                    var prediction = predictionEngine.Predict(new TyreStint()
                    {
                        Track = race.Track.Name,
                        TrackLength = race.Track.Distance,
                        Team = d.Team,
                        Car = d.Car,
                        Driver = d.Name,
                        Compound = c,
                        AirTemperature = race.AirTemperature,
                        TrackTemperature = race.TrackTemperature,
                        Reason = "Pit Stop"
                    });
                    Console.WriteLine($"| {d.Name} | {c} | {prediction.Distance / race.Track.Distance} |  |");
                }
            }

        }
    }
}
