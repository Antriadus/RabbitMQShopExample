using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace RPCClientNamespace
{
    public class RpcClient
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _replyQueueName;
        private readonly EventingBasicConsumer _consumer;
        private readonly BlockingCollection<string> _respQueue = new BlockingCollection<string>();
        private readonly IBasicProperties _props;

        private const string _routingKey = "Reports_RPC_Queue";

        public RpcClient()
        {
            _connection = ConnectionFactoryNamespace.ConnectionFactory.GetConnection();
            _channel = _connection.CreateModel();
            _replyQueueName = _channel.QueueDeclare().QueueName;
            _consumer = new EventingBasicConsumer(_channel);

            _props = _channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            _props.CorrelationId = correlationId;
            _props.ReplyTo = _replyQueueName;

            _consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    _respQueue.Add(response);
                }
            };
        }

        public Dictionary<string, int> CallForAccountReport()
        {
            _channel.BasicPublish(
                exchange: "",
                routingKey: _routingKey,
                basicProperties: _props);
            _channel.BasicConsume(
                consumer: _consumer,
                queue: _replyQueueName,
                autoAck: true);
            var resultJSON = _respQueue.Take();
            return JsonConvert.DeserializeObject<Dictionary<string, int>>(resultJSON);
        }
        public Dictionary<string, int> CallForAccountReport(params string[] websites)
        {
            _channel.BasicPublish(
                exchange: "",
                routingKey: _routingKey,
                basicProperties: _props,
                body: UtilsNamespace.Utils.StringToUtf8(string.Join(",", websites)));
            _channel.BasicConsume(
                consumer: _consumer,
                queue: _replyQueueName,
                autoAck: true);
            var resultJSON = _respQueue.Take();
            return JsonConvert.DeserializeObject<Dictionary<string, int>>(resultJSON);
        }
        public void Close()
        {
            _connection.Close();
        }
    }
}