using System;
using System.Threading;
using ModelsNamespace;
using RabbitMQ.Client;
using DatabaseNamespace;
using static UtilsNamespace.Utils;

namespace NewsletterProducerNamespace
{
    class Program
    {
        private const string _exchange = "AccountsExchange";
        private const string _routingKey = "Newsletter";
        public const int _interval = 1000;
        private static readonly string[] _newsletters =
        {
            "Wielkanocny Newsletter",
            "Wakacyjny Newsletter",
            "Bożonarodzeniowy Newsletter",
            "BlackFriday Newsletter",
        };

        static void Main()
        {
            try
            {
                using (var connection = ConnectionFactoryNamespace.ConnectionFactory.GetConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ConfirmSelect();
                    Console.WriteLine($"Będę produkował zadanie wysłania newslettera na adres email co {_interval} ms");
                    int newsletterIndex = 0;
                    while (true)
                    {
                        //umożliwia uzyskaniwanie potwierdzenia o dostarczeniu przesyłki do Exchange
                        channel.ConfirmSelect();

                        //wysłanie newslettera do wszystkich subskrybentów
                        var newsletterSubscribers = Accounts.GetNewsletterSubscribers();
                        foreach (var newsletterSubscriber in newsletterSubscribers)
                        {
                            var message = new Message
                            {
                                ReceiverAddress = newsletterSubscriber.EmailAddress,
                                MessageValue = _newsletters[newsletterIndex]
                            };
                            var body = MessageToJsonString(message);
                            channel.BasicPublish(exchange: _exchange,
                                routingKey: _routingKey,
                                basicProperties: null,
                                body: StringToUtf8(body));
                        }
                        //oczekiwanie aż wszystkie wysyłki dojda do exchange
                        channel.WaitForConfirmsOrDie();
                        //oczekujemy ponieważ newslettery nie są wysyłane cały czas lecz co kilka tygodni
                        Thread.Sleep(_interval);
                        Console.WriteLine($"Utworzenia zadania wysłania {_newsletters[newsletterIndex]} na {newsletterSubscribers.Length} adresów");
                        //zmieniamy index, symuluje to wysyłanie newsletterów różnego typu
                        newsletterIndex++;
                        if (newsletterIndex == _newsletters.Length)
                            newsletterIndex = 0;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
