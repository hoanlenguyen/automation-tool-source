using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BITool.Helpers;

namespace BITool.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveAndGetShortUrl(byte[] fileBytes, string fileOriginName, string folder = "", bool isAddAppfix = false);

        Task<string> SaveAndGetFullUrl(byte[] fileBytes, string fileOriginName, string folder = "", bool isAddAppfix = false);

        Task<byte[]?> ReadBlobInByteArray(string url);
    }

    public class FileStorageService : IFileStorageService
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly BlobContainerClient containerClient;
        private readonly string End_Point;
        private readonly string Container_Name;
        private readonly List<string> AllowedFileExtensions;
        private readonly List<string> PathTraversalBlackList;

        public FileStorageService(
            IConfiguration configuration
            )
        {
            End_Point = configuration["AzureStorage:EndPoint"];
            Container_Name = configuration["AzureStorage:ContainerName"];
            blobServiceClient = new BlobServiceClient(configuration["AzureStorage:ConnectionString"]);
            containerClient = blobServiceClient.GetBlobContainerClient(Container_Name);
        }

        public async Task<string> SaveAndGetShortUrl(byte[] fileBytes, string fileOriginName, string folder = "", bool isAddAppfix = false)
        {
            var fileExtension = Path.GetExtension(fileOriginName).ToLower();
            var fileName = isAddAppfix ? $"{Path.GetFileNameWithoutExtension(fileOriginName)}-{DateTime.Now.ToFileTime()}{fileExtension}" : fileOriginName;
            var filePath = folder.IsNotNullOrEmpty() ? $"{folder}/{fileName}" : fileName;
            var blobClient = containerClient.GetBlobClient($"{folder}/{fileName}");
            var stream = new MemoryStream(fileBytes);
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = fileExtension.GetFileContentType() });
            stream.Close();
            return filePath;
        }

        public async Task<string> SaveAndGetFullUrl(byte[] fileBytes, string fileOriginName, string folder = "", bool isAddAppfix = false)
        {
            var fileExtension = Path.GetExtension(fileOriginName).ToLower();
            var fileName = isAddAppfix ? $"{Path.GetFileNameWithoutExtension(fileOriginName)}-{DateTime.Now.ToFileTime()}{fileExtension}" : fileOriginName;
            var filePath = folder.IsNotNullOrEmpty() ? $"{folder}/{fileName}" : fileName;
            var blobClient = containerClient.GetBlobClient($"{folder}/{fileName}");
            var stream = new MemoryStream(fileBytes);
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = fileExtension.GetFileContentType() });
            stream.Close();
            return $"{End_Point}/{Container_Name}/{filePath}";
        }

        public async Task<byte[]?> ReadBlobInByteArray(string url)
        {
            BlobClient blobClient = containerClient.GetBlobClient(url);
            if (!await blobClient.ExistsAsync())
                return null;
            BlobDownloadInfo download = await blobClient.DownloadAsync();
            using (var ms = new MemoryStream())
            {
                await download.Content.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }
}