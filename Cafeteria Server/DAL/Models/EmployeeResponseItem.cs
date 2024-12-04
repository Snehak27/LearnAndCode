using System;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaServer.DAL.Models
{
    public class EmployeeResponseItem
    {
        [Key]
        public int EmployeeResponseItemId { get; set; }
        public int EmployeeResponseId { get; set; }
        public int RecommendedItemId { get; set; }

        public virtual EmployeeResponse EmployeeResponse { get; set; }
        public virtual RecommendedItem RecommendedItem { get; set; }
    }
}
