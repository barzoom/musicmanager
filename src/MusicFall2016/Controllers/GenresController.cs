using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicFall2016.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MusicFall2016.Controllers
{
    public class GenresController : Controller
    {
        private readonly MusicDbContext _context;

        public GenresController(MusicDbContext context)
        {
            _context = context;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            var genres =  _context.Genres.ToList();
            return View(genres);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Genre g)
        {
            if (ModelState.IsValid)
            {
                foreach (Genre genreTest in _context.Genres)
                {
                    var name = genreTest.Name;
                    if (name == g.Name)
                    {
                        return RedirectToAction("Create");
                    }
                }
                 _context.Genres.Add(g); 
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return Create();
            }
        }
        public IActionResult AlbumList(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var albums = _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre).ToList();
            var genre = _context.Genres.SingleOrDefault(a => a.GenreID == id);
            ViewData["Genre"] = genre.Name;
            if (albums == null)
            {
                return RedirectToAction("Index");
            }
            return View(albums);
        }
    }
}
