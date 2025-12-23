using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using museat.Data;

namespace museat.Controllers
{
    [Authorize(Roles = "Writer")]
    public class WriterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public WriterController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> MyProjects()
        {
            var userId = _userManager.GetUserId(User);

            // .Include(c => c.Beat) sayesinde Beat bilgilerine erişebiliyoruz
            var projects = await _context.Collaborations
                .Include(c => c.Beat)
                .Where(c => c.WriterId == userId)
                .ToListAsync();

            return View(projects);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFinalSong(int collabId, IFormFile finalFile)
        {
            if (finalFile != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = "Final_" + Guid.NewGuid().ToString() + Path.GetExtension(finalFile.FileName);
                string path = Path.Combine(wwwRootPath + "/final_songs/", fileName);

                // Klasör yoksa oluştur
                if (!Directory.Exists(Path.Combine(wwwRootPath, "final_songs")))
                    Directory.CreateDirectory(Path.Combine(wwwRootPath, "final_songs"));

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await finalFile.CopyToAsync(fileStream);
                }

                var collab = await _context.Collaborations.FindAsync(collabId);
                if (collab != null)
                {
                    collab.FinalSongPath = "/final_songs/" + fileName;
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(MyProjects));
        }
    }
}