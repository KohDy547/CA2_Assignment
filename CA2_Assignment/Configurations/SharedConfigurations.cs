namespace CA2_Assignment.Configurations
{
    public class App_SharedSettings
    {
        public string Aws_CredentialsPath { get; set; }
        public string Aws_RdsConnection { get; set; }

        public string AuthMsg_SendGridUser { get; set; }
        public string AuthMsg_SendGridKey { get; set; }
        public string AuthMsg_ServerEmail { get; set; }
        public string AuthMsg_ServerEmailName { get; set; }
    }
}
