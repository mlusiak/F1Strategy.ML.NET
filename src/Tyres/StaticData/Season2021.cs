using System;
using System.Collections.Generic;
using System.Text;
using Tyres.DomainModels;

namespace Tyres.StaticData
{
    public static class Season2021
    {
        public static List<Driver> Drivers = new List<Driver>()
        {
            new Driver() {Team = "Mercedes", Car = "W12", Name = "Lewis Hamilton"},
            new Driver() {Team = "Mercedes", Car = "W12", Name = "Valtteri Bottas"},
            new Driver() {Team = "Red Bull", Car = "RB16B", Name = "Max Verstappen"},
            new Driver() {Team = "Red Bull", Car = "RB16B", Name = "Sergio Pérez"},
            new Driver() {Team = "McLaren", Car = "MCL35M", Name = "Lando Norris"},
            new Driver() {Team = "McLaren", Car = "MCL35M", Name = "Daniel Ricciardo"},
            new Driver() {Team = "Force India / Racing Point / Aston Martin", Car = "AMR21", Name = "Lance Stroll"},
            new Driver() {Team = "Force India / Racing Point / Aston Martin", Car = "AMR21", Name = "Sebastian Vettel"},
            new Driver() {Team = "Renault / Alpine", Car = "A521", Name = "Fernando Alonso"},
            new Driver() {Team = "Renault / Alpine", Car = "A521", Name = "Esteban Ocon"},
            new Driver() {Team = "Ferrari", Car = "SF21", Name = "Charles Leclerc"},
            new Driver() {Team = "Ferrari", Car = "SF21", Name = "Carlos Sainz"},
            new Driver() {Team = "Toro Rosso / AlphaTauri", Car = "AT02", Name = "Pierre Gasly"},
            new Driver() {Team = "Toro Rosso / AlphaTauri", Car = "AT02", Name = "Yuki Tsunoda"},
            new Driver() {Team = "Sauber / Alfa Romeo", Car = "C41", Name = "Antonio Giovinazzi"},
            new Driver() {Team = "Sauber / Alfa Romeo", Car = "C41", Name = "Kimi Räikkönen"},
            new Driver() {Team = "HAAS", Car = "VF21", Name = "Nikita Mazepin"},
            new Driver() {Team = "HAAS", Car = "VF21", Name = "Mick Schumacher"},
            new Driver() {Team = "Williams", Car = "FW43B", Name = "George Russell"},
            new Driver() {Team = "Williams", Car = "FW43B", Name = "Nicholas Latifi"},
        };

        public static Dictionary<string, Track> Tracks = new Dictionary<string, Track>()
        {
            { "Bahrain", new Track() {Name = "Bahrain International Circuit", TrackLength = 5412f} },
            { "Imola", new Track() {Name = "Imola", TrackLength = 4909f } },
            { "Portimão", new Track() {Name = "Portimão", TrackLength = 4653f } },
            { "Catalunya", new Track() {Name = "Catalunya", TrackLength = 4675f } },
            { "Monaco", new Track() {Name = "Monaco", TrackLength = 3337f } }
        };
    }
}
