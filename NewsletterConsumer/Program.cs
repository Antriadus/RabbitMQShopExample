using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NewsletterConsumerNamespace
{
    class Program
    {
        private const string _queue = "NewsletterQueue";
        private const int _interval = 1000;

        static void Main()
        {
            try
            {
                using (var connection = ConnectionFactoryNamespace.ConnectionFactory.GetConnection())
                using (var channel = connection.CreateModel())
                {
                    //ustalamy,że będzie pobierał jeden item z kolejki i przetwarzał
                    channel.BasicQos(0, 1, false);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        //deserializacja
                        var message = UtilsNamespace.Utils.JsonStringToMessage(Encoding.UTF8.GetString(ea.Body));
                        Thread.Sleep(_interval);
                        Console.WriteLine($"Wysłałem {message.MessageValue} do {message.ReceiverAddress}");
                        //potwierdzenie,że wykonano zadanie
                        channel.BasicAck(ea.DeliveryTag, false);
                    };
                    //połączenie kolejki z konsumentem
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

    }
}

