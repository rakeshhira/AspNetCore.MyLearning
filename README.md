# AspNetCore.MyLearning
I started this repository to document my learning experience with using Visual Studio 2017 to 
- Building AspNetCore based Microservices Using Docker 
- Using selenium & xUnit for Behavior Driven Development 
- Using Postgres, Redis for data storage
- Using Rabbitmq for asynchronous message queuing

[Visual Studio Tools for Docker with ASP.NET Core](./VsToolsForDocker/README.md)

[Branch: Add-Swagger-Support: ASP.NET Core Web API help pages with Swagger / Open API](https://github.com/rakeshhira/AspNetCore.MyLearning/blob/add-swagger-support/SwaggerSupport/README.md)

[Branch: Integrate-rabbitmq: ASP.NET Core Web API with rabbit mq](https://github.com/rakeshhira/AspNetCore.MyLearning/blob/integrate-rabbitmq/)

# Integrate MassTransit

My goal was to integrate [MassTransit](http://masstransit-project.com/) on top three web api(s) that had been created in [Branch: integrate-rabbitmq](https://github.com/rakeshhira/AspNetCore.MyLearning/tree/integrate-rabbitmq).

Specifically, I wanted to implement following workflow
- Migrate RabbitMQ tutorials to use MassTrasit
- Migrate AspNetCoreWeb tutorial to use MassTrasit
- Migrate AspNetCoreWeb1-3 APIs to use MassTrasit

# Concepts I learned

## Messages and serialization 
Concept | Notes
------- | ------
Message | A message is a chunk of JSON, XML or even binary data
Typed Message | When using typed language (C#) a message is represeted by an instance of a class (or interface)
Typed Enties | Messages are sent, received, published and subscribed as types
JSON Serialization | Messages are tranlated using JSON serialization for RabbitMQ.

## Sagas
Concept | Notes
------- | -----
Sagas | A saga is a long-running transcation that is managed at the application layer.  MassTransit allows sagas to be declared as a regular class or as a state machine using a fluent interface.
Correlation | Framework manages the saga instance and correlates messages to the proper saga instance.
CorrelationId | Correlation is typically done using a CorrelationId, a Guid.

## Endpoints
Concept | Notes
------- | ------
Receive Endpoints | A receive endpoint receives messages from a transport, deserializes the message body, and routes the message to the consumers.
Send Endpoints | A send endpoint is used by an application to send a message to a specific address.
Endpoint addressing | URI are used to identify endpoints.  An example RabbitMQ endpoint address for my_queue on the local machine would be: rabbitmq://localhost/my_queue

## Routing on RabbitMQ


* `pub/sub` : MassTransit follows pub/sub message pattern, where a copy of the message is delivered to each subscriber.
* `Exchanges` : MassTransit uses the message type to declare exchanges and exchange bindings that match the hierarchy of the message types.  Interfaces are declared as separate exchanges and bound to the published message type exchange.
* `lifetime` : When message is first published, the exchanges are declared once, and then used for the life of the channel.
* `performance`: MassTransit use the [routing-topologies-for-performance-and-scalability-with-rabbitmq]
(http://spring.io/blog/2011/04/01/routing-topologies-for-performance-and-scalability-with-rabbitmq/) approach to maximize the routing performance in RabbitMQ.
* 




