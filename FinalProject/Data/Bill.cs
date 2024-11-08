namespace FinalProject.Data
{
    public partial class Bill
    {
        public int BillId { get; set; }

        public int PostId { get; set; }

        public int ServiceId { get; set; }

        public decimal? TotalPrice { get; set; }

        public DateOnly? PaymentDate { get; set; }

        public int? BillStatus { get; set; }

        public DateOnly? ExpirationDate { get; set; }

        public virtual RoomPost Post { get; set; } = null!;

        public virtual Service Service { get; set; } = null!;
    }

}
