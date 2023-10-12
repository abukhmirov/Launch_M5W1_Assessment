using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecordCollection.DataAccess;

namespace RecordCollection.Controllers
{
    [Route("api/albums")]
    [ApiController]
    public class AlbumsAPIController : ControllerBase
    {
        private readonly RecordCollectionContext _context;
        private readonly Serilog.ILogger _logger;

        public AlbumsAPIController(RecordCollectionContext context, Serilog.ILogger logger)
        {
            _context = context;
            _logger = logger;
        }
                                                //ADDED A TRY/CATCH FOR GETALL ALBUMS
        public IActionResult GetAll()
        {
            try
            {
            var albums = _context.Albums.ToList();
            return new JsonResult(albums);
            }
            catch (Exception ex)
            {
                _logger.Error("Albums could not be fetched.");
                return StatusCode(404);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            var album = _context.Albums.FirstOrDefault(a => a.Id == id);
            return new JsonResult(album);
        }

        [HttpDelete("{id}")]
        public void DeleteOne(int id)
        {
            var album = _context.Albums.FirstOrDefault(a => a.Id == id);
            _context.Albums.Remove(album);
            _context.SaveChanges();
        }
    }
}
