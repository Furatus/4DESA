using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureAPI.Services;

public class BlobStorageService : IBlobStorageService
{
    BlobServiceClient _blobServiceClient;
    IAzureService _azureService;

    public BlobStorageService(BlobServiceClient blobServiceClient, IAzureService azureService)
    {
        _blobServiceClient = blobServiceClient;
        _azureService = azureService;
    }

    public string uploadFileToAzureBlob(IFormFile file, Guid PostId)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("mediastorage");
            var fileExtension = Path.GetExtension(file.FileName);
            var blobClient = containerClient.GetBlobClient($"{PostId}{fileExtension}");
            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };

            blobClient.Upload(file.OpenReadStream(), new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders,
            });

            var blobUri = blobClient.Uri.ToString();
            _azureService.UploadMedia(PostId, blobUri);

            return blobUri;
        }
catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}