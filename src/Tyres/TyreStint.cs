using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML.Data;

namespace Tyres
{
    public class TyreStint
    {
        [LoadColumn(1)]
        public string Track;
        [LoadColumn(4)]
        public string Team;
        [LoadColumn(5)]
        public string Car;
        [LoadColumn(6)]
        public string Driver;
        [LoadColumn(7)]
        public string Compound;
        [LoadColumn(8)]
        public float AirTemperature;
        [LoadColumn(9)]
        public float TrackTemperature;
        [LoadColumn(10)]
        public string Reason;
        [LoadColumn(11)]
        public float Laps;
    }

    public class TyreStintPrediction
    {
        [ColumnName("Score")]
        public float Laps;
    }
}
