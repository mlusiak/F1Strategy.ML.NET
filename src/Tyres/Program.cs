using System;
using System.Collections.Generic;
using System.Collections;
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
        private static string DatasetsLocation = @"../../../../../data/stints.csv";

        static void Main(string[] args)
        {
            //Environment.SetEnvironmentVariable("AutoMLServiceRegressors", "LGBM;RF;FASTTREE;LBFGS;SDCA");

            var mlContext = new MLContext(seed: 0);

            // Load data
            var data = mlContext.Data.LoadFromTextFile<TyreStint>(DatasetsLocation, ';', true);

            // Divide dataset into training and testing data
            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.1);
            var trainingData = split.TrainSet;
            var testingData = split.TestSet;


            // Run AutoML experiment
            var experimentTime = 60u;
            Console.WriteLine("=============== Training the model ===============");
            Console.WriteLine($"Running AutoML regression experiment for {experimentTime} seconds...");
            var experimentResult = mlContext.Auto()
                .CreateRegressionExperiment(experimentTime)
                .Execute(trainingData, testingData,
                    columnInformation: new ColumnInformation()
                    {
                        CategoricalColumnNames = { nameof(TyreStint.Team), nameof(TyreStint.Driver), nameof(TyreStint.Compound) },
                        NumericColumnNames = { nameof(TyreStint.Season), nameof(TyreStint.AirTemperature), nameof(TyreStint.TrackTemperature) },
                        LabelColumnName = nameof(TyreStint.StintLength)
                    }
                );

            // Print top models found by AutoML
            TrainingHelper.PrintTopModels(experimentResult);

            Console.WriteLine("===== Evaluating model's accuracy with test data =====");
            var best = experimentResult.BestRun;

            var trainedModel = best.Model;
            var predictions = trainedModel.Transform(testingData);

            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: nameof(TyreStint.StintLength), scoreColumnName: "Score");
            TrainingHelper.PrintRegressionMetrics(best.TrainerName, metrics);


            // Run sample predictions
            var predictionEngine = mlContext.Model.CreatePredictionEngine<TyreStint, TyreStintPrediction>(trainedModel);

            var lh = new TyreStint() { Track = "Bahrain International Circuit", Team = "Mercedes", Driver = "Lewis Hamilton", Compound = "C3", AirTemperature = 20.5f, TrackTemperature = 28.3f };
            var lhPred = predictionEngine.Predict(lh);
            var lhLaps = lhPred.StintLength;

            var mv = new TyreStint() { Track = "Bahrain International Circuit", Team = "Red Bull", Driver = "Max Verstappen", Compound = "C3", AirTemperature = 20.5f, TrackTemperature = 28.3f };
            var mvPred = predictionEngine.Predict(mv);
            var mvLaps = mvPred.StintLength;


            // Printing predictions for top10 grid places
            var top10Monza = new List<Top10Driver>()
            {
                new Top10Driver() {Team = "Ferrari", Name = "Charles Leclerc", StartingCompound = "C5"},
                new Top10Driver() {Team = "Red Bull", Name = "Max Verstappen", StartingCompound = "C5"},
                new Top10Driver() {Team = "Mercedes", Name = "Valtteri Bottas", StartingCompound = "C5"},
                new Top10Driver() {Team = "Ferrari", Name = "Carlos Sainz", StartingCompound = "C5"},
                new Top10Driver() {Team = "McLaren", Name = "Lando Norris", StartingCompound = "C5"},
                new Top10Driver() {Team = "Faenza", Name = "Pierre Gasly", StartingCompound = "C5"},
                new Top10Driver() {Team = "Mercedes", Name = "Lewis Hamilton", StartingCompound = "C5"},
                new Top10Driver() {Team = "Silverstone", Name = "Sebastian Vettel", StartingCompound = "C5"},
                new Top10Driver() {Team = "Red Bull", Name = "Sergio Pérez", StartingCompound = "C5"},
                new Top10Driver() {Team = "Sauber", Name = "Antonio Giovinazzi", StartingCompound = "C5"},
                new Top10Driver() {Team = "Enstone", Name = "Esteban Ocon", StartingCompound = "C5"},
            };

            DataHelper.PrintPredictionTable(predictionEngine, "Monza", 26.0f, 45.0f, top10Monza);
        }
    }
}



//TODO out of bound exception if moving to a 2.0 api can be related to wrong path to data