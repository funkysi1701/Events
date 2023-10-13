namespace EventsTest
{
    public class RabbitMqOptions
    {
        public string Host { get; set; }

        public int Port { get; set; } = 5672;


        public string User { get; set; } = "guest";


        public string Password { get; set; } = "guest";


        public string VHost { get; set; } = "/";

        public Uri ToHostUri()
        {
            if (string.IsNullOrWhiteSpace(VHost))
            {
                VHost = "/";
            }

            Uri uri = new Uri(Host);
            if (!string.IsNullOrWhiteSpace(VHost))
            {
                uri = new Uri(uri, VHost);
            }

            UriBuilder uriBuilder = new UriBuilder(uri);
            if (Port > 0)
            {
                uriBuilder.Port = Port;
            }

            return uriBuilder.Uri;
        }
    }
}
