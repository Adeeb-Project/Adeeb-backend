using System;

namespace adeeb.Models
{
    public class AwsSettings
    {
        public AwsSettings()
        {
            BucketName = string.Empty;
            Region = string.Empty;
            AccessKey = string.Empty;
            SecretKey = string.Empty;
        }

        public string BucketName { get; set; }
        public string Region { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }
}