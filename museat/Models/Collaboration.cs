using System.ComponentModel.DataAnnotations.Schema;

namespace museat.Models
{
    public class Collaboration
    {
        public int Id { get; set; }

        public int BeatId { get; set; }

        // Bu satır iki tabloyu birbirine bağlar (Navigation Property)
        [ForeignKey("BeatId")]
        public virtual Beat? Beat { get; set; }

        public string? WriterId { get; set; }
        public bool IsApproved { get; set; } = false;

        // Yazıcının yükleyeceği final şarkı yolu
        public string? FinalSongPath { get; set; }
    }
}