using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using museat.Models;

namespace museat.Data
{
    // IdentityDbContext sayesinde kullanıcı ve rol tabloları otomatik gelir
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Kendi tablolarımızı buraya ekliyoruz
        public DbSet<Beat> Beats { get; set; }
        public DbSet<Collaboration> Collaborations { get; set; }
    }
}