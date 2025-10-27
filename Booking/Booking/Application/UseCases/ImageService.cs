using Booking.Application.Interfaces;
using Microsoft.Extensions.Options;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.Runtime;
using Booking.BuildingBlocks.Core;
namespace Booking.Application.UseCases
{
    public class ImageService : IImageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName = "uks-2024";

        public ImageService(IOptions<AwsSettings> awsSettings, ILogger<ImageService> logger)
        {
            var awsConfig = awsSettings.Value;

            var credentials = new BasicAWSCredentials(awsConfig.AccessKey, awsConfig.SecretKey);
            _s3Client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(awsConfig.Region));
        }

        public async Task<string> GetImageUrl(string fileName)
        {
            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
                    Key = fileName,
                    Expires = DateTime.UtcNow.AddMinutes(60)
                };

                string preSignedUrl = _s3Client.GetPreSignedURL(request);
                return preSignedUrl;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task UploadImage(string imageName, Stream fileStream)
        {
            try
            {
                var transferUtility = new TransferUtility(_s3Client);
                await transferUtility.UploadAsync(fileStream, _bucketName, imageName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error uploading file to S3 with name: {ImageName}", imageName);
            }
        }

        public async Task DeleteImage(string filePath)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = filePath
                };

                await _s3Client.DeleteObjectAsync(deleteObjectRequest);
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine("Amazon S3 error while deleting file with path: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting file from S3 with path: {FilePath}", filePath);
            }
        }

    }
}
