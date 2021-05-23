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

    public class DistanceTyreStint : TyreStint
    {
        public float Distance { get; set; }
    }

    public class TyreStintPrediction
    {
        [ColumnName("Score")]
        public float Distance;
    }

    public class LapsToDistanceInput
    {
        public float TrackLength { get; set; }
        public float Laps { get; set; }
    }

    public class LapsToDistanceOutput
    {
        public float Distance { get; set; }
    }

    [CustomMappingFactoryAttribute(nameof(CustomMappings.DistanceMapping))]
    public class CustomMappings : CustomMappingFactory<LapsToDistanceInput, LapsToDistanceOutput>
    {
        public static void DistanceMapping(LapsToDistanceInput input, LapsToDistanceOutput output) => output.Distance = input.Laps * input.TrackLength;

        public override Action<LapsToDistanceInput, LapsToDistanceOutput> GetMapping()
        {
            return DistanceMapping;
        }
    }
}
