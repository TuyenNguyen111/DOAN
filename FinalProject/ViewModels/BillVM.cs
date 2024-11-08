using Humanizer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace FinalProject.ViewModels
{
    public class BillVM
    {
        public int billId { get; set; }
        public int postId { get; set; }
        public int serviceId { get; set; }
        public decimal? totalPrice { get; set; }
        public DateOnly? paymentDate { get; set; }
        public int? billStatus { get; set; }
        public DateOnly? expirationDate { get; set; }
    }
}
