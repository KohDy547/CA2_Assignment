namespace CA2_Assignment.Configurations
{
    public class Csc_StripeSettings
    {
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }

        public string PremiumSubscriptionCost { get; set; }
    }
    public class Csc_GoogleAuthSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
    public class Csc_AwsS3Settings
    {
        public string Profile { get; set; }
        public string Region { get; set; }
        public string Bucket { get; set; }

        public string Talents_FileKey { get; set; }
        public string Talents_ImgBaseUrl { get; set; }
    }
}
