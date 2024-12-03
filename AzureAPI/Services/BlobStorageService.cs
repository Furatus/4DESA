using Azure.Storage.Blobs;

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
            var post = _azureService.GetPostById(PostId);
            if (post == null)
            {
                throw new Exception("Post not found");
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient("mediastorage");
            var fileExtension = Path.GetExtension(file.FileName);
            var blobClient = containerClient.GetBlobClient($"{PostId}{fileExtension}");
            blobClient.UploadAsync(file.OpenReadStream(), true);

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