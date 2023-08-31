using System;
using System.Collections.Generic;
using System.Text;
using Tyres.DomainModels;

namespace Tyres.StaticData
{
    public static class Season2023
    {
        public static List<Driver> Drivers = new List<Driver>()
        {
            new Driver() {Team = "Mercedes", Name = "Lewis Hamilton"},
            new Driver() {Team = "Mercedes", Name = "George Russell"},
            new Driver() {Team = "Red Bull", Name = "Max Verstappen"},
            new Driver() {Team = "Red Bull", Name = "Sergio Pérez"},
            new Driver() {Team = "McLaren", Name = "Lando Norris"},
            new Driver() {Team = "McLaren", Name = "Oscar Piastri"},
            new Driver() {Team = "Silverstone", Name = "Lance Stroll"},
            new Driver() {Team = "Silverstone", Name = "Fernando Alonso"},
            new Driver() {Team = "Enstone", Name = "Pierre Gasly"},
            new Driver() {Team = "Enstone", Name = "Esteban Ocon"},
            new Driver() {Team = "Ferrari", Name = "Charles Leclerc"},
            new Driver() {Team = "Ferrari", Name = "Carlos Sainz"},
            new Driver() {Team = "Faenza", Name = "Liam Lawson"},
            new Driver() {Team = "Faenza", Name = "Yuki Tsunoda"},
            new Driver() {Team = "Sauber", Name = "Valtteri Bottas"},
            new Driver() {Team = "Sauber", Name = "Guanyu Zhou"},
            new Driver() {Team = "HAAS", Name = "Kevin Magnussen"},
            new Driver() {Team = "HAAS", Name = "Nicko Hülkenberg"},
            new Driver() {Team = "Williams", Name = "Alex Albon"},
            new Driver() {Team = "Williams", Name = "Logan Sargeant"},
        };

        public static Dictionary<string, Track> Tracks = new Dictionary<string, Track>()
        {
            { "Bahrain", new Track() {Name = "Bahrain International Circuit", TrackLength = 5412f} },
            { "Imola", new Track() {Name = "Imola", TrackLength = 4909f } },
            { "Portimão", new Track() {Name = "Portimão", TrackLength = 4653f } },
            { "Catalunya", new Track() {Name = "Catalunya", TrackLength = 4675f } },
            { "Monaco", new Track() {Name = "Monaco", TrackLength = 3337f } },
            { "Monza", new Track() {Name = "Monza", TrackLength = 5793f } }
        };
    }
}
