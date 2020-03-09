using System;
using System.Threading;
using ConnectionFactoryNamespace;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using AccountGeneratorNamespace;
using UtilsNamespace;

namespace NewAccountRequestProducerNamespace
{
    class Program
    {
        private const string _exchange = "AccountsExchange";
        public const string _routingKey = "NewAccount";
        public const int _interval = 1000;
        static void Main()
        {
            try
            {
                Console.WriteLine($"Będę produkował zadania dodania nowego konta co {_interval} ms");
                using (var connection = ConnectionFactoryNamespace.ConnectionFactory.GetConnection())
                using (var channel = connection.CreateModel())
                {
                    //umożliwia potwierdzanie, że przesyłkę dostarczono do Exchange
                    channel.ConfirmSelect();
                    while (true)
                    {
                        //generujemy nowe konto
                        GenerateAndPublishMessage(channel);
                        Thread.Sleep(_interval);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void GenerateAndPublishMessage(IModel channel)
        {
            var newAccount = AccountGenerator.GenerateRandomAccount();
            //serializujemy oraz przekształcamy w tablicę bajtów
            var body = Utils.StringToUtf8(Utils.AccountToJsonString(newAccount));
            channel.BasicPublish(exchange: _exchange,
                routingKey: _routingKey,
                basicProperties: null,
                body: body);
            //oczekujemy na potwierdzenie, że przesyłka dotarła do exchange
            channel.WaitForConfirmsOrDie();
            Console.WriteLine($"Dodałem dla: {newAccount.EmailAddress} i passwordHash: {newAccount.PasswordHash}");
        }
    }
}
