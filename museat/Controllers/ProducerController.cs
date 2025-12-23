using Microsoft.AspNetCore.Authorization; // Yetkilendirme için gerekli
using Microsoft.AspNetCore.Identity; // Kullanıcı bilgisi için gerekli
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Include kullanabilmek için gerekli
using museat.Data;
using museat.Models;

namespace museat.Controllers
{
    [Authorize(Roles = "Producer")] // KİLİT BURADA: Sadece Prodüktörler bu controller'a girebilir.
    public class ProducerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        public ProducerController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        // Beat Yükleme Sayfası
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(Beat beat, IFormFile audioFile)
        {
            if (audioFile != null)
            {
                // Giriş yapan prodüktörün ID'sini alıp beat'e kaydediyoruz
                beat.ProducerId = _userManager.GetUserId(User);

                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(audioFile.FileName);
                string extension = Path.GetExtension(audioFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/beats/", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await audioFile.CopyToAsync(fileStream);
                }

                beat.AudioFilePath = "/beats/" + fileName;
                _context.Beats.Add(beat);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // --- YENİ: SATIŞLAR VE ONAY BEKLEYENLER SAYFASI ---
        public async Task<IActionResult> MySales()
        {
            var currentProducerId = _userManager.GetUserId(User);

            // Bu prodüktöre ait beatlere gelen tüm talepleri çekiyoruz
            var requests = await _context.Collaborations
                .Where(c => _context.Beats.Any(b => b.Id == c.BeatId && b.ProducerId == currentProducerId))
                .ToListAsync();

            return View(requests);
        }

        // --- YENİ: TALEBİ ONAYLAMA İŞLEMİ ---
        [HttpPost]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            var request = await _context.Collaborations.FindAsync(id);
            if (request != null)
            {
                request.IsApproved = true; // Onaylandığını işaretle
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(MySales));
        }
    }
}