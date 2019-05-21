using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebServiceAds.Models;

namespace WebServiceAds.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        [BindProperty]
        private Images Img { get; set; }
        public ImagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Images>>> GetAllImages()
        {
            return await _context.Images.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Images>> GetImages(int id)
        {
            var emp = await _context.Images.FindAsync(id);

            if (emp == null)
                return NotFound();

            return emp;
        }

        [HttpPost]
        public async Task<ActionResult<Images>> Insert()
        {
            _context.Images.Add(Img);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetImages), new { id = Img.Id }, Img);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Images>> Edit(int id)
        {
            if (id != Img.Id)
            {
                return BadRequest();
            }
            _context.Entry(Img).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Images>> Delete(int id)
        {
            var img = await _context.Images.FindAsync(id);

            if (img == null)
                return NotFound();
            _context.Images.Remove(img);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}