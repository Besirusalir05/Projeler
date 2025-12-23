using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using museat.Data;
using museat.Models;

namespace museat.Controllers
{
    // Sadece sisteme giriş yapmış "Writer" (Yazıcı) rolü bu işlemleri yapabilir
    [Authorize(Roles = "Writer")]
    public class CollaborationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CollaborationController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Yazıcı "İşbirliği Talebi Gönder" butonuna bastığında çalışacak metot
        [HttpPost]
        public async Task<IActionResult> SendRequest(int beatId)
        {
            // 1. Giriş yapan Yazıcının (Alıcının) ID'sini alıyoruz
            var userId = _userManager.GetUserId(User);

            // 2. Aynı beat için daha önce istek atılmış mı kontrol ediyoruz (Mükerrer kaydı önlemek için)
            var existingRequest = _context.Collaborations
                .FirstOrDefault(c => c.BeatId == beatId && c.WriterId == userId);

            if (existingRequest == null)
            {
                // 3. Yeni bir işbirliği (Collaboration) kaydı oluşturuyoruz
                var newCollab = new Collaboration
                {
                    BeatId = beatId,
                    WriterId = userId,
                    IsApproved = false // Prodüktör onaylayana kadar false kalacak
                };

                _context.Collaborations.Add(newCollab);
                await _context.SaveChangesAsync();

                // Başarılı mesajı gönderilebilir (Opsiyonel)
                TempData["Message"] = "Talebiniz başarıyla iletildi. Prodüktörün onayı bekleniyor.";
            }
            else
            {
                TempData["Error"] = "Bu beat için zaten bir talebiniz bulunuyor.";
            }

            // İşlem bittikten sonra ana sayfaya geri dön
            return RedirectToAction("Index", "Home");
        }
    }
}