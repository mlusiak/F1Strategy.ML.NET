using System.Collections.Generic;

namespace Tyres.DomainModels
{
    public class Track
    {
        public string Name { get; set; }
        public float Distance { get; set; }
    }

    public class Race
    {
        public Track Track { get; set; }
        public List<Driver> Drivers { get; set; }
        public List<string> TyreCompounds { get; set; }
        public float AirTemperature { get; set; }
        public float TrackTemperature { get; set; }
    }
}