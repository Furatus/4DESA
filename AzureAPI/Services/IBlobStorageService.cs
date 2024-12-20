﻿namespace AzureAPI.Services;

public interface IBlobStorageService
{
    public string uploadFileToAzureBlob(IFormFile file, Guid postId);

    public void deleteFileFromAzureBlob(string Uri);
}