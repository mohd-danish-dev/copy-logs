using System;
namespace CopyLogs
{
	public static class Constants
	{
        public static readonly string AWS_ACCESS_KEY_ID = "ACC_ESS";

        public static readonly string AWS_SECRET_KEY = "SEC_RET";   

        public static readonly string SOURCE_S3_BUCKET_NAME = "SOURCE_S3_BUCKET_NAME";

        public static readonly string TARGET_S3_BUCKET_NAME = "TARGET_S3_BUCKET_NAME";

        public static readonly string LAST_MODIFIED_MINUTES = "LAST_MODIFIED_MINUTES";

        public static readonly string SOURCE_S3_DIRECTORIES = "SOURCE_S3_DIRECTORIES";

        public static readonly string SOURCE_S3_FILE_PREFIXES = "SOURCE_S3_FILE_PREFIXES";


        public static readonly string DEFAULT_SOURCE_S3_BUCKET = "c706-sftp-s3";

        public static readonly string DEFAULT_TARGET_S3_BUCKET = "mstr-logs-us-east-1";

        public static readonly int DEFAULT_LAST_MODIFIED = 120;

        public static readonly string[] DEFAULT_S3_DIRECTORIES = new string[]
        {
            "env-311840/logs/env-311840laio1use1/Web/",
            "env-311840/logs/env-311840laio1use1/Intelligenceserver/"
        };

        public static readonly string[] DEFAULT_FILE_PREFIXES = new string[]
        {
            "AuthTask",
            "MSTRLog",
            "Auth_Trace",
            "DSSErrors"
        };


    }
}

