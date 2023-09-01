using System;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;

namespace Tyres.DataModels
{
    public class TyreStint
    {
        [LoadColumn(1)]
        public float Season;

        [LoadColumn(2)]
        public string Track;

        [LoadColumn(3)]
        public string Team;

        [LoadColumn(4)]
        public string Driver;

        [LoadColumn(5)]
        public float AirTemperature;

        [LoadColumn(6)]
        public float TrackTemperature;

        [LoadColumn(9)]
        public string Compound;

        [LoadColumn(8)]
        public float StintLength;
    }

    public class TyreStintPrediction
    {
        [ColumnName("Score")]
        public float StintLength;
    }
}
