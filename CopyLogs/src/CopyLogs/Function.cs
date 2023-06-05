using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CopyLogs;

public class Function
{
    public async Task FunctionHandler(ILambdaContext context)
    {
        await Worker.CopyLogsToS3Bucket(context);
    }

}

