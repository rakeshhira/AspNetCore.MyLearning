# Swagger Support

I followed the instructions in following article to create a visual studio project to learn swagger support

- [ASP.NET Core Web API help pages with Swagger / Open API](https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-2.1)

If you had followed this article in the past, you can compare your learning with mine.  I created the project as following
- Visual Studio 2017
- **File** -> **New** -> **Project** -> **Visual C#** -> **Web** > **ASP.NET Core Web Application**
- Set name of the project to **VsToolsForDocker**
- Select **API** template
- Select **Enable Docker Support**
- Unselect **HTTPS**

## What did I learn?

### Basic Concepts
- Swagger is documentation for web api.  This documentation needs to be generated in JSON format and then exposed as an endpoint so that it could be consumed by machines or by humans.

	Swagger endpoint looks like following

> http://localhost:\<port\>/swagger/v1/swagger.json


- While the JSON is machine consumable, browsers don't yet automatically translate use it a pretty UI that can be used to exercise the webapi.  So UI needs to be generated in addition to the swagger JSON.

- Swagger can be configured to incorporate XML comments.  Decorating model with `DataAnnotations` help drive Swagger UI components.

- In Dev env, it is a good idea to serve swagger UI at the app's root during development.  Following article details about supported enviroments.  [Use multiple environments in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-2.1)

The steps mentioned above fit nicely in Startup.ConfigureServices & Startup.Configure 

```
		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			...
			// Register the Swagger generator, defining 1 or more Swagger documents
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
			});
			// Set the comments path for the Swagger JSON and UI.
			var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
			var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
			c.IncludeXmlComments(xmlPath);
			...
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			...
			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
				if (env.IsDevelopment())
				{
					c.RoutePrefix = string.Empty;	
				}
			});
			...
		}
```

`launchSettings.json` with Docker using app root url.

```
	"Docker": {
	  "commandName": "Docker",
	  "launchBrowser": true,
	  "launchUrl": "{Scheme}://localhost:{ServicePort}/"
	}
```