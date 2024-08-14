using System;

namespace CafeteriaServer.DTO.RequestModel
{
    public class DiscardActionRequest
    {
        public string Action { get; set; }
        public List<int> MenuItemIds { get; set; }
    }
}
