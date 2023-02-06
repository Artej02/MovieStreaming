using System.ComponentModel.DataAnnotations;
using System;

namespace MovieStreaming.Areas.Admin.Models.ViewAuthorization
{
    public class ViewAuthorization
    {
        [Key]
        public int? ID { get; set; }
        [Required]
        public int ViewID { get; set; }
        [Required]
        public int RoleID { get; set; }
        [Required]
        public int AuthorizationTypeID { get; set; }
        public int? CreatedUserId { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ViewURL { get; set; }
    }
}
