using ImageMagick;
using MediaContentManagementServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MediaContentManagementServer.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly string _imagesDirectory;

        public MediaController()
        {
            _imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Images");
        }

        [HttpPost]
        public async Task<IActionResult> SaveImage([FromBody] MediaContent mediaContent)
        {
            try
            {
                string fileName = Guid.NewGuid().ToString() + ".jpg";
                string imagePath = Path.Combine(_imagesDirectory, fileName);
                Directory.CreateDirectory(Path.Combine(_imagesDirectory));

                using (MagickImage image = new MagickImage(mediaContent.ImageBytes))
                {
                    image.SetAttribute("Comment", mediaContent.Text);
                    await image.WriteAsync(imagePath);
                }

                return Ok("Изображение успешно сохранено");
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetImages(int pageNumber, int pageSize)
        {
            try
            {
                if (!Directory.Exists(_imagesDirectory))
                {
                    return Ok(new List<ImageInfo>());
                }

                var imageFiles = Directory.GetFiles(_imagesDirectory, "*.jpg").OrderByDescending(GetImageCreationDate).Skip((pageNumber - 1) * pageSize).Take(pageSize);

                List<ImageInfo> imageInfos = new List<ImageInfo>();
                foreach (var filePath in imageFiles)
                {
                    using (MagickImage image = new MagickImage(filePath))
                    {
                        var imageInfo = new ImageInfo
                        {
                            FilePath = filePath,
                            Text = image.GetAttribute("Comment")
                        };
                        imageInfos.Add(imageInfo);
                    }
                }

                return Ok(imageInfos);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAllImagesCount()
        {
            try
            {
                if (!Directory.Exists(_imagesDirectory))
                {
                    return Ok(0);
                }

                int countImages = Directory.GetFiles(_imagesDirectory, "*.jpg").Count();

                return Ok(countImages);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private DateTime GetImageCreationDate(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo.CreationTime;
        }
    }
}
