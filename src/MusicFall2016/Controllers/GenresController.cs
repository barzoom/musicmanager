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
                try { _context.Genres.Add(g); }
                catch
                {
                }
                _context.SaveChanges();
                return Index();
            }
            else
            {
                return Create();
            }
        }
    }
}
