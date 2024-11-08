using Humanizer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FinalProject.ViewModels
{
    public class ServiceVM
    {
        public int serviceId { get; set; }
        public string serviceName { get; set; }
        public string serviceDescription { get; set; }
        public decimal servicePrice { get; set; }
        public int serviceTime { get; set; }
    }
}
