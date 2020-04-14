using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Text;

namespace VsDraw
{
    public static class SaveImage
    {
        private const string BlobStorageBasePathKey = "BlobStorageBasePath";
        private const string BlobStorageConnectionStringKey = "BlobStorageConnectionString";
        private const int oneYear = 31536000;
        private static readonly string cacheControlHeaderValue = "max-age=" + oneYear;

        [FunctionName("SaveImage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            IBinder binder,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var form = await req.ReadFormAsync();

            var guid = Guid.NewGuid();
            var imagePath = $"image/{guid}.png";
            var imageDataPath = $"imagedata/{guid}.json";
            try
            {
                var imageBlob = await binder.OutputBlob(imagePath, "image/png");
                var imageDataBlob = await binder.OutputBlob(imageDataPath, "application/json");

                using (var image = await imageBlob.OpenWriteAsync())
                using (var imageData = await imageDataBlob.OpenWriteAsync())
                {
                    await form.Files.GetFile("Image").CopyToAsync(image);
                    await imageData.WriteAsync(Encoding.UTF8.GetBytes(form["ImageData"].ToString()));
                    log.LogInformation($"Saved image and imageData {guid}");
                }
            }
            catch (Exception e)
            {
                log.LogError($"error transferring form data to blob bindings: {e}");
                return new StatusCodeResult(400);
            }
            var blobStorageBaseUrl = Environment.GetEnvironmentVariable(BlobStorageBasePathKey);
            var responseBody = JsonConvert.SerializeObject(new
            {
                imageUrl = $"{blobStorageBaseUrl}{imagePath}",
                imageDataUrl = $"{blobStorageBaseUrl}{imageDataPath}"
            });
            return new JsonResult(responseBody) { StatusCode = 201 };
        }

        public async static Task<CloudBlockBlob> OutputBlob(this IBinder binder, string path, string ContentType)
        {
            var result = await binder.BindAsync<CloudBlockBlob>(Blob(path));
            result.Properties.ContentType = ContentType;
            result.Properties.CacheControl = cacheControlHeaderValue;
            return result;
        }
        private static BlobAttribute Blob(string path) =>
            new BlobAttribute(path, FileAccess.Write) { Connection = BlobStorageConnectionStringKey };
    }
}
