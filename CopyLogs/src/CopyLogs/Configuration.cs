using System;
using Amazon.Lambda.Core;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace CopyLogs
{
	public class Configuration
	{
		public string AccessKey { get; set; }

		public string SecretKey { get; set; }

		public string SourceS3BucketName { get; set; }

		public string TargetS3BucketName { get; set; }

		public int LastModifiedMinutes { get; set; }

		public string[] Directories { get; set; }

		public string[] FilePrefixes { get; set; }


		public static Configuration Create(ILambdaLogger logger)
		{
			var config = new Configuration();

            
            config.AccessKey = GetEnvString(Constants.AWS_ACCESS_KEY_ID);
            config.SecretKey = GetEnvString(Constants.AWS_SECRET_KEY);
            config.SourceS3BucketName = GetEnvString(Constants.SOURCE_S3_BUCKET_NAME, Constants.DEFAULT_SOURCE_S3_BUCKET);
            config.TargetS3BucketName = GetEnvString(Constants.TARGET_S3_BUCKET_NAME, Constants.DEFAULT_TARGET_S3_BUCKET);

            config.Directories = GetEnvString(Constants.SOURCE_S3_DIRECTORIES)
                                ?.Split(",")
                                ?? Constants.DEFAULT_S3_DIRECTORIES;

            config.FilePrefixes = GetEnvString(Constants.SOURCE_S3_FILE_PREFIXES)
                                ?.Split(",")
                                ?? Constants.DEFAULT_FILE_PREFIXES;

            if (!int.TryParse(GetEnvString(Constants.LAST_MODIFIED_MINUTES), out int value))
                value = Constants.DEFAULT_LAST_MODIFIED;

            config.LastModifiedMinutes = value;

            if (string.IsNullOrEmpty(config.AccessKey)
                || string.IsNullOrEmpty(config.SecretKey)
                || string.IsNullOrEmpty(config.SourceS3BucketName)
                || string.IsNullOrEmpty(config.TargetS3BucketName)
                || config.Directories.Length < 1
                || config.FilePrefixes.Length < 1)
            {
                var variables = new string[]
                {
                    Constants.AWS_ACCESS_KEY_ID,
                    Constants.AWS_SECRET_KEY,
                };

                logger.LogError($"Missing required enviroment variable. {string.Join(", ", variables)}");
                throw new InvalidOperationException("Missing environment variable");
            }
            
            return config;
		}


        private static string GetEnvString(string key, string fallback = null) =>
            Environment.GetEnvironmentVariable(key) ?? fallback;
	}
}

