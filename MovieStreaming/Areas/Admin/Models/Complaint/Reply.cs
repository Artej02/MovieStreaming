using System;
using System.ComponentModel.DataAnnotations;

namespace MovieStreaming.Areas.Admin.Models.Complaint
{
    public class Reply
    {
        public int Id { get; set; }
        public int ComplaintId { get; set; }

        [Required]
        public string Message { get; set; }
        public int UserId { get; set; }

        public DateTime Date { get; set; }

        public string Email { get; set; }
    }
}
