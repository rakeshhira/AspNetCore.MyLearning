# Integrate RabbitMQ

My goal was to integrate rabbitmq with the three web api(s) that had been created in [Branch: Add-Docker-Compose](https://github.com/rakeshhira/AspNetCore.MyLearning/blob/add-docker-compose/AspNetCoreWebApi1/README.md).

Specifically, I wanted to implement following workflow
- Send message to AspNetCoreWebApi1
- Set up AspNetCoreWebApi1 to issue receiept of message after message has been successfully persisted but yet not processed by AspNetCoreWebApi2 and AspNetCoreWebApi3
- Check status of message processing by contacting AspNetCoreWebApi1

I used following article to learn rabbitmq
- [AMQP 0-9-1 Model](http://www.rabbitmq.com/tutorials/amqp-concepts.html)
- [Hello World Tutorial](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html)
- [Worker Queues Tutorial](https://www.rabbitmq.com/tutorials/tutorial-two-dotnet.html)
- [Publish/Subscribe Tutorial](https://www.rabbitmq.com/tutorials/tutorial-three-dotnet.html)
- [Routing Tutorial](https://www.rabbitmq.com/tutorials/tutorial-four-dotnet.html)
- [Topics Tutorial](https://www.rabbitmq.com/tutorials/tutorial-five-dotnet.html)
- [RPC Tutorial](https://www.rabbitmq.com/tutorials/tutorial-RPC-dotnet.html)
- [DotNetCore Logging](https://www.blinkingcaret.com/2018/02/14/net-core-console-logging/)
- [Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1)

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

## RabbitMQ .NET Client
[RabbitMQ .NET Client](http://www.rabbitmq.com/dotnet.html) is an implementation of AMQP 0-9-1 client library for C# .

