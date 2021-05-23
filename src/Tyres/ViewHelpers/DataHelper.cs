﻿using System;
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
            Console.WriteLine(Season2021.Tracks[trackName].Name);
            Console.WriteLine("==========");
            foreach (var d in drivers)
            {
                var prediction = predictionEngine.Predict(new TyreStint()
                {
                    Track = Season2021.Tracks[trackName].Name,
                    TrackLength = Season2021.Tracks[trackName].Distance,
                    Team = d.Team,
                    Car = d.Car,
                    Driver = d.Name,
                    Compound = d.StartingCompound,
                    AirTemperature = airTemperature,
                    TrackTemperature = trackTemperature,
                    Reason = "Pit Stop"
                });
                Console.WriteLine($"| {d.Name} | {d.StartingCompound} | {prediction.Distance } |  |  |");
            }
            Console.WriteLine("");
        }
    }
}