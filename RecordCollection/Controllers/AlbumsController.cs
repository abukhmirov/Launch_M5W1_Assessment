using Microsoft.AspNetCore.Mvc;
using RecordCollection.DataAccess;
using RecordCollection.Models;

namespace RecordCollection.Controllers
{
    public class AlbumsController : Controller {

        private readonly RecordCollectionContext _context;
        private readonly Serilog.ILogger _logger;

        public AlbumsController(RecordCollectionContext context, Serilog.ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var albums = _context.Albums.ToList();
            return View(albums);
        }

        [Route("/albums/{id:int}")]
        public IActionResult Show(int? id)
        {
            var album = _context.Albums.FirstOrDefault(a => a.Id == id);

            return View(album);
        }

        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Album album)
        {
            _context.Albums.Add(album);
            _context.SaveChanges();

            _logger.Information("Album " + album.Title + " created!");          //REWROTE THE LOGGER CONTENT TO BETTER SUIT THE LOG PURPOSE

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("/albums/{id:int}")]
        public IActionResult Delete(int? id)
        {
            if (id == null)                                                         //NULL CHECK FOR ID
            {
                _logger.Warning("Album ID was null in Delete action.");             //LOG THE NULL ALBUM ID
                return NotFound();
            }

            var album = _context.Albums.FirstOrDefault(a => a.Id == id);

            if (album == null)                                                      //NULL CHECK FOR ALBUM
            {
                _logger.Warning($"Attempted to delete non-existing album with ID: {id}");       //LOG THE NULL ALBUM 
                return NotFound();
            }

            try                                                                      //TRY/CATCH FOR _CONTEXT.SAVECHANGES
            {
                _context.Albums.Remove(album);
                _context.SaveChanges();

                _logger.Information($"Success! {album.Title} was removed from the database.");      //LOG THE SUCCESSFUL DELETION OF ALBUM

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                _logger.Fatal($"FATAL ERROR! Failed to delete album {album.Title}.");               //LOG THE FAILURE TO DELETE ALBUM
                return StatusCode(500);
            }

        }
    }
}
