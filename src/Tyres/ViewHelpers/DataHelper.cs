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
            var track = Season2023.Tracks[trackName];

            Console.WriteLine(track.Name);
            Console.WriteLine("==========");
            foreach (var d in drivers)
            {
                var prediction = predictionEngine.Predict(new TyreStint()
                {
                    Season = 2023,
                    Track = track.Name,
                    Team = d.Team,
                    Driver = d.Name,
                    AirTemperature = airTemperature,
                    TrackTemperature = trackTemperature,
                    Compound = d.StartingCompound,
                });
                Console.WriteLine($"| {d.Name} | {d.StartingCompound} | {prediction.StintLength} |  |  |");
            }
            Console.WriteLine("");
        }
    }
}