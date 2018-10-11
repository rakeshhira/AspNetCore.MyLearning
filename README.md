# AspNetCore.MyLearning
I started this repository to document my learning experience with using Visual Studio 2017 to 
- Building AspNetCore based Microservices Using Docker 
- Using selenium & xUnit for Behavior Driven Development 
- Using Postgres, Redis for data storage
- Using Rabbitmq for asynchronous message queuing

[Visual Studio Tools for Docker with ASP.NET Core](./VsToolsForDocker/README.md)

[Branch: Add-Swagger-Support: ASP.NET Core Web API help pages with Swagger / Open API](https://github.com/rakeshhira/AspNetCore.MyLearning/blob/add-swagger-support/SwaggerSupport/README.md)

[Branch: Integrate-rabbitmq: ASP.NET Core Web API with rabbit mq](https://github.com/rakeshhira/AspNetCore.MyLearning/blob/integrate-rabbitmq/)

# Integrate RabbitMQ

My goal was to integrate rabbitmq with the three web api(s) that had been created in [Branch: Add-Docker-Compose](https://github.com/rakeshhira/AspNetCore.MyLearning/blob/add-docker-compose/AspNetCoreWebApi1/README.md).

Specifically, I wanted to implement following workflow
- Send message to AspNetCoreWebApi1
- Set up AspNetCoreWebApi1 to issue receiept of message after message has been successfully persisted but yet not processed by AspNetCoreWebApi2 and AspNetCoreWebApi3
- Check status of message processing by contacting AspNetCoreWebApi1

I used following articles to learn rabbitmq
- [AMQP 0-9-1 Model](http://www.rabbitmq.com/tutorials/amqp-concepts.html)
- [RabbitMQ Tutorials](http://www.rabbitmq.com/getstarted.html)

I used following articles to learn logging & dependency injection in dotnet core
- [DotNetCore Logging](https://www.blinkingcaret.com/2018/02/14/net-core-console-logging/)
- [Dependency Injection in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1)
- [Dependency Injection with .NET Core](https://csharp.christiannagel.com/2016/06/04/dependencyinjection/)

I used following articles to learn how to host rabbitmq in AspNetCore
- [setup-rabbitmq-consumer-in-asp-net-core-application](https://stackoverflow.com/questions/43609345/setup-rabbitmq-consumer-in-asp-net-core-application)

# Concepts I learned

## AMQP 0-9-1 Protocol
RabbitMQ speaks [multiple protocols](http://www.rabbitmq.com/protocols.html).  AMQP 0-9-1 is one such protocol for messaging.

Concept | Notes | 
------- |-------|
AMQP 0-9-1 Model | AMQP 0-9-1 (Advanced Message Queing Protocol) is a messaging protocal that enables conforming client applications to communicate with conforming messaging middleware brokers. Messages are published to *exchanges*, which are often compared to post offices or mailboxes.  Exchanges then distribute message copies to *queues* using rules called *bindings*.  The AMQP brokers either deliver messages to consumers subscribed to queues, or consumers fetch/pull messages from queues on demand.
Post Office Analogy | RabbitMQ can be thought as a post office: when you put the mail that you want posting in a post box, you can be sure that Mr./Ms. Mailperson will eventually deliver the mail to the receipient.  In this analogy, RabbitMQ is a post box, a post office and a postman.  

`AMQP 0-9-1 Entities:`
Queues, exchanges and bindings are collectively referred as AMQP entities.

Concept | Notes | 
------- |-------|
Producer | A program that sends message is a producer
Consumer | A program that waits to receive messages.
Broker | A program that receive messages from publishers and route them to consumers
Queue | A queue is the name for a post box that is used to store messages.
Exchange | Exchanges are where messages are sent.  Exchange takes a message and route it into zero or more queues.
Bindings | Bindings are rules used by the exchange to copy messages to queues.

`AMQP 0-9-1 Programmable Protocol:`
The entities and routing schemes are primarily defined by the applications themselves and not a broker administrator.  So declaring queues, exchanges, defining bindings between them, subscribe to queue etc. are all programmable.

Concept | Notes | 
------- |-------|
Exchange Types | AMQP 0-9-1 provides fourt exchange types: Direct, Fanout, Topic, Headers
Exchange Attributes | In addition to exchange type, an exchange is declared with number of attributes like Name, Durability, Auto-delete, Arguments
Default Exchange | The default exchange is a direct exchange with no name pre-declared by the broker.  It has a special property where every queue created is automatically bound to a routing key which is same as the queue name.
Direct Exchange | A direct exchange delivers messags to queues based on the message routing key.  Ideal of unicast routing of messages.  It is often used to distribute tasks between multiple workers in a round robin manner.
Fanout Exchange | A fanout exchange routes message to all queues that are bound to it and the routing key is ignored.  Ideal of broadcast routing of messages.
Topic Exchange | Topic exchange route messages to one or more queues based on matching between a message routing key and the pattern that was used to bind a queue to an exchange.  Ideal of pub/sub pattern to multicast routing of messages.
Headers Exchange | A headers exchange is designed for routing on multiple attributes that are more easily expressed as message headers than a routing key.
Queue Name | Applications may pick queue names or ask the broker to generate a name for them.
Queue Durability | Durable queues are persisted to disk and thus survive broken restarts.  Durability of a queue does not make *messages* that are routed to that queue durable.  Only *persistent* messages are recovered upon broker restart. 
Message Durability | Durable messages are persisted to disk and thus survive broken restarts. 

`AMQP 0-9-1 Methods:`
AMQP 0-9-1 is structured as a number of *methods*.  AMQP methods are groupped into *classes*.

Concept | Notes | 
------- |-------|
*exchange* class | Has methods: exchange.declare, exchange.declare-ok, exchange.delete, exchange.delete-ok
*queue* class | queue.declare, queue.declare-ok

`AMQP Connections, Channels, Virtual Hosts:`

Concept | Notes | 
------- |-------|
Connections |  Connections are typically long-lived.  AMQP is an application level protocol that uses TCP for reliable delivery.  AMQP connections use authentication and can be protected using TLS(SSL).  
Channels | Some applications need multiple connections to an AMQP broker.  AMQP 0-9-1 connections are multiplexed and channels can be thought as lightweight connections that share a single TCP connection.
Virtual Hosts | Virtual hosts make it possible to host multiple isolated environments (users, exchanges, queues, etc.) by single broker.

## Consumer Acks & Publisher Confirms
Since protocol methods (messages) sent are not guaranteed to reach the peer or be successfully processed by it, both publishers and consumers need a mechanism for delivery and processing confirmation.

Concept | Notes | 
------- |-------|
Consumer Delivery Acks | Using `basic.consume` or `basic.get` a consuming application can register for either automatic ack or manual ack.  In automatic ack mode, a message is considered successfully delivered immediately after it is sent.  Ideal of higher throughput.  This is 'fire-and-forget' and is considered unsafe.
Delivery Tags | Messages delivered carry a *delivery tag*, which uniquely identifies the delivery on a channel.  Delivery tags are scoped per channel
`basic.ack` |  `basic.ack` is used for explicit positive acknowledgements.  This instructs RabbitMQ to record a message as delivered and can be discarded.
`basic.nack` |  `basic.nack` is used for explicit negative acknowledgements.  
`basic.reject` |  `basic.reject` is used for negative acknowledgements.  This instructs RabbitMQ to record a message was not processed but still should be deleted.
requeuing | Consumer can requeue a message when using `basic.reject` and `basic.nack`.  
number of redeliveries | Requeuing can lead of requeue/redelivery loop.  Consumer implementations can track the number of redeliveries and reject messages or schedule requeuing after a delay.
`basic.qos` | Due to async protocol, there can be more than one message "in flight" on a channel at any given moment. The number of unacknowledged messages can be capped by using `basic.qos` method.
`redeliver` | When manual ack are used, any delivery that was not acked is automatically requeued when the channel is closed.  Redeliveries have special boolean property `redeliver`, set to `true`.  Consumer should handle messages as idempotence.
Publisher Confirms | Publisher confirm mechanism ensures that message was saved by RabbitMQ.  
`confirm.select` | To enable publisher confirms, client sends `confirm.select` method.  This puts the channel in `confirm` mode.  In this mode, both the client and broker count messages (starts at 1).  The broker confirms messages by sending `basic.ack` with `delivery-tag` set to sequence number of confirmed message.

## RabbitMQ Management
[RabbitMQ Management](http://www.rabbitmq.com/management.html) frontend and HTTP API is implemented as a plugin on top of AMQP 0-9-1 protocol.

## RabbitMQ .NET Client && Getting Started Tutorials
[RabbitMQ .NET Client](http://www.rabbitmq.com/dotnet.html) is an implementation of AMQP 0-9-1 client library for C# .

The RabbitMQ [Getting Started](http://www.rabbitmq.com/getstarted.html) tutorials were the key to exploring RabbitMG .NET client library and understanding various ways of solving problems using RabbitMQ.

After going through first 3 tutorials I realized there was lot of repeatition.  So I 
- abstracted the configuration information into a separate json file.  I 
- wrote a configuration class that reads the JSON.
- wrote a mgr class that initializes rabbitmq client based configuration.

The JSON file looks like following for [Tutorial1 Sender](http://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html)
```json
{
	"rabbitmq": {
		"connection": {
			"hostname": "localhost",
			"username": "rabbitmq",
			"password": "rabbitmq"
		},
		"queue": {
			"name": "hello",
			"exclusive": "0",
			"durable": "0",
			"autoDelete": "1",
			"arguments": []
		},
		"basicPublish": {
			"routingKey": "hello"
		}
	}
}
```

The Sender Code looked like following:

```C#
using System;
using System.Text;
using RabbitMQ.Client;
using TutorialCommon;

namespace Tutorial1_Sender
{
	class Program
	{
		static void Main()
		{
			RabbitMQClientMgr rabbitMQClientMgr = TutorialCommonHelper.Initialize();

			string message = "Hello World!!";
			var body = Encoding.UTF8.GetBytes(message);
			rabbitMQClientMgr.BasicPublish(body);

			Console.WriteLine(" [x] Sent {0}", message);

			Console.WriteLine(" Press [enter] to exit.");
			Console.ReadLine();

			rabbitMQClientMgr.Uninitialize();
		}
	}
}
```

I was able to hide most of the plumbing work into RabbitMQClientMgr's Initialize method.

```C#
		public void Initialize()
		{
			var connectionFactory = new ConnectionFactory();
			connectionFactory.HostName = _rabbitMQConfig.Connection.HostName;
			connectionFactory.UserName = _rabbitMQConfig.Connection.UserName;
			connectionFactory.Password = _rabbitMQConfig.Connection.Password;

			Connection = connectionFactory.CreateConnection();
			_logger.LogInformation($"Connection created {Connection}");
			Model = Connection.CreateModel();
			_logger.LogInformation($"Model created {Model}");

			IExchangeConfig exchangeConfig = _rabbitMQConfig.Exchange;
			_logger.LogInformation($"exchangeConfig: {JsonConvert.SerializeObject(exchangeConfig)}");

			if (exchangeConfig != null)
			{
				Model.ExchangeDeclare(
						exchange: exchangeConfig.Name,
						type: exchangeConfig.Type,
						autoDelete: exchangeConfig.AutoDelete.Value,
						durable: exchangeConfig.Durable.Value,
						arguments: exchangeConfig.Arguments);
				_logger.LogInformation($"ExchangeDeclare completed.  exchange={exchangeConfig.Name}, type={exchangeConfig.Type}, autoDelete={exchangeConfig.AutoDelete}, durable={exchangeConfig.Durable}, arguments={exchangeConfig.Arguments}");
			}

			if (_rabbitMQConfig.Queue != null)
			{
				IQueueConfig queueConfig = _rabbitMQConfig.Queue;
				_logger.LogInformation($"queueConfig: {JsonConvert.SerializeObject(queueConfig)}");

				var queueDeclareOk = Model.QueueDeclare(
					queue: queueConfig.Name,
					exclusive: queueConfig.Exclusive.Value,
					autoDelete: queueConfig.AutoDelete.Value,
					durable: queueConfig.Durable.Value,
					arguments: queueConfig.Arguments);
				_logger.LogInformation($"QueueDeclare completed.  queue={queueConfig.Name}, exclusive={queueConfig.Exclusive}, autoDelete={queueConfig.AutoDelete}, durable={queueConfig.Durable}, arguments={queueConfig.Arguments}, queueDeclareOk ={queueDeclareOk}");

				if (queueConfig.Name.Length == 0)
				{
					queueConfig.SetAutoGeneratedQueueName(queueDeclareOk.QueueName);
				}
			}

			if (_rabbitMQConfig.BasicQosConfig != null)
			{
				Model.BasicQos(
					prefetchSize: _rabbitMQConfig.BasicQosConfig.PrefetchSize.Value,
					prefetchCount: _rabbitMQConfig.BasicQosConfig.PrefetchCount.Value,
					global: _rabbitMQConfig.BasicQosConfig.Global.Value);
			}

			if (_rabbitMQConfig.QueueBinding != null)
			{
				_logger.LogInformation($"queueBindingConfig: {JsonConvert.SerializeObject(_rabbitMQConfig.QueueBinding)}");

				foreach (string routingKey in _rabbitMQConfig.QueueBinding.RoutingKeys)
				{
					QueueBind(routingKey: routingKey);
				}
			}
		}
```

Logger and configuration was injected into the mgr

```c#
		public RabbitMQClientMgr(ILogger<RabbitMQClientMgr> logger, IRabbitMQConfig rabbitMQConfig)
		{
			_logger = logger;
			_rabbitMQConfig = rabbitMQConfig;

			_logger.LogInformation($"{nameof(RabbitMQClientMgr)} constructed");
		}
```

I also added some helper method in mgr to simplify the information necessary publish/consume

```c#
		public void BasicPublish(byte[] body)
		{
			BasicPublish(_rabbitMQConfig.BasicPublishConfig.RoutingKey, body);
		}

		public void BasicPublish(string routingKey, byte[] body)
		{
			string exchange = _rabbitMQConfig.Exchange != null ? _rabbitMQConfig.Exchange.Name : "";
			IBasicProperties basicProperties = null;
			if (_rabbitMQConfig.BasicPublishConfig != null && _rabbitMQConfig.BasicPublishConfig.BasicProperitesConfig != null)
			{
				basicProperties = Model.CreateBasicProperties();
				basicProperties.Persistent = _rabbitMQConfig.BasicPublishConfig.BasicProperitesConfig.Persistent.Value;
			}
			Model.BasicPublish(
				exchange: exchange,
				routingKey: routingKey,
				mandatory: false,
				basicProperties: basicProperties,
				body: body);
			_logger.LogInformation($"BasicPublish completed: exchange={exchange}, routingKey={routingKey}, mandatory=false, basicProperties=null,body={body}");
		}

		public void QueueBind(string routingKey)
		{
			Model.QueueBind(
				queue: _rabbitMQConfig.Queue.Name,
				exchange: _rabbitMQConfig.Exchange.Name,
				routingKey: routingKey);
			_logger.LogInformation($"QueueBind completed: Name={_rabbitMQConfig.Queue.Name}, exchange={_rabbitMQConfig.Exchange.Name} routingKey={routingKey}");
		}

		public void BasicConsume()
		{
			bool autoAck = _rabbitMQConfig.BasicConsumeConfig != null ? _rabbitMQConfig.BasicConsumeConfig.AutoAck.Value : true;
			Model.BasicConsume(
				queue: _rabbitMQConfig.Queue.Name,
				autoAck: autoAck,
				consumer: EventingBasicConsumer);
			_logger.LogInformation($"BasicConsume completed: queue={_rabbitMQConfig.Queue.Name}, autoAck={autoAck}, consumer={EventingBasicConsumer}");
		}

		public void BasicAck(ulong deliveryTag, bool multiple)
		{
			Model.BasicAck(deliveryTag: deliveryTag, multiple: multiple);
			_logger.LogInformation($"BasicAck completed: deliveryTag={deliveryTag}, multiple={multiple}");
		}

		public EventingBasicConsumer EventingBasicConsumer
		{
			get
			{
				if (_eventingBasicConsumer == null)
				{
					_eventingBasicConsumer = new EventingBasicConsumer(Model);
				}
				return _eventingBasicConsumer;
			}
		}
```

The full JSON file looks like following:

```json
{
	"rabbitmq": {
		"connection": {
			"hostname": "required, name of the rabbit mq host goes here",
			"username": "required, user name of the rabbit mq host goes here",
			"password": "required, password of the rabbit mq user host goes here"
		},
		"exchange": {
			"name": "exchange section is optional.  exchange name can be empty to use direct exchange",
			"type": "required, can be direct|fanout|topic",
			"durable": "optional, can be 0|1, true|false, yes|no, on|off. defaults to false",
			"autoDelete": "optional, can be 0|1, true|false, yes|no, on|off. defaults to true",
			"arguments": []
		}
		"queue": {
			"name": "queue section is optional.  queue name can be empty to allow the broker to auto generate an unique name",
			"exclusive": "optional, can be 0|1, true|false, yes|no, on|off. defaults to false",
			"durable": "optional, can be 0|1, true|false, yes|no, on|off. defaults to false",
			"autoDelete": "optional, can be 0|1, true|false, yes|no, on|off. defaults to true",
			"arguments": ["not supported"]
		},
		"queueBinding": {
			"routingKeys": [ "queue binding section is optional.  Can be used to pre-define the routing key for the exchange and queue"]
		}
		"basicPublish": {
			"routingKey": "basicPublish is required only when you want to pre-define the routing key"
			"basicProperties": {
				"persistent": "basicProperties section is optional.  persistent is optional and can be 0|1, true|false, yes|no, on|off. defaults to false",
			}
		}
		"basicQos": {
			"prefetchSize": "basicQos is optional.  prefetchSize is a unit value",
			"prefetchCount": "a ushort value",
			"global": "optional, can be 0|1, true|false, yes|no, on|off. defaults to false",
		},
		"basicConsume": {
			"autoAck": "basicPublish is optional.  autoAck is optional, can be 0|1, true|false, yes|no, on|off. defaults to true"
		}
	}
}
```
