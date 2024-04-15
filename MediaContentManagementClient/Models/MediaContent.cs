using System.ComponentModel.DataAnnotations;

namespace MediaContentManagementClient.Models
{
    internal class MediaContent
    {
        [Required]
        [MaxLength(100)]
        public string Text { get; set; }
        [Required]
        public byte[] ImageBytes { get; set; }
    }
}
