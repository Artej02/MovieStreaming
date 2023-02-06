using Microsoft.AspNetCore.Mvc;
using MovieStreaming.Areas.Admin.Models.Movie;
using MovieStreaming.Custom.DatabaseHelpers;
using MovieStreaming.Custom.Models;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;

namespace MovieStreaming.Areas.Users.Controllers
{
    public class MoviesController : Controller
    {
        private MovieDBContext _context = new MovieDBContext();
        public MoviesController(MovieDBContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            ViewBag.Movies = (await new Query().Select<Movie>("select * from Movie")).Result.ToList();
            ViewBag.M = true;
            return View();
        }

        public async Task<ActionResult> Details(Movie mov)
        {
            Movie movie = _context.Movies.Find(mov.Id);
            ViewBag.ThisMovie = (await new Query().Select<Movie>($"select * from Movie where Id={mov.Id}")).Result.ToList();
            return View(movie);
        }
    }
}
