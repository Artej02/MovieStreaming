using System;

namespace MovieStreaming.Custom.Models.LogsModel
{
    public class UserLog
    {
        public int? Id { get; set; }             
        public string Name { get; set; }               
        public string Surname { get; set; }               
        public string Username { get; set; }               
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public int? OrganisationId { get; set; }
        public int? RoleId { get; set; }
        public bool? IsApproved { get; set; }
        public int? CreatedUserId { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
