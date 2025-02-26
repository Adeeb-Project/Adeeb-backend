using System;

namespace AdeebBackend.Services;

using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using adeeb.Models; // Adjust based on your namespace

public class S3Service
{
    private readonly AwsSettings _awsSettings;
    private readonly IAmazonS3 _s3Client;

    public S3Service(IOptions<AwsSettings> awsOptions)
    {
        _awsSettings = awsOptions.Value;
        _s3Client = new AmazonS3Client(
            _awsSettings.AccessKey,
            _awsSettings.SecretKey,
            RegionEndpoint.GetBySystemName(_awsSettings.Region)
        );
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty.");

        // Generate a unique file name
        var key = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Reset stream position

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = memoryStream,
                Key = key,
                BucketName = _awsSettings.BucketName,
                ContentType = file.ContentType,
            };

            var fileTransferUtility = new TransferUtility(_s3Client);
            await fileTransferUtility.UploadAsync(uploadRequest);
        }

        // Return the public URL of the uploaded file
        return $"https://{_awsSettings.BucketName}.s3.amazonaws.com/{key}";
    }

    public async Task<string> UploadCompanyLogoAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty.");

        // Define the folder prefix for your S3 bucket
        string folderPrefix = "company-logos/";

        // Generate a unique file key with the folder prefix
        var key = $"{folderPrefix}{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Reset stream position

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = memoryStream,
                Key = key,
                BucketName = _awsSettings.BucketName,
                ContentType = file.ContentType,
            };

            var fileTransferUtility = new TransferUtility(_s3Client);
            await fileTransferUtility.UploadAsync(uploadRequest);
        }

        // Return the public URL of the uploaded file
        return $"https://{_awsSettings.BucketName}.s3.amazonaws.com/{key}";
    }

}