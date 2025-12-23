using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using museat.Data; // Bunu ekledik (Veritabanýna eriþim için)
using museat.Models;
using System.Diagnostics;

namespace museat.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context; // Veritabaný baðlantýsý için ekledik

    // Constructor'a _context'i dahil ettik
    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Ham beatler
        var beats = await _context.Beats.ToListAsync();

        // Tamamlanmýþ (þarkýsý yüklenmiþ) iþbirlikleri
        var completedProjects = await _context.Collaborations
            .Include(c => c.Beat)
            .Where(c => c.IsApproved && !string.IsNullOrEmpty(c.FinalSongPath))
            .ToListAsync();

        // Ýkisini birden View'a göndermek için bir ViewModel kullanabiliriz 
        // veya þimdilik ViewBag ile hýzlýca çözelim:
        ViewBag.CompletedProjects = completedProjects;

        return View(beats);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}