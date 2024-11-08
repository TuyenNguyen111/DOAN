using FinalProject.Data;
using FinalProject.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Services
{
    public class RegisterService
    {
        private readonly QlptContext db;

        public RegisterService(QlptContext db)
        {
            this.db = db;
        }
        public async Task<Service> GetServiceByIdAsync(int serviceId)
        {
            var service = await db.Services.FirstOrDefaultAsync(s => s.ServiceId == serviceId);
            if (service == null)
            {
                throw new Exception("Service not found in the database.");
            }
            return service;
        }

        public async Task<List<ServiceVM>> GetServicesAsync()
        {
            return await db.Services
                .Select(s => new ServiceVM
                {
                    serviceId = s.ServiceId,
                    serviceName = s.ServiceName,
                    serviceDescription = s.ServiceDescription ?? string.Empty,
                    servicePrice = s.ServicePrice ?? 0,
                    serviceTime = s.ServiceTime ?? 0
                }).ToListAsync();
        }

        public async Task<int> CreateBillAsync(int postId, int serviceId, decimal totalPrice, DateOnly paymentDate, int billStatus, DateOnly expirationDate)
        {
            var roomPostExists = await db.RoomPosts.AnyAsync(rp => rp.PostId == postId);
            if (!roomPostExists)
            {
                throw new Exception($"PostId {postId} does not exist in Room_Post table.");
            }
            var bill = new Bill
            {
                PostId = postId,
                ServiceId = serviceId,
                TotalPrice = totalPrice,
                PaymentDate = paymentDate,
                BillStatus = billStatus,
                ExpirationDate = expirationDate
            };

            db.Bills.Add(bill);
            await db.SaveChangesAsync();

            return bill.BillId; // Return the bill ID if needed
        }
        public async Task<List<BillVM>> GetBillsAsync()
        {
            return await db.Bills.Select(b => new BillVM
            {
                billId = b.BillId,
                postId = b.PostId,
                serviceId = b.ServiceId,
                totalPrice = b.TotalPrice,
                paymentDate = b.PaymentDate,
                billStatus = b.BillStatus,
                expirationDate = b.ExpirationDate
            }).ToListAsync();
        }

    }
}
