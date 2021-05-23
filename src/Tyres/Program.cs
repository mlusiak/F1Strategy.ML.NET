using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.AutoML;
using Tyres.DataModels;
using Tyres.DomainModels;
using Tyres.StaticData;
using Tyres.ViewHelpers;

namespace Tyres
{
    public class Program
    {
        private static string DatasetsLocation = @"../../../../../data/TyreStints.csv";

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


            /*
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
            PrintRegressionMetrics(trainer, metrics);

            */

            var experimentTime = 600u;
            Console.WriteLine("=============== Training the model ===============");
            Console.WriteLine($"Running AutoML regression experiment for {experimentTime} seconds...");
            var experimentResult = mlContext.Auto()
                .CreateRegressionExperiment(experimentTime)
                .Execute(trainingData, progressHandler: null, labelColumnName: "Laps");

            // Print top models found by AutoML
            Console.WriteLine();
            TrainingHelper.PrintTopModels(experimentResult);

            Console.WriteLine("===== Evaluating model's accuracy with test data =====");
            var best = experimentResult.BestRun;

            var trainedModel = best.Model;
            var predictions = trainedModel.Transform(testingData);

            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Laps", scoreColumnName: "Score");
            TrainingHelper.PrintRegressionMetrics(best.TrainerName, metrics);


            // Run sample predictions
            var predictionEngine = mlContext.Model.CreatePredictionEngine<TyreStint, TyreStintPrediction>(trainedModel);

            var lh = new TyreStint() { Track = "Bahrain International Circuit", TrackLength = 5412f, Team = "Mercedes", Car = "W12", Driver = "Lewis Hamilton", Compound = "C3", AirTemperature = 20.5f, TrackTemperature = 28.3f, Reason = "Pit Stop" };
            var lhPred = predictionEngine.Predict(lh);
            var lhLaps = lhPred.Distance / 5412f;

            var mv = new TyreStint() { Track = "Bahrain International Circuit", TrackLength = 5412f, Team = "Red Bull", Car = "RB16B", Driver = "Max Verstappen", Compound = "C3", AirTemperature = 20.5f, TrackTemperature = 28.3f, Reason = "Pit Stop" };
            var mvPred = predictionEngine.Predict(mv);
            var mvLaps = mvPred.Distance / 5412f;



            var top10Monaco = new List<Top10Driver>()
            {
                new Top10Driver() {Team = "Ferrari", Car = "SF21", Name = "Charles Leclerc", StartingCompound = "C5"},
                new Top10Driver() {Team = "Red Bull", Car = "RB16B", Name = "Max Verstappen", StartingCompound = "C5"},
                new Top10Driver() {Team = "Mercedes", Car = "W12", Name = "Valtteri Bottas", StartingCompound = "C5"},
                new Top10Driver() {Team = "Ferrari", Car = "SF21", Name = "Carlos Sainz", StartingCompound = "C5"},
                new Top10Driver() {Team = "McLaren", Car = "MCL35M", Name = "Lando Norris", StartingCompound = "C5"},
                new Top10Driver() {Team = "Toro Rosso / AlphaTauri", Car = "AT02", Name = "Pierre Gasly", StartingCompound = "C5"},
                new Top10Driver() {Team = "Mercedes", Car = "W12", Name = "Lewis Hamilton", StartingCompound = "C5"},
                new Top10Driver() {Team = "Force India / Racing Point / Aston Martin", Car = "AMR21", Name = "Sebastian Vettel", StartingCompound = "C5"},
                new Top10Driver() {Team = "Red Bull", Car = "RB16B", Name = "Sergio Pérez", StartingCompound = "C5"},
                new Top10Driver() {Team = "Sauber / Alfa Romeo", Car = "C41", Name = "Antonio Giovinazzi", StartingCompound = "C5"},
                new Top10Driver() {Team = "Renault / Alpine", Car = "A521", Name = "Esteban Ocon", StartingCompound = "C5"},
            };

            DataHelper.PrintPredictionTable(predictionEngine, "Monaco", 25.0f, 43.7f, top10Monaco);
        }
    }
}
