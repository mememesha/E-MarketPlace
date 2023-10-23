namespace EM.Contracts
{
    public class GeoTag : EntityBase
    {
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
        public virtual List<Place>? Places { get; set; }
    }
}