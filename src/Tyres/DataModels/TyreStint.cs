using System;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;

namespace Tyres.DataModels
{
    public class TyreStint
    {
        [LoadColumn(1)]
        public string Track;
        [LoadColumn(3)]
        public float TrackLength;
        [LoadColumn(4)]
        public string Team;
        [LoadColumn(5)]
        public string Driver;
        [LoadColumn(6)]
        public string Compound;
        [LoadColumn(7)]
        public float AirTemperature;
        [LoadColumn(8)]
        public float TrackTemperature;
        [LoadColumn(9)]
        public float Distance;
    }

    public class TyreStintPrediction
    {
        [ColumnName("Score")]
        public float Distance;
    }
}
