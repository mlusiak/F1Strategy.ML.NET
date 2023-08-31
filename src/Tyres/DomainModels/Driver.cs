namespace Tyres.DomainModels
{
    public class Driver
    {
        public string Name { get; set; }
        public string Team { get; set; }
    }

    public class Top10Driver : Driver
    {
        public string StartingCompound { get; set; }
    }
}