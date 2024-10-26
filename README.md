# .NET 8 web API starter pack

## Introduction

This project implements an ASP.NET Core 8 web API with the most common features. It is paired with a SQL Server database using a code first approach.
While this project use Docker Compose for running the API and its database, the main goal is not to cover DevOps technologies. This content focus
primarily on building a simple ASP.NET web API with the latest .NET version.

### Prerequisites

Before anything please install if they are not already the following elements
- Download and install **.NET 8** [here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Download and install **Docker Desktop** [here](https://docs.docker.com/desktop/install/windows-install/)

### Run

In order to run this application, set Docker Compose as start up project and click on run. According to its configuration, Docker Compose will create a
**container for the API** based on the corresponding project and an other **container for a SQL Server database** based on a Microsoft official image. 
EntityFramework migrations will be applied at runtime to the database.

Inside the web API project there is a folder which contains **predefined HTTP requests**. First request should create a new user with **UserCredentials.http**.
Once it's done JWT can be generated with **Authentication.http**, then authorized endpoints can be accessed and so on.

### Tests

Of course this solution includes unit tests and integration tests for this application. 

Note that **integration tests use a containerized database** by leveraging the **Testcontainers** nuget package. This is a relevant solution as integration 
tests aim to test the application in an environment mirroring production, so we can validate its behavior in conditions closely matching actual usage.

## Features

### Services and dependency injection pattern

If you inspect old projects, you might find **most of the code inside controllers**, after all this it what **MVC** means. As practices have evolved,
it is now recommended to implement business logic in what we call **services**. 

Once this is done, services can be injected in controllers but also in other services. Where previously the business logic contained in a
controller was not accessible from another controller, we can now **share it everywhere** with a service.

In this project services are declared in **DependencyInjectionSetup.cs**

```csharp
public static class DependencyInjection
{
    public static void AddStarterServices(this IServiceCollection services)
    {
        services.AddScoped<IAppContextAccessor, AppContextAccessor>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();
    }
}
```

This method is called in **Program.cs** like this `builder.Services.AddStarterServices()`. Once this is done, services are 
**available in all controllers and services constructor** like in the following examples.

```csharp
public class AuthenticationController(IJwtService jwtService) : StarterControllerBase

public class JwtService(ILogger<JwtService> logger, IConfiguration configuration,
    IUserRepository userService) : IJwtService
```

### JWT authentication

As JWT authentication is the standard **approach** to secure a web API, it's quite a good place to start. This project implements the whole process.
- **Send your credentials with a post requests** to an endpoint of **AuthenticationController** and get a token. 
- Now **secured endpoints** can be accessed by adding this token to the **authorization header** of HTTP requests.

JWT authentication is declared in **Program.cs** as follows.

```csharp
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        JsonWebTokenParameters jwt = builder.Configuration
            .GetRequiredSection("JsonWebTokenParameters")
            .Get<JsonWebTokenParameters>()
                ?? throw new Exception("JWT settings are not configured");

        byte[] encodedKey = Encoding.ASCII.GetBytes(jwt.Key);
        SymmetricSecurityKey symmetricSecurityKey = new(encodedKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = symmetricSecurityKey
        };
    });
```

### Mulitple configurations

Maybe you need an application that has more than the usual Debug and Release configuration. Let's say you have a **custom configuration**
for one of your clients. We will call this configuration Custom. That being said you might have different settings for this configuration and
create an **appsettings.Custom.json file**. Now you expect the application to use these settings when you **publish** it with the Custom configuration.

But this is not how .NET works. No matter what configuration you select during publishing, the application will look for **appsettings.environmentName.json**
during its execution. For example **appsettings.Development.json** or **appsettings.Production.json** for respectively **Debug** and **Release** configuration.

This project has code in Program.cs to **handle this situation**. Hence if you publish your application with Custom configuration, it will look for
**appsettings.Custom.json** and so on.

```csharp
string configurationName = Assembly.GetExecutingAssembly()
    .GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration
        ?? throw new Exception("Can not read configuration name");

builder.Configuration.AddJsonFile($"appsettings.{configurationName}.json");
```

### Endpoints URL convention

Google specified some guidelines for URL [here](https://developers.google.com/search/docs/crawling-indexing/url-structure?hl=fr). They should be written 
using the **kebab case** but as this is the not default behavior in an ASP.NET Core web API, some modifications are needed. First we need to define a 
**IOutboundParameterTransformer** which will convert any value from pascal case to kebab case. In the **Utilities** folder you will find 
**ToKebabParameterTransformer.cs** file with the following content.

```csharp
public partial class ToKebabParameterTransformer : IOutboundParameterTransformer
{
    public string TransformOutbound(object? value)
    {
        return MatchLowercaseThenUppercase()
            .Replace(value?.ToString() ?? "", "$1-$2")
            .ToLower();
    }

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex MatchLowercaseThenUppercase();
}
```

Now we can add a new convention inside every controllers by modifying the **Program.cs** file like this.

```csharp
builder.Services.AddControllers(options =>
    {
        ToKebabParameterTransformer toKebab = new();
        options.Conventions.Add(new RouteTokenTransformerConvention(toKebab));
    });
```

Every endpoint URL will now use the kebab case by default.

```
/api/authentication/token
```

### HTTP files

While **Postman** is really great, having a HTTP client with all your predefined requests **inside your project** is such a handy tool. It allows to 
bind your code to those requests **in the version control**. Hence when members of the team pull your code, they instantly have the possibility 
to test it with your HTTP requests, saving time and making collaboration easier.

HTTP requests are defined in **.http files**. Examples for this project can be found in the **Https** folder. Each file corresponds to a controller. There 
a still some limitations, it is not possible to add **pre-request or post-response scripts** like in Postman. That being said, this tool is quite new
and it is reasonable to think that this kind of features will be added in the future.

```http
POST {{HostAddress}}/api/authentication/token
Content-Type: application/json

{
  "emailAddress": "john.doe@example.com",
  "hashedPassword": "TWF0cml4UmVsb2FkZWQh"
}
```

**Variables**, which are between double curly braces, can be defined in the **http-client.env.json file**. Multiple environments can be configured, making 
possible to attribute **a different value** to a variable for each environment. Then it is easy to **switch** between environment with the same request, 
making the workflow even **faster**.

Note that everytime this file is modified, **closing and reopening** Visual Studio is needed so changes are **taken into account**. I hope Microsoft will 
fix this in the future.

```json
{
  "dev": {
    "HostAddress": "https://localhost:7137",
    "Jwt": "xxx.yyy.zzz"
  },
  "prod": {
    "HostAddress": "https://starterwebapi.com",
    "Jwt": "xxx.yyy.zzz"
  }
}
```

See official Microsoft documentation for more information [here](https://learn.microsoft.com/en-us/aspnet/core/test/http-files?view=aspnetcore-8.0).

### Code first approach

First create DbContext and entity classes inside the infrastructure layer. Then **register DbContext as a service** inside Program.cs.

```csharp
string connectionString = builder.Configuration.GetConnectionString("SqlServer")
    ?? throw new Exception("Connection string for SQL Server is missing");

builder.Services.AddDbContext<StarterDbContext>(options =>
    options.UseSqlServer(connectionString));
```

Once all above is done, it is possible **apply this structure** to the running database. As the project containing DbContext and the web API project are 
two different things we define an implementation of **DesignTimeDbContextFactory** interface. 

This is needed as otherwise we would have to install EntityFramework packages in the web API project in order to create migration files. This is **strongly 
not recommanded** as in Clean Architecture, the only layer responsible for persistance is the infrastructure layer, not the presentation layer.

In this example we are using a SQL Server 2022 database.

```csharp
public class StarterDbContextFactory : IDesignTimeDbContextFactory<StarterDbContext>
{
    public StarterDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<StarterDbContext> optionBuilder = new();

        // docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=B1q22MPXUgosXiqZ" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
        string connectionString = "Data Source=localhost,1433;Initial Catalog=Starter;User=sa;Password=B1q22MPXUgosXiqZ;TrustServerCertificate=yes";
        optionBuilder.UseSqlServer(connectionString);

        // Context used during the creation of migrations
        return new StarterDbContext(optionBuilder.Options);
    }
}
```

Then create a new migration with this .NET CLI command inside the infrastructure project. New .cs files describing every table will be generated in **Migrations folder**.

```bash
dotnet ef migrations add InitialCreate --output-dir Persistance/Migrations
```

When it's done, **migration** can be applied to the database with this command. EntityFramework will use the connection string
in order to connect to the running database and **apply changes** contained in the previously generated migration files.

```bash
dotnet ef database update
```

As this project use a containerized database, some unsual scenarios can happen. For example, the API is already running and a database container is 
created. Another scenario could be with an already running database container and the API that starts running.

In each of these scenarios we need to make sure the **migrations are applied** to the database and in case not, the application should do it. Migration 
could be **applied at application startup** but that wouldn't cover the scenario where **database is dropped and recreated while applicaton is still running**. 
This is why I make sure **all migrations are applied at every interaction with the database** with the following piece of code inside my **DbContext**.

```csharp
public partial class StarterDbContext : DbContext
{
    public StarterDbContext(DbContextOptions<StarterDbContext> options)
        : base(options)
    {
        string? aspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (aspNetCoreEnvironment == "Development" || aspNetCoreEnvironment == "Integration")
        {
            Database.Migrate();
        }
    }
```

As this application uses an in-memory database for unit tests, **migration should not be applied during those tests**. This is why I am checking the environment 
name before applying or not the migrations.

### Log automatically invalid model state

A common error when developing a web API is to post an **invalid object** and get a **bad request** response in return. When this happens the developer 
needs to investigate the **ModelState**, but it can be a long and painful process. Fortunately, it is now possible to automatically log ModelState errors and
see the **relevant details**, particularly which **object property** is causing the invalid state.

```csharp
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        var builtInFactory = options.InvalidModelStateResponseFactory;
        options.InvalidModelStateResponseFactory = context =>
        {
            ILogger<Program> logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<Program>>();

            IEnumerable<ModelError> errors = context.ModelState.Values
                .SelectMany(item => item.Errors);

            foreach (ModelError error in errors)
            {
                logger.LogError("{ErrorMessage}", error.ErrorMessage);
            }

            return builtInFactory(context);
        };
    });
```

As this is logging, it will appear in every configured sink. One of them being the console which is always open, it allows developers to see this 
type of content immediately. Here is an example.

```
2024-08-08 18:10:01 fail: Program[0]
2024-08-08 18:10:01       The EmailAddress field is required.
```

### Global usings

With the new feature `global using`, namespaces can be included for the whole project instead having to specify it in every file. This feature improve 
maintainability and save time on repetitive tasks. Implementation can be found in **GlobalUsing.cs** file, inside each project root folder.

```csharp
global using AutoMapper;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Starter.WebApi;
global using Starter.WebApi.Controllers.Abstracts;
global using Starter.WebApi.Utilities;
```