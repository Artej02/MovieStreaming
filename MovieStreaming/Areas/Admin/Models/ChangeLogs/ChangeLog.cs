using System;

namespace MovieStreaming.Areas.Admin.Models.ChangeLogs
{
    public class ChangeLog
    {
        public int Id { get; set; }
        public int EntryUserId { get; set; }
        public byte[] Before { get; set; }
        public byte[] After { get; set; }
        public int TableId { get; set; }
        public DateTime InsertDate { get; set; }
        public int ActionType { get; set; }
    }
}
