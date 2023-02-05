using System;

namespace MovieStreaming.Custom.Models.LogsModel
{
    public class ViewAuthLog
    {
        public int? ID { get; set; }  
        public int ViewID { get; set; }
        public int RoleID { get; set; }    
        public int AuthorizationTypeID { get; set; }
        public int? CreatedUserId { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ViewURL { get; set; }
    }
}
