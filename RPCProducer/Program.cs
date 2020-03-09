using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static System.String;

namespace RPCServerNamespace
{
    class Program
    {
        private const string _queue = "Reports_RPC_Queue";
        private const int _sleepTime = 1000;

        static void Main()
        {
            using (var connection = ConnectionFactoryNamespace.ConnectionFactory.GetConnection())
            using (var channel = connection.CreateModel())
            {
                Console.WriteLine("Będę odpowiadał na rządania RPC dla generowania raportów");

                channel.QueueDeclare(queue: _queue, durable: false,
                  exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: _queue, consumer: consumer);

                consumer.Received += (model, ea) =>
                {
                    HandleRecivedEvent(ea, channel);
                };

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static void HandleRecivedEvent(BasicDeliverEventArgs ea, IModel channel)
        {
            Dictionary<string, int> response = null;
            var props = ea.BasicProperties;
            var replyProps = channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            try
            {
                //na podstawie danych z body będziemy generować różne raporty
                var bodyString = Encoding.UTF8.GetString(ea.Body);
                response = IsNullOrEmpty(bodyString)
                    ? AddressesReport(true)
                    : AddressesReport(bodyString.Split(',').ToList());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                var responseBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
                channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                    basicProperties: replyProps, body: responseBytes);
                channel.BasicAck(deliveryTag: ea.DeliveryTag,
                    multiple: false);
                Console.WriteLine($"Odpowiedziałem na {ea.DeliveryTag}");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Zwraca słownik który zawiera informację o stronie i ilości użytkowników. np wp.pl 10, interia.poczta.pl 15
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, int> AddressesReport(bool showInfo = false)
        {
            if (showInfo)
                Console.WriteLine("Zaczynam generować raport dla wszystkich użytkowników");
            var accounts = DatabaseNamespace.Accounts.GetAccounts();
            var result = accounts.GroupBy(x => x.EmailAddress.Split('@')[1], x => x).Select(y => new
            {
                Prefix = y.First().EmailAddress.Split('@')[1],
                Count = y.Count()
            }).ToDictionary(x => x.Prefix, x => x.Count);
            Thread.Sleep(_sleepTime);
            if (showInfo)
                Console.WriteLine("Skończyłem generować raport dla wszystkich użytkowników");
            return result;
        }

        /// <summary>
        /// Dla podanych stron zwraca słownik który zawiera informację o stronie i ilości użytkowników. np wp.pl 10, interia.poczta.pl 15
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, int> AddressesReport(List<string> websites)
        {
            Console.WriteLine($"Zaczynam generować raport dla {string.Join(",", websites)}");
            var onAllWebsites = AddressesReport();
            var result = new Dictionary<string, int>();
            foreach (var website in websites)
            {
                var quantity = onAllWebsites.SingleOrDefault(x => x.Key.Equals(website)).Value;
                result.Add(website, quantity);
            }
            Thread.Sleep(_sleepTime);
            Console.WriteLine($"Skończyłem generować raport dla {string.Join(",", websites)}");
            return result;
        }
    }
}

