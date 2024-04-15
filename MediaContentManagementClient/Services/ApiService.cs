using MediaContentManagementClient.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MediaContentManagementClient.Services
{
    internal class ApiService
    {
        private readonly HttpClient httpClient;

        public ApiService()
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7159")
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> SaveImageAsync(MediaContent mediaContent)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/Media/SaveImage", mediaContent);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return $"Ошибка: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                return $"Произошла ошибка: {ex.Message}";
            }
        }

        public async Task<List<ImageInfo>> GetImagesAsync(int pageNumber, int pageSize)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/Media/GetImages?pageNumber={pageNumber}&pageSize={pageSize}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var imagesInfo = JsonSerializer.Deserialize<List<ImageInfo>>(jsonContent);

                    return imagesInfo;
                }
                else
                {
                    throw new Exception($"{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public (bool Success, string Count) GetAllImagesCount()
        {
            try
            {
                var response = httpClient.GetAsync("api/Media/GetAllImagesCount").Result;

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = response.Content.ReadAsStringAsync().Result;
                    return (true, jsonContent);
                }
                else
                {
                    return (false, $"{response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
