using System;

namespace MovieStreaming.Areas.Admin.Models.Movie
{
    public class Movie
    {

        public int? Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Video { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}
