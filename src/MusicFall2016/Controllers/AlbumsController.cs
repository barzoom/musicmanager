using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicFall2016.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MusicFall2016.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly MusicDbContext _context;

        public AlbumsController(MusicDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, string search)
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_asc" : "";
            ViewData["ArtistSortParm"] = String.IsNullOrEmpty(sortOrder) ? "artist_asc" : "artist_desc";
            ViewData["GenreSortParm"] = String.IsNullOrEmpty(sortOrder) ? "genre_asc" : "genre_desc";
            ViewData["PriceSortParm"] = String.IsNullOrEmpty(sortOrder) ? "price_asc" : "price_desc";
            ViewData["LikeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "like_asc" : "";

            var albums = from x in _context.Albums.Include(a => a.Artist).Include(a => a.Genre)
                         select x;

            if (search != null)
            {
                if (!String.IsNullOrEmpty(search))
                    albums = albums.Where(s => s.Title.Contains(search) || s.Artist.Name.Contains(search) || s.Genre.Name.Contains(search));
            }

            switch (sortOrder)
            {
                case "title_asc":
                    albums = albums.OrderBy(s => s.Title);
                    break;
                case "artist_asc":
                    albums = albums.OrderBy(s => s.Artist.Name);
                    break;
                case "genre_asc":
                    albums = albums.OrderBy(s => s.Genre.Name);
                    break;
                case "price_asc":
                    albums = albums.OrderBy(s => s.Price);
                    break;
                 case "like_asc":
                  albums = albums.OrderBy(s => s.Likes);
                 break;
                case "artist_desc":
                    albums = albums.OrderByDescending(s => s.Artist.Name);
                    break;
                case "genre_desc":
                    albums = albums.OrderByDescending(s => s.Genre.Name);
                    break;
                case "price_desc":
                    albums = albums.OrderByDescending(s => s.Price);
                    break;
                default:
                    albums = albums.OrderBy(s => s.Title);
                    break;
            }
            if (sortOrder == "artist_desc" || sortOrder == "genre_desc" || sortOrder == "price_desc")
            {
                sortOrder = "";
                ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_asc" : "";
                ViewData["ArtistSortParm"] = String.IsNullOrEmpty(sortOrder) ? "artist_asc" : "artist_desc";
                ViewData["GenreSortParm"] = String.IsNullOrEmpty(sortOrder) ? "genre_asc" : "genre_desc";
                ViewData["PriceSortParm"] = String.IsNullOrEmpty(sortOrder) ? "price_asc" : "price_desc";
                ViewData["LikeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "like_asc" : "";
            }
            return View(await albums.AsNoTracking().ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.ArtistList = new SelectList(_context.Artists, "ArtistID", "Name");
            ViewBag.GenreList = new SelectList(_context.Genres, "GenreID", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Album album, string newArtist, string newGenre)
        {
            
            if (ModelState.IsValid)
            {
                if (newArtist != null)
                {
                    Artist artist = new Artist();
                    foreach (var artistName in _context.Artists.ToList())
                    {
                        if (artistName.Name == newArtist) { }
                        else
                        {
                            artist.Name = newArtist;
                            artist.Bio = "";
                        }
                    }
                    _context.Artists.Add(artist);
                }
                if (newGenre != null)
                {
                    Genre genre = new Genre();
                    foreach (var genreName in _context.Genres.ToList())
                    {
                        if (genreName.Name == newGenre) { }
                        else
                        {
                            genre.Name = newGenre;
                        }
                    }
                    _context.Genres.Add(genre);
                }
                _context.SaveChanges();

                album.ArtistID = _context.Artists.Last().ArtistID;
                album.GenreID = _context.Genres.Last().GenreID;
                album.Artist = _context.Artists.Last();
                album.Genre = _context.Genres.Last();

                _context.Albums.Add(album);
                _context.SaveChanges();


                return RedirectToAction("Index");
            }
            ViewBag.ArtistList = new SelectList(_context.Artists, "ArtistID", "Name");
            ViewBag.GenreList = new SelectList(_context.Genres, "GenreID", "Name");
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.ArtistList = new SelectList(_context.Artists, "ArtistID", "Name");
            ViewBag.GenreList = new SelectList(_context.Genres, "GenreID", "Name");

            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums.SingleOrDefaultAsync(m => m.AlbumID == id);
            if (album == null)
            {
                return NotFound();
            }
            return View(album);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlbumID,Title,Price,ArtistID,Artist,GenreID,Genre")] Album album)
        {
            ViewBag.ArtistList = new SelectList(_context.Artists, "ArtistID", "Name");
            ViewBag.GenreList = new SelectList(_context.Genres, "GenreID", "Name");
            if (id != album.AlbumID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(album);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(album);
        }

        public IActionResult Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var album = _context.Albums
                .Include(a => a.Artist)
                .SingleOrDefault(a => a.AlbumID == id);
            if (album == null)
            {
                return NotFound();
            }
            return View(album);
        }

        [HttpPost]
        public IActionResult Delete(Album album)
        {
            _context.Albums.Remove(album);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var albums = _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .SingleOrDefault(a => a.AlbumID == id);
            if (albums == null)
            {
                return NotFound();
            }
            return View(albums);
        }

        public IActionResult Like(int? id, int? from)
        {
            int idnumber = (int)id;
            if (id == null)
            {
                return NotFound();
            }
            var albums = _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .SingleOrDefault(a => a.AlbumID == id);
            if (albums == null)
            {
                return NotFound();
            }
            albums.Likes += 1;
            _context.SaveChanges();

            if(from == 1)
                return RedirectToAction("Details", new { id = idnumber });
            else
                return RedirectToAction("Index");
        }
    }
}
