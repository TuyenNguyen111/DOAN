
namespace FinalProject.Data
{
    public partial class Service
    {
        public int ServiceId { get; set; }

        public string ServiceName { get; set; } = null!;

        public string? ServiceDescription { get; set; }

        public decimal? ServicePrice { get; set; }

        public int? ServiceTime { get; set; }

        public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

        public static implicit operator int(Service? v)
        {
            throw new NotImplementedException();
        }
    }

}
