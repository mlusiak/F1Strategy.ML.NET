using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.ML;

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
            //mlContext.Data.CreateEnumerable<TyreStint>(data, reuseRowObject: false);

            // Divide dataset into training and testing data
            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.1);
            var trainingData = split.TrainSet;
            var testingData = split.TestSet;

            // Build data pipeline
            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(TyreStint.Laps))
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

            var lh1 = new TyreStint() { Track = "Bahrain International Circuit", Team = "Mercedes", Car = "W12", Driver = "Lewis Hamilton", Compound = "C3", AirTemperature = 20.5f, TrackTemperature = 28.3f, Reason = "Pit Stop" };
            var lh1_pred = predictionEngine.Predict(lh1);

            var mv1 = new TyreStint() { Track = "Bahrain International Circuit", Team = "Red Bull", Car = "RB16B", Driver = "Max Verstappen", Compound = "C3", AirTemperature = 20.5f, TrackTemperature = 28.3f, Reason = "Pit Stop" };
            var mv1_pred = predictionEngine.Predict(mv1);


            // Calculate predictions for all drivers
            var bahrainDrivers = new List<TeamDriverMatch>()
            {
                new TeamDriverMatch() {Team = "Mercedes", Car = "W12", Driver = "Lewis Hamilton"},
                new TeamDriverMatch() {Team = "Mercedes", Car = "W12", Driver = "Valtteri Bottas"},
                new TeamDriverMatch() {Team = "Red Bull", Car = "RB16B", Driver = "Max Verstappen"},
                new TeamDriverMatch() {Team = "Red Bull", Car = "RB16B", Driver = "Sergio Pérez"},
                new TeamDriverMatch() {Team = "McLaren", Car = "MCL35M", Driver = "Lando Norris"},
                new TeamDriverMatch() {Team = "McLaren", Car = "MCL35M", Driver = "Daniel Ricciardo"},
                new TeamDriverMatch() {Team = "Force India / Racing Point / Aston Martin", Car = "AMR21", Driver = "Lance Stroll"},
                new TeamDriverMatch() {Team = "Force India / Racing Point / Aston Martin", Car = "AMR21", Driver = "Sebastian Vettel"},
                new TeamDriverMatch() {Team = "Renault / Alpine", Car = "A521", Driver = "Fernando Alonso"},
                new TeamDriverMatch() {Team = "Renault / Alpine", Car = "A521", Driver = "Esteban Ocon"},
                new TeamDriverMatch() {Team = "Ferrari", Car = "SF21", Driver = "Charles Leclerc"},
                new TeamDriverMatch() {Team = "Ferrari", Car = "SF21", Driver = "Carlos Sainz"},
                new TeamDriverMatch() {Team = "Toro Rosso / AlphaTauri", Car = "AT02", Driver = "Pierre Gasly"},
                new TeamDriverMatch() {Team = "Toro Rosso / AlphaTauri", Car = "AT02", Driver = "Yuki Tsunoda"},
                new TeamDriverMatch() {Team = "Sauber / Alfa Romeo", Car = "C41", Driver = "Antonio Giovinazzi"},
                new TeamDriverMatch() {Team = "Sauber / Alfa Romeo", Car = "C41", Driver = "Kimi Räikkönen"},
                new TeamDriverMatch() {Team = "HAAS", Car = "VF21", Driver = "Nikita Mazepin"},
                new TeamDriverMatch() {Team = "HAAS", Car = "VF21", Driver = "Mick Schumacher"},
                new TeamDriverMatch() {Team = "Williams", Car = "FW43B", Driver = "George Russell"},
                new TeamDriverMatch() {Team = "Williams", Car = "FW43B", Driver = "Nicholas Latifi"},
            };
            var bahrainTyres = new List<string>() { "C2", "C3", "C4" };
            var airTemp = 20.5f;
            var trackTemp = 28.3f;
            var stopReason = "Pit Stop";
            var trackName = "Bahrain International Circuit"; 

            foreach (var d in bahrainDrivers)
            {
                foreach (var c in bahrainTyres)
                {
                    var prediction = predictionEngine.Predict(new TyreStint()
                    {
                        Track = trackName,
                        Team = d.Team, 
                        Car = d.Car, 
                        Driver = d.Driver, 
                        Compound = c, 
                        AirTemperature = airTemp, 
                        TrackTemperature = trackTemp, 
                        Reason = stopReason
                    });
                    Console.WriteLine($"{d.Driver} @ {c}: {prediction.Laps}");
                }
            }

            Console.WriteLine("======================");
        }
    }

    public class TeamDriverMatch
    {
        public string Team { get; set; }
        public string Car { get; set; }
        public string Driver { get; set; }
    }
}
