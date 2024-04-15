using System.ComponentModel.DataAnnotations;

namespace MediaContentManagementServer.Models
{
    public class MediaContent
    {
        [Required]
        public byte[] ImageBytes { get; set; }
        [Required]
        [StringLength(100)]
        public string Text { get; set; }
    }
}
