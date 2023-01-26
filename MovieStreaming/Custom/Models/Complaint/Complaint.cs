namespace MovieStreaming.Custom.Models.Complaint
{
    public class Complaint
    {
          public int Id { get; set; }

          public string Title { get; set; }

          public string Description { get; set; }

          public int TypeId { get; set; }

          public int SeverityId { get; set; }

          public int CreatedUserId { get; set; }

          public bool IsActive { get; set; }

    }
}
