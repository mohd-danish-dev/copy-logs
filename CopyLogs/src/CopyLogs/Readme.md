# Requirement
As per requirement we need to pull selected log files from s3 bucket owned by microstrategy to our s3 bucket.
So that we can make the required changes to stream the logs to Splunk.

This lambda functions does following steps.

    1. get the configuration from environment variables.
    2. Pulls the selected files from microstrategy s3 bucket.
    3. Upload the files in step 2 to our s3 bucket.

# How to provide the environment variables

1. AWS_ACCESS_KEY_ID (aws access key as plain text)
2. AWS_SECRET_KEY (aws secret key as plain text)
3. SOURCE_S3_BUCKET_NAME (bucket name of microstrategy's aws a/c as plain text)
4. TARGET_S3_BUCKET_NAME(bucket name where we upload files.)
5. LAST_MODIFIED_MINUTES(specifies difference b/w last modified date of file and time when lambda will run. Default is 120 minutes)
6. SOURCE_S3_DIRECTORIES(Microstrategy's bucket  directory names as comma separated. e.g. day1/user-001/Web/,day1/user-001/Instances/)
7. SOURCE_S3_FILE_PREFIXES(comma separated file prefixes. Lambda copy only those files which starts with prefix name mentioned here.)

