using AutoMapper;

namespace MovieStreaming.Areas.Admin.Models.Movie
{
    public class MovieMappingProfile : Profile
    {
        public MovieMappingProfile()
        {
            CreateMap<MoviePopupViewModel, Movie>();
        }
    }
}
