using System;

namespace CafeteriaServer.DTO.ResponseModel
{
    public class EmployeePreferenceResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public PreferenceResponse PreferenceResponse { get; set; }
    }

    public class PreferenceResponse
    {
        public string FoodPreference { get; set; }
        public string SpiceLevel { get; set; }
        public string CuisinePreference { get; set; }
        public bool HasSweetTooth { get; set; }
    }
}
