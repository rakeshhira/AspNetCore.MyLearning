# Add Docker Compose Support
My aim was to create three web apis with one depending upon postgres, another depending upon redis and all depending upon rabbitmq.

Articles used:
- I used following article to get an overview of docker compose
	- [compose overview](https://docs.docker.com/compose/overview/)
- I used following article to add docker compose support
  - [Add container orchestrator support to an app](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/visual-studio-tools-for-docker?view=aspnetcore-2.1#add-container-orchestrator-support-to-an-app)
- I used following article to install PostgreSQL on Docker
	- [Docker Hub postgres](https://hub.docker.com/_/postgres/)
- I used following article to install Redis on Docker
	- [Docker Hub redis](https://hub.docker.com/_/redis/)
- I used following article to install RabbitMQ on Docker
	- [Docker Hub rabbitmq](https://hub.docker.com/_/rabbitmq/)
	- https://github.com/micahhausler/rabbitmq-compose
	- [RabbitMQ Tutorials](https://www.rabbitmq.com/getstarted.html)

Visual studio started the initial docker compose file with support for three web api's and then I added support for redis, postgres and rabbitmq.  Following is the final docker compose.

```
version: '3.4'

services:
  aspnetcorewebapp1:
	image: ${DOCKER_REGISTRY}aspnetcorewebapp1
	build:
	  context: .
	  dockerfile: AspNetCoreWebApp1/Dockerfile
	depends_on:
	  - rabbitmq

  aspnetcorewebapp2:
	image: ${DOCKER_REGISTRY}aspnetcorewebapp2
	build:
	  context: .
	  dockerfile: AspNetCoreWebApp2/Dockerfile
	depends_on:
	  - db
	  - rabbitmq

  aspnetcorewebapp3:
	image: ${DOCKER_REGISTRY}aspnetcorewebapp3
	build:
	  context: .
	  dockerfile: AspNetCoreWebApp3/Dockerfile
	depends_on:
	  - redis
	  - rabbitmq

  db:
	image: postgres
	environment:
	  POSTGRES_PASSWORD: example

  redis:
	  image: redis:alpine

  rabbitmq:
	image: "rabbitmq:3-management"
	hostname: "rabbitmq"
	environment:
	  RABBITMQ_ERLANG_COOKIE: "my_secret_goes_here"
	  RABBITMQ_DEFAULT_USER: "rabbitmq"
	  RABBITMQ_DEFAULT_PASS: "rabbitmq"
	  RABBITMQ_DEFAULT_VHOST: "/"
	ports:
	  - "15672:15672"
	  - "5672:5672"
```

## What did i learn?

### Syntax
In [YAML syntax](http://yaml.org/spec/1.2/spec.html) 
- Colon is used to define key:value pair.
- Indentation is used to define block or objects.

### Keywords
#### version
The default docker-compose.yml file created by visual studio has version 3.4.  There are several [version of docker compose file](https://docs.docker.com/compose/compose-file/#compose-and-docker-compatibility-matrix)

#### services 
`services:` keyword is used to define service definition of each container.  


#### Service/container name keyword
The first indentation level under the `services:` defines the service/container names.  The default compose file has 3 services.

#### image
[`image`](https://docs.docker.com/compose/compose-file/#image) keyword specify the image to start the container from.  
- $\{DOCKER_REGISTRY\} is an enviroment variable.
- It is typically empty for dev env.
- It is set to docker registry for public images
 
#### build
[`build`](https://docs.docker.com/compose/compose-file/#build) keyword specify the options applied at build time.  It specifies the context, dockerfile and args.  Compose names the build image per the `image` keyword.
 
#### context
[`context`](https://docs.docker.com/compose/compose-file/#context) keyword specify the path to directory containing dockerfile.
 
#### dockerfile
[`dockerfile`](https://docs.docker.com/compose/compose-file/#dockerfile) keyword specify the alternate file to build with.

#### environment
[`environment`](https://docs.docker.com/compose/compose-file/#environment) keyword specifies the environment variables

#### ports
[`ports`](https://docs.docker.com/compose/compose-file/#ports) keyword exposes / maps ports.

#### depends_on
[`depends_on`](https://docs.docker.com/compose/compose-file/#depends_on) keyword expresses dependencies between services

### Adding Postgres, Redis and Rabbitmq
#### Postgres
The docker-compose for [postgres](https://hub.docker.com/_/postgres/) looks like following:
```
  db:
	image: postgres
	environment:
	  POSTGRES_PASSWORD: example
```
- `image:postgres` The `db` service uses public [postgres](https://hub.docker.com/_/postgres/) image pulled from the Docker Hub registry
- `environment` Adds one or more environment varialbes
  - `POSTGRES_PASSWORD: example` Sets superuser password for PostgreSQL.  The default superuser is defined by the `POSTGRES_USER` environment variable.  The default user is `postgres`.

#### redis
The docker-compose for [redis](https://hub.docker.com/_/redis/) looks like following:
```
  redis:
	  image: redis:alpine
```
- `image:redis:aline` The `redis` service uses public [redis](https://hub.docker.com/_/redis/) image pulled from the Docker Hub registry

#### rabbitmq
The docker-compose for [rabbitmq](https://hub.docker.com/_/rabbitmq/) looks like following:
```
  rabbitmq:
	image: "rabbitmq:3-management"
	hostname: "rabbitmq"
	environment:
	  RABBITMQ_ERLANG_COOKIE: "my_secret_goes_here"
	  RABBITMQ_DEFAULT_USER: "rabbitmq"
	  RABBITMQ_DEFAULT_PASS: "rabbitmq"
	  RABBITMQ_DEFAULT_VHOST: "/"
	ports:
	  - "15672:15672"
	  - "5672:5672"
```
- `image:rabbitmq:3-management` The `rabbitmq` service uses public [rabbitmq](https://hub.docker.com/_/redis/) image pulled from the Docker Hub registry.  The `3-management` tagged image is build on top of rabbitmq:3.7 and adds management plugin. 
- `hostname: "rabbitmq"` RabbitMQ stores data based on what it calls the "Node Name" which defaults to the hostname. So that we don't get a random hostname, `rabbitmq` service assigns a hostname of `rabbitmq`.
- `RABBITMQ_ERLANG_COOKIE: "my_secret_goes_here"` The `RABBITMQ_ERLANG_COOKIE` environment variable is used to set a consistent cookie to allow RabbitMQ nodes and CLI tools to communicate with each other.
- `RABBITMQ_DEFAULT_USER: "rabbitmq"` The RABBITMQ_DEFAULT_USER envinronment variable changes the default  username from "guest" to "rabbitmq"
- `RABBITMQ_DEFAULT_PASS: "rabbitmq"` The RABBITMQ_DEFAULT_PASS envinronment variable changes the default  password from "guest" to "rabbitmq"
- `RABBITMQ_DEFAULT_VHOST: "/"`  The RABBITMQ_DEFAULT_VHOST environment variable changes the default vhost.
- `ports:`
  - `"15672:15672"` The management plugin is available on default container port 15762.  It is exposed to host at the same port.
  - `"5672:5672"` The RabbitMQ listens on default container port 5762.  It is exposed to host at the same port.
