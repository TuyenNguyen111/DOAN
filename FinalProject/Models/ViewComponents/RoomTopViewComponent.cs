using FinalProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Models.ViewComponents
{
    public class RoomTopViewComponent:ViewComponent
    {
        private readonly BillService billService;

        public RoomTopViewComponent(BillService billService)
        {
            this.billService = billService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int serviceId)
        {
            var rooms = await billService.GetRoomPostsByServiceIdAsync(serviceId);
            return View(rooms);
        }
    }
}
