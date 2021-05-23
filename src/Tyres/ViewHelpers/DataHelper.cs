using System;
using System.Collections.Generic;
using Microsoft.ML;
using Tyres.DataModels;
using Tyres.DomainModels;
using Tyres.StaticData;

namespace Tyres.ViewHelpers
{
    public class DataHelper
    {
        public static void PrintPredictionTable(PredictionEngine<TyreStint, TyreStintPrediction> predictionEngine, string trackName, float airTemperature, float trackTemperature, List<Top10Driver> drivers)
        {
            var monaco = Season2021.Tracks[trackName];

            Console.WriteLine(monaco.Name);
            Console.WriteLine("==========");
            foreach (var d in drivers)
            {
                var prediction = predictionEngine.Predict(new TyreStint()
                {
                    Track = monaco.Name,
                    TrackLength = monaco.TrackLength,
                    Team = d.Team,
                    Car = d.Car,
                    Driver = d.Name,
                    Compound = d.StartingCompound,
                    AirTemperature = airTemperature,
                    TrackTemperature = trackTemperature,
                    Reason = "Pit Stop"
                });
                Console.WriteLine($"| {d.Name} | {d.StartingCompound} | {prediction.Distance / monaco.TrackLength } |  |  |");
            }
            Console.WriteLine("");
        }
    }
}