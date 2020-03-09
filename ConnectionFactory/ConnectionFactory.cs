using RabbitMQ.Client;

namespace ConnectionFactoryNamespace
{
    public static class ConnectionFactory
    {
        /// <summary>
        /// Tworzy instancję ConnectionFactory która zawiera informację o hoście z którym chcemy się połączyć, oraz dane do uwierzytelnienia
        /// </summary>
        /// <returns></returns>
        public static IConnection GetConnection()
        {
            var connectionFactory = new RabbitMQ.Client.ConnectionFactory()
            {
                HostName = "localhost",
                Port = AmqpTcpEndpoint.UseDefaultPort,
                Protocol = Protocols.DefaultProtocol,
                VirtualHost = "/",
                UserName = "guest",
                Password = "guest"
            };
            return connectionFactory.CreateConnection();
        }

    }
}
