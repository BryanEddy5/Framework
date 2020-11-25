# Webcore

A repository defining a collection of libraries which collectively make up an opinionated core for .NET Core WebAPIs. Meant to facilitate rapid creation and deployment
of .NET Core WebAPIs on a re-usable hardened architectural stack.

## First-time setup

1. Install the .NET SDK for either [Windows](https://dotnet.microsoft.com/download/dotnet-core/thank-you/sdk-3.1.300-windows-x64-installer) or [macOS](https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-3.1.300-macos-x64-installer)
    - Current version is 3.1.300

### Visual Studio

The version of the SDK in use requires you to be using VS2019. VS2017 is not compatible with this version of the SDK.

## Features
### Dependency Injection using attributes
#### Registering a service
[DiComponent Example](example/WebApi/DependencyInjection/TransientComponent.cs)
```
[DiComponent]
FooService : IFooService
```

#### Registering Options Pattern 
```
[DiOption]
FooOptions
{
    public string Name { get; set; }
}
```
Put values in appsettings.json to be injected into `FooOptions`
```
"FooOptions" : {
    "Name" : "Bar"
}
```

## Secrets Manager
Register secrets manager with the **secret** shape and **configuration** settings that point to the location and version of the secret.
1. [Register the service](example/WebApi/Startup.cs#L29)
1. [Create instance of configuration options](example/WebApi/Secrets/FooSecretsOptions.cs)
1. [Add configuration settings to appsettings.json](example/WebApi/appsettings.json#L16)
1. [Inject IService<TSecret>](example/WebApi/Secrets/UseSecretService.cs)

## Application Bootstrapping
There are two components to the bootstrapping process. 
1. Inheriting from `BaseStartup<TStartup>` for the [Startup](example/WebApi/Startup.cs#L20) class.
This allows for all applications to be bootstrapped using specific services and MVC pipeline middleware that is common for all Microservices.
Examples:
- Adding all common Webcore services (dependency injection, swagger, rest client, application telemetry)
- HttpContextAccessor
- Utilizing Routing for controllers
- Use Hsts
- Use Http Redirection
- Mapping controllers
- Exception Handling middleware
Each Microservice can also customize the services [ConfigureAppServices](example/WebApi/Startup.cs#L26)

## Encryption Service
