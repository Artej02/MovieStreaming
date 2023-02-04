using Microsoft.AspNetCore.Http;

namespace MovieStreaming.Areas.Admin.Models.Movie
{
    public class MoviePopupViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile Video { get; set; }
        public decimal Cost { get; set; }
    }
}
