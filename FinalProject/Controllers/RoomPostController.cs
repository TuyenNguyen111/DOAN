using FinalProject.Data;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using FinalProject.Services;
using Newtonsoft.Json.Linq;
using FinalProject.Helpers;
using System.Security.Claims;

namespace FinalProject.Controllers
{
    public class RoomPostController : Controller
    {
        private readonly QlptContext _db;
        private readonly RoomService _roomService;
        private readonly UserService _userService;
        private readonly IHttpClientFactory _httpClientFactory;

        public RoomPostController(QlptContext db, RoomService _roomService,  IHttpClientFactory _httpClientFactory, UserService userService)
        {
            this._db = db;
            this._roomService = _roomService;
            this._httpClientFactory = _httpClientFactory;
            _userService = userService;
        }


        public IActionResult Index(int? id, int page = 1, int pageSize = 6)
        {
            var roomList = _db.RoomPostVM.FromSql($"GetRoomPosts {id}").ToList();

            int totalRooms = roomList.Count;
            int totalPages = (int)Math.Ceiling((double)totalRooms / pageSize);

            var rooms = roomList.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new RoomListVM
            {
                Rooms = rooms,
                CurrentPage = page,
                TotalPages = totalPages
            };
            
            return View(viewModel);
        }


        [HttpGet]
        public IActionResult SearchRoom(int? roomType, string? district, string? ward, int? adult, string? priceRange)
        {
            List<RoomPostVM> rooms = new List<RoomPostVM>();

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC SearchRoom @RoomTypeId, @District, @Ward, @Adult, @PriceRange";

                command.Parameters.Add(new SqlParameter("@RoomTypeId", (object)roomType ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@District", (object)district ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Ward", (object)ward ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Adult", (object)adult ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@PriceRange", (object)priceRange ?? DBNull.Value));

                _db.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var room = new RoomPostVM
                        {
                            PostId = reader.GetInt32(reader.GetOrdinal("PostId")),
                            RoomName = reader.GetString(reader.GetOrdinal("RoomName")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            RoomDescription = reader.GetString(reader.GetOrdinal("RoomDescription")),
                            RoomImage = reader.GetString(reader.GetOrdinal("RoomImage")),
                            RoomPrice = reader.GetDecimal(reader.GetOrdinal("RoomPrice")),
                            RoomSize = reader.GetDecimal(reader.GetOrdinal("RoomSize")),
                            RoomAddress = reader.GetString(reader.GetOrdinal("RoomAddress")),
                            RoomType = reader.GetString(reader.GetOrdinal("RoomType"))
                        };

                        rooms.Add(room);
                    }
                }

                _db.Database.CloseConnection();
            }

