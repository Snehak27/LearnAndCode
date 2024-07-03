using System;

namespace CafeteriaServer.DTO.RequestModel
{
    public class UpdateProfileRequest
    {
        public int UserId { get; set; }
        public int FoodTypeId { get; set; }
        public int SpiceLevelId { get; set; }
        public int CuisineTypeId { get; set; }
        public bool HasSweetTooth { get; set; }
    }
}
