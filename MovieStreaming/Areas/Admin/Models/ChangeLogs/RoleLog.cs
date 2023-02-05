using System;

namespace MovieStreaming.Custom.Models.LogsModel
{
    public class RoleLog
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int OrganisationTypeId { get; set; }
        public int CreatedUserId { get; set; }
        public int UpdatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
