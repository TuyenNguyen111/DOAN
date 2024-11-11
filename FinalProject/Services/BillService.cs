using FinalProject.Data;
using FinalProject.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FinalProject.Services
{
    public class BillService
    {
        private readonly QlptContext db;

        public BillService(QlptContext db)
        {
            this.db = db;
        }

        public async Task<List<RoomPostVM>> GetRoomPostsByServiceIdAsync(int serviceId)
        {
           

            var bills = await db.Bills
              .Where(b => b.ServiceId == serviceId && b.ExpirationDate >= DateOnly.FromDateTime(DateTime.UtcNow))
              .Include(b => b.Post)
              .ThenInclude(p => p.RoomImages)
              .Select(b => new RoomPostVM
              {
                  PostId = b.Post.PostId,
                  RoomName = b.Post.RoomName,
                  Quantity = (int)b.Post.Quantity,
                  RoomDescription = b.Post.RoomDescription,
                  RoomImage = b.Post.RoomImages.Select(img => img.ImageUrl).FirstOrDefault(),
                  RoomPrice = (decimal)b.Post.RoomPrice,
                  RoomSize = (decimal)b.Post.RoomSize,
                  RoomAddress = BillService.FormatAddress(b.Post.Address),
                  RoomType = b.Post.RoomType.TypeName ?? "Unknown"
              })
         .ToListAsync();

            return bills;
        }

        public static string FormatAddress(string address)
        {
            if (string.IsNullOrEmpty(address)) return address;

            // Sử dụng Regex để tìm phần số nhà, tên đường và quận
            var match = Regex.Match(address, @"^(.+?),\s*Phường[^,]+,\s*(Quận[^,]+)");
            if (match.Success)
            {
                // Ghép phần số nhà và tên đường với phần quận
                return $"{match.Groups[1].Value}, {match.Groups[2].Value}";
            }

            return address; // Trả về địa chỉ gốc nếu không khớp
        }
    }
}