            return View("SearchRoom", rooms);
        }



        public IActionResult Detail(int id)
        {
            var roomDetail = new RoomPostDetailVM();
            var roomImages = new List<RoomImageVM>();

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC GetRoomDetail @PostID";
                command.Parameters.Add(new SqlParameter("@PostID", id));

                _db.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        roomDetail = new RoomPostDetailVM
                        {
                            PostId = reader.GetInt32(reader.GetOrdinal("PostId")),
                            RoomName = reader.GetString(reader.GetOrdinal("RoomName")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            RoomDescription = reader.GetString(reader.GetOrdinal("RoomDescription")),
                            RoomPrice = reader.GetDecimal(reader.GetOrdinal("RoomPrice")),
                            RoomSize = reader.GetDecimal(reader.GetOrdinal("RoomSize")),
                            RoomAddress = reader.GetString(reader.GetOrdinal("RoomAddress")),
                            RoomType = reader.GetString(reader.GetOrdinal("RoomType")),
                            DatePosted = reader.GetDateTime(reader.GetOrdinal("DatePosted")),
                            ExpirationDate = reader.GetDateTime(reader.GetOrdinal("ExpirationDate")),
                            FullName = reader.GetString(reader.GetOrdinal("FullName")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Gender = !reader.IsDBNull(reader.GetOrdinal("Gender")) && reader.GetBoolean(reader.GetOrdinal("Gender")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            UserImage = reader.GetString(reader.GetOrdinal("UserImage")),
                            UtilityNames = reader.GetString(reader.GetOrdinal("UtilityNames")),
                            UtilityDescriptions = reader.GetString(reader.GetOrdinal("UtilityDescriptions")),
                            Latitude = reader.GetDouble(reader.GetOrdinal("Latitude")),
                            Longitude = reader.GetDouble(reader.GetOrdinal("Longitude")),
                        };
                    }
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("RoomImage")))
                            {
                                roomImages.Add(new RoomImageVM
                                {
                                    RoomImage = reader.GetString(reader.GetOrdinal("RoomImage"))
                                });
                            }
                        }
                    }
                }

                _db.Database.CloseConnection();
            }

            var model = new RoomDetailAndImagesVM
            {
                RoomDetail = roomDetail,
                RoomImages = roomImages
            };

            return View(model);
        }

        [Authorize]
        public IActionResult UploadRoomPost()
        {
            if (Request.Cookies.TryGetValue("user_id", out var userIdString) && int.TryParse(userIdString, out var userId))
            {
                var model = new UploadRoomPostVM
                {
                    Users = _userService.GetUserById(userId),
                    Utility = _roomService.GetUtilities()?.ToList() ?? new List<UtilityVM>(),
                    RoomTypes = _roomService.GetRoomType(),
                    RoomPostContentVM = new RoomPostContentVM()
                };
                return View(model);
            }

            return RedirectToAction("SignIn", "Customer");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadRoomPost(UploadRoomPostVM model)
        {

            ModelState.Remove("RoomTypes");
            ModelState.Remove("Users");

            if (ModelState.IsValid)
            {
                var coordinates = await GetCoordinatesAsync(model.RoomPostContentVM.RoomAddress);

                if (coordinates.latitude == 0 && coordinates.longitude == 0)
                {
                    TempData["FailMessage"] = "Invalid coordinates.";
                    model.RoomTypes = _roomService.GetRoomType();
                    model.Utility = _roomService.GetUtilities();
                    model.Users = GetUserFromCookies();
                    return View(model);
                }

                int userId;

                if (!Request.Cookies.TryGetValue("user_id", out var userIdString) || !int.TryParse(userIdString, out userId))
                {
                    TempData["FailMessage"] = "User ID not found.";
                    return RedirectToAction("SignIn", "Customer");
                }

                if (_roomService.CheckExistingCoordinate(coordinates.latitude, coordinates.longitude))
                {
                    var roomCoordinateId = _roomService.GetRoomCoordinateId(coordinates.latitude, coordinates.longitude);

                    if (roomCoordinateId != 0)
                    {
                        model.RoomPostContentVM.RoomCoordinateId = roomCoordinateId;

                        if (_roomService.AddNewRoomPost(model.RoomPostContentVM, userId) is int newRoomId)
                        {
                            _roomService.SaveSelectedUtilities(newRoomId, model.SelectedUtilities);
                            SaveRoomImages(newRoomId, model.RoomImages);
                            TempData["SuccessMessage"] = "Added to favorites successfully!";
                        }
                        else
                        {
                            TempData["FailMessage"] = "Failed to add to favorites!";
                        }

                    }
                    else
                    {
                        TempData["FailMessage"] = "Room Coordinate Id doesn't exist!";
                    }
                }
                else
                {
                    int roomCoordinateId = _roomService.AddNewRoomCoordinate(coordinates.latitude, coordinates.longitude);

                    if (roomCoordinateId != 0)
                    {
                        model.RoomPostContentVM.RoomCoordinateId = roomCoordinateId;

                        if (_roomService.AddNewRoomPost(model.RoomPostContentVM, userId) is int newRoomId)
                        {
                            _roomService.SaveSelectedUtilities(newRoomId, model.SelectedUtilities);

                            SaveRoomImages(newRoomId, model.RoomImages);

                            TempData["SuccessMessage"] = "Added to favorites successfully!";
                        }
                        else
                        {
                            TempData["FailMessage"] = "Failed to add to favorites!";
                        }

                    }
                    else
                    {
                        TempData["FailMessage"] = "Failed to add new Room Coordinate!";
                    }
                }

                model.RoomTypes = _roomService.GetRoomType();
                model.Users = GetUserFromCookies();
                model.Utility = _roomService.GetUtilities();

                return View(model);
            }
            else
            {
                TempData["FailMessage"] = "Failed to add new Room Post!";

                model.RoomTypes = _roomService.GetRoomType();
                model.Users = GetUserFromCookies();
                model.Utility = _roomService.GetUtilities();
                return View(model);
            }
        }



        private async Task<(double latitude, double longitude)> GetCoordinatesAsync(string address)
        {
            var client = _httpClientFactory.CreateClient();
            string url = $"https://localhost:7127/api/geocode/getCoordinates?address={Uri.EscapeDataString(address)}";

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Geocode API Response: {jsonResponse}");

                var result = JObject.Parse(jsonResponse);

                if (result["latitude"] != null && result["longitude"] != null)
                {
                    double latitude = result["latitude"].Value<double>();
                    double longitude = result["longitude"].Value<double>();
                    return (latitude, longitude);
                }
                else
                {
                    Console.WriteLine($"Geocoding error: {result["status"]}"); 
                }
            }
            else
            {
                Console.WriteLine($"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}");
            }


            System.Diagnostics.Debug.WriteLine(response);

            return (0, 0); 
        }

        private UpdatePersonalInformationVM GetUserFromCookies()
        {
            if (Request.Cookies.TryGetValue("user_id", out var userIdString) && int.TryParse(userIdString, out var userId))
            {
                return _userService.GetUserById(userId);
            }

            return null; 
        }

        private void SaveRoomImages(int newRoomId, List<IFormFile> roomImages)
        {
            if (roomImages != null && roomImages.Count > 0)
            {
                foreach (var image in roomImages)
                {
                    var originalFileName = image.FileName;
                    var fileExtension = Path.GetExtension(originalFileName);
                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                    var filePath = Path.Combine("wwwroot/img/Room", uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(stream);
                    }

                    _roomService.SaveRoomImage(newRoomId, uniqueFileName); 
                }
            }
        }

        #region ManageRoom

        [Authorize]
        public IActionResult ManageRoom()
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var roomList = _roomService.GetRoomListByUserId(userId);

            return View(roomList);
        }
        #endregion
    }
}
