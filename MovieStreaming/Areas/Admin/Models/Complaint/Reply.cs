using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.ComponentModel.DataAnnotations;

namespace MovieStreaming.Areas.Adim.Models.Complaint
{
    public class Reply
    {
        public int Id { get; set; }
        public int TicketId { get; set; }

        [Required]
        public string Message { get; set; }
        public int UserId { get; set; }

        public DateTime Date { get; set; }

        public string Email { get; set; }
    }
}
