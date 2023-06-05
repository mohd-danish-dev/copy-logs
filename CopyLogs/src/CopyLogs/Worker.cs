using System;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using System.Net;

namespace CopyLogs
{
	public class Worker
	{
		#region Public Method

        public static async Task CopyLogsToS3Bucket(ILambdaContext context)
        {
			var config = Configuration.Create(context.Logger);
            var wroker = new Worker(context.Logger, config);
            await wroker.Run();
        }

        #endregion


        #region Private Fields

        private ILambdaLogger _logger;

		private AmazonS3Client _s3Client;

		private AmazonS3Client _mstrS3Client;

		private Configuration _config;

        #endregion

        private Worker(ILambdaLogger logger, Configuration config)
		{
			_logger = logger;
			_config = config;
			_s3Client = new AmazonS3Client();
			_mstrS3Client = new AmazonS3Client(config.AccessKey, config.SecretKey);
		}

		private async Task Run()
		{
			var fileNames = await GetFileListFromMstrS3Bucket();
            var request = new GetObjectRequest
			{
				BucketName = _config.SourceS3BucketName
			};

			_logger.LogLine($"Files queued: {string.Join(",", fileNames)}.");

            foreach (var key in fileNames)
			{
				request.Key = key;

				var response = await _mstrS3Client!.GetObjectAsync(request);

				if(response.HttpStatusCode != HttpStatusCode.OK)
				{
					_logger.LogLine($"Failed to get file from mstr s3 bucket. key = {key}");
					continue;
				}

                try
                {
					using var stream = new MemoryStream();
					await response.ResponseStream.CopyToAsync(stream);

                    var putRequest = new PutObjectRequest
                    {
                        BucketName = _config.TargetS3BucketName,
                        Key = key,
						InputStream = stream
                    };

                    var putResponse = await _s3Client.PutObjectAsync(putRequest);
					stream.Close();

                    if (putResponse.HttpStatusCode == HttpStatusCode.OK)
                        _logger.LogLine($"Successfully upload file on s3. key = ${key}");
					else
						_logger.LogLine($"Failed to upload file on s3.Key = ${key}");
                }
                catch (Exception ex)
                {
                    _logger.LogLine($"Failed to upload file on s3. key = {key} .{ex.Message} {ex.StackTrace}");
                }
            }
        }

        private async Task<List<string>> GetFileListFromMstrS3Bucket()
		{
			var fileList = new List<string>();

			var lastModifiedThreshold = DateTime.Now.AddMinutes(-1 * _config.LastModifiedMinutes);

			foreach (var dir in _config.Directories)
			{
				var request = new ListObjectsV2Request
				{
					BucketName = _config.SourceS3BucketName,
					Prefix = dir,
					MaxKeys = 1_000
				};


				var response = await _mstrS3Client!.ListObjectsV2Async(request);

				if(response.HttpStatusCode != HttpStatusCode.OK)
				{
					_logger.LogError($"Failed to get the list of file from {dir} directory.");
					continue;
				}

				var validFileNames = response.S3Objects
					.Where(file =>
					{
						var filename = Path.GetFileName(file.Key);
						return _config.FilePrefixes.Any(prefix => filename.StartsWith(prefix));
					})
                    .Where(file => file.LastModified > lastModifiedThreshold)
                    .Where(file => Path.GetExtension(file.Key).Equals(".log"))
                    .Select(file => file.Key);

				fileList.AddRange(validFileNames);
			}

			return fileList;
		}

		private async Task UploadToS3Bucket(string key, Stream stream)
		{
			try
			{
				var request = new PutObjectRequest
				{
					BucketName = _config.TargetS3BucketName,
					Key = key
				};

				await stream.CopyToAsync(request.InputStream);

				var response = await _s3Client.PutObjectAsync(request);

				if(response.HttpStatusCode == HttpStatusCode.OK)
				{
					_logger.LogLine($"Successfully upload file on s3. key = ${key}");
					return;
				}

				_logger.LogLine($"Failed to upload file on s3.Key = ${key}");
			}
			catch (Exception ex)
			{
				_logger.LogLine($"Failed to upload file on s3. key = {key} .{ex.Message} {ex.StackTrace}");
			}
		}
	}
}

