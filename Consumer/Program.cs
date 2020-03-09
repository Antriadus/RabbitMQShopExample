using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using DatabaseNamespace;

namespace NewAccountConsumerNamespace
{
    class Program
    {
        private const string _queue = "NewAccountQueue";
        private const int _interval = 1500;
        static void Main()
        {
            try
            {
                Console.WriteLine("Będę dodawał nowe konta użytkowników do bazy danych");
                using (var connection = ConnectionFactoryNamespace.ConnectionFactory.GetConnection())
                using (var channel = connection.CreateModel())
                {
                    //ustalamy,że będzie pobierał jeden item z kolejki i przetwarzał
                    channel.BasicQos(0, 1, false);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        HandleReceivedEvent(ea, channel);
                    };
                    //podłączenie konsumenta do kolejki
                    channel.BasicConsume(queue: _queue,
                       consumer: consumer);
                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void HandleReceivedEvent(BasicDeliverEventArgs ea, IModel channel)
        {
            //desierializacja pobranych danych
            var account = UtilsNamespace.Utils.JsonStringToAccount(Encoding.UTF8.GetString(ea.Body));
            //dodanie do bazy danych 
            Accounts.PostAccount(account);
            //udawanie, że coś długo liczymy
            Thread.Sleep(_interval);

            Console.WriteLine($"Dodałem konto dla: {account.EmailAddress}");
            //potwierdzenie,że wykonano zadanie
            channel.BasicAck(ea.DeliveryTag, false);
        }
    }
}

