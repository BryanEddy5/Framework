# Webcore

A repository defining a collection of libraries which collectively make up an opinionated core for .NET Core WebAPIs. Meant to facilitate rapid creation and deployment
of .NET Core WebAPIs on a re-usable hardened architectural stack.

## First-time setup

1. Install the [.NET SDK ](https://dotnet.microsoft.com/download)
    - Current version is 5.0.300

### Visual Studio

The version of the SDK in use requires you to be using VS2019. VS2017 is not compatible with this version of the SDK.

## Features
### Dependency Injection using attributes
The `DiComponent` attribute allows developers to easily register services for dependency injection by decorating the class with `DiComponent`. The default service registration lifetime is `Transient` per Microsoft's recommendation as the default service lifetime.

Developers can figure configure the service lifetime by setting `LifetimeScope` property on the attribute.
#### Registering a service
[DiComponent Example](example/src/WebApi/DependencyInjection/TransientComponent.cs)
```
[DiComponent]
FooService : IFooService
```
#### Registering a service using a non-default lifetime
```
[DiComponent(LifetimeScope = LifetimeScopeEnum.Singleton)]
FooService : IFooService
```

### Captive Dependency Issue
Developers should be aware of the lifetime of their developed services as well as any services injected into it. A service should never depend on a service that has a shorter lifetime than its own. See (Microsoft's documentation)[https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines#captive-dependency] for additional detail.

#### Registering Options Pattern
To aid in the registration of POCO's in utilizing the Options Pattern (injectd `IOptionsMonitor<T>` or `IOptionsSnapshot<T>`) an attribute has been created that is used to decorate the Options POCO.
[DiOptions Example](test/framework/Framework.DependencyInjection.Tests/Stubs/FooClientOptions.cs)
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

## Working with SOAP-ey Integrations
There comes a time in every developer's life where they may need to build an integration with a SOAP service.
Fortunately for you, you can leverage the `Core.Soap` and `Framework.Soap` projects to ease the pain of having
to work with such an old and abandoned protocol in this day and age. Unfortunately, due to this age, there is
a bit of a manual process you'll need to walk through in order to scaffold your integration and build it out.
However, once built, you should be able to leverage it in a similar fashion to our `IRestClient`. Follow the
below steps to get started in building your own SOAP integration leveraging webcore.

### Prerequisites
Despite this SDK multi-targeting `netcoreapp3.1` and `netstandard2.1` and `net5`:
- your integration must be `net5`.
- you must ensure you have a NET Core 2.1 SDK installed on your local machine.

#### 1) Installing `dotnet-svcutil`
This is a command line procedure, so you just need to run:
```
dotnet tool install --global dotnet-svcutil
```
Which installs the necessary tool for the next step.

#### 2) Scaffolding the `Reference.cs`
The `Reference.cs` is the contract file of which your `ISoapClient` will leverage. You'll want to navigate via CLI
into the integration project, create a `/Client` directory, and run the following command in order to generate it:
```
dotnet-svcutil WSDL_SVC_URL_HERE
```
An example of the `WSDL_SVC_URL_HERE` as found in `/example` is `http://www.dneonline.com/calculator.asmx?wsdl`.

#### 3) I'm sorry StyleCop, but you aren't needed here.
You'll see that suddenly, there is a `/ServiceReference` directory in your `/Client` folder. Inside there, the
`Reference.cs` and some other json file will be in there. In order to build the project to even use this file, you'll
want to add a `#pragma warning disable CS1591` line in there, because XML comments likely won't be there from the server.
At this point, your project should be build-able.

#### 4) Construct the SOAPey client.
Though this procedure is similar, this is where the difference between a RESTful client and SOAPey client starts to stand out.
Like you do with the `RestClient`, you'll need to spin up a new class and make it inherit from `BaseSoapClient^2` (from `Core.Soap`)
and make the constructor pass the requested parameters into the `base` class. However, this is where we start to deviate from your
average `RestClient` implementation...

#### 5) Leverage your inner Channel!
When it comes to `RESTful` requests, you typically just need a url and to know the parameters/body of the contract in order
to send the request. That is often derived externally from requirements found in a story or otherwise. In the land of SOAP, we instead
prebuild that ahead of time and serve it to consumers of our SOAPey service as a `wsdl`. Integrating with a `wsdl` url as we
did in step 2 will produce effective a list of contracts and methods available for you to use in your service. As such, you'll need to
pass in the respective interface that contains those methods _into your `BaseSoapClient^2`_, and build corresponding methods to
leverage them. This may be a tad confusing, so I strongly recommend you peek at the example that is built out in the `/example` solution
provided alongside webcore to see what that all means.

#### 6) My SOAPey client has methods?
That is correct! Unlike the `RestClient` we all know and love, our `BaseSoapClient^2` knows what to do under the covers with the
Channel, so all you need to do is use it and build some methods that correspond with your individual methods on the Channel. These should
also exist on your own interface, so that you can leverage them in your services and write unit tests for them! Again you should review
the `Integration.Calculator` project for a visual representation of how that looks if it doesn't make sense.

#### 7) Configure the configuration.
Now that you're setup with your very own `SOAP` client, you should have no problem leveraging that client's additional methods described in
steps 5 and 6 to build services, but don't forget that similar to the `RestClient`, there is a configuration builder that can aid you in
building the configuration object with some hand-holding. For example, if your `wsdl` url is the sample one listed above, then the base is this:
```
http://www.dneonline.com/calculator.asmx
```
Additionally, you can set the timeout for your particular client to be non-standard if you need it- but it does have its own default.

### Concluding SOAPey Notes
At this point, you should have a working SOAP client that interfaces with your chosen `wsdl`! Though every `wsdl` is a little different, and some
even come with multiple interfaces (which mean you should have multiple clients), but yours should be working as-expected.
A working example of a basic public SOAP `wsdl` lives in the `/example` project within webcore, and shows all the various features that the
this implementation of a `SOAP` client supports- it is strongly encouraged to review it for clarifying details.

## Secrets Manager
The Secrets Manager library retrieves stored secrets from GCP Secrets Manager and cache's in memory the secret once it is retrieved.  Since secrets are versioned in GCP there is no need for the cache to be refreshed for if the secret is updated in GCP so is the version.  The `appsettings.json` file will need to be updated with the new version and thus a new value will be pulled into the cache as the cache key is a composite key of the values for retrieving the key from Secrets Manager.
Steps: Register secrets manager with the **secret** shape and **configuration** settings that point to the location and version of the secret.
1. [Create a class that matches the Secret shape and inherit from `ISecret`](example/src/WebApi/Secrets/FooSecret.cs)
1. [Register the service](example/src/WebApi/Startup.cs#L37)
1. [Add configuration settings to appsettings.json](example/src/WebApi/appsettings.json#L16)
1. [Inject ISecretsService<TSecret>](example/src/WebApi/Secrets/UseSecretService.cs#L20)

Note that since each individual secret needs it's own configuration settings (ProjectId, SecretId, and Secret Version) it is necessary to register each secret individually if you would like to retrieve multiple secrets for a particular service.
### Add the appsettings configuration for each secret.
```
  "FooSecretsOptions": {
    "ProjectId": "some-project-id",
    "SecretId": "some-secret-id",
    "SecretVersionId": "1"
  },
  "BarSecretsOptions": {
    "ProjectId": "a-different-project-id",
    "SecretId": "some-other-secret-id",
    "SecretVersionId": "2"
  },

```

### Register the services with the correct extension of the `SecretsOptions`
```
services.AddSecret<FooSecret>(Configuration.GetSection(nameof(FooSecretsOptions)));
services.AddSecret<BarSecret>(Configuration.GetSection(nameof(BarSecretsOptions)));
```

## Application Bootstrapping
In an effort to consolidate code and remain DRY, much of the application bootstrapping has been moved to a `BaseStartup<TStartup>` class.  This allows for changes to be made in a single place and abstract away details that are not of interest to each individual service.  There are some virtual methods that can be overriden to allow for customization of each microservice.
There are two components to the bootstrapping process.

1.[Inheriting from `BaseStartup<TStartup>` for the `Startup` class](example/src/WebApi/Startup.cs#L18) .

This allows for all applications to be bootstrapped using specific services and MVC pipeline middleware that is common for all Microservices.
Examples:
- Adding all common Webcore services (options pattern, dependency injection, swagger, rest client, application telemetry)
- HttpContextAccessor
- Utilizing Routing for controllers
- Use Hsts
- Use Http Redirection
- Mapping controllers
- Exception Handling middleware
- Request telemetry
- Swagger (OpenApi) documentation and `/index.html` GUI.

[ConfigureAppServices](example/src/WebApi/Startup.cs#L34) allows for each Microservice can also customize the services

2.[Utilizing `UseCustomHostBuilder<TStartup>` in `Program` class](example/src/WebApi/Program.cs#L28)

This configures the logging and service dependency injection from Webcore.

## Encryption Service
Offers simple symmetric encryption and decryption in utf-8 base64 url encoded string.  This encoding enables the encrypted string to be passed as part of a Url path and properly decode and decrypt.
1. [Register the service](example/src/WebApi/Startup.cs#36)
1. [Add configuration settings to appsettings.json](example/src/WebApi/appsettings.json#L21)
1. [Inject IEncryptionService](example/src/WebApi/Encryption/UseEncryptionService.cs#L20)

## MessageAppException
`MessageAppException` allows for communicating a message and http status code back to the consumer. These exceptions are caught by hte Exception Handling middleware that will inspect the exception and set the response equal to the message and assigned http status code.
As use case for this scenario would be a resource not found (404). Upon retrieving a resource from an external source (database, RESTful service, etc) a status code of 404 can be attached with a message of `The resource of Foo is not found`.  This removes the need to implement the `result inspection` anti pattern of communicating back to consuming services that a resource has not been found.
Exceptions are our first class citizen for communicating errors to consumers.

Developers should extend `MessageAppException` and override the http status code in order to set it in the response.
Example: [Resource Not Found](example/src/Integration.CatFacts/Exceptions/NotFoundCatFactsExceptions.cs)

You can also include a separate message to be logged that supports structured logging to enrich the log context with additional information.
Example: [Structured Exception Logging](example/src/WebApi/Controllers/ExceptionController.cs)

## Pub/Sub Subscriber
Creates an instance of `IHostedService` that pulls from a Pub/Sub Subscription.  The published message will not be acked or nacked until the process handler has completed the request.  If an exception is not thrown and the process completes then the message will be acked.  The default behavior for an exception thrown is to nack the message to be retried.  This flow can be controlled utilizing `PubSubException` and overriding the `Reply` property to `Ack` instead of `Nack` if the exception is not recoverable (meaning that the exception will persist with future attempts.)  The suggest course of action for exceptions thrown is to `Nack` the message and incorporate a Dead Letter Queue (in the form of another GCP Topic and subscription) to push the message to aftre it has failed after so many attempts (10 being the current max attempts in GCP).
The client incorporates telemetry of type `Subscription` that indicates if the message was successfully `Ack`ed, the duration of the subscription handling, and the unique message id of the message that was processed.
1. [Create an implementation of ISubOrchestrationService<TMessage>](example/src/WebApi/PubSub/Subscription/FooSubscriptionHandler.cs)
1. [Create a class that matches the shape of the Topic Message](example/src/WebApi/PubSub/FooContract.cs)
1. [Register the service with classes from previous steps](example/src/WebApi/Startup.cs#37)
1. [Add configuration settings to appsettings.json](example/src/WebApi/appsettings.json#L27)
#### Configuration - PubSubOptions
Limiting number of messages processed in parallel has proven to be a valuable configuration setting.  By limiting the number of messages being processed we can ensure other integrated services aren't overwhelmed.  This was highlighted in the case of `ah-prv-contracting` sending hundreds of concurrent requests to `Nexus`.  
Example of configuration in `appsettings.json`
```
"FooSubscriptionOptions": {
    "ProjectId": "some-project-id",
    "Name": "the-subscription-name",
    "MaxMessageCount": 1, // The number of parrallel messages that will be processed.
    "Resiliency": {
      "MaxRetries": 5 // The max number of retries before Acking a message.
    },
    "ExceptionStorage": {
      "ApplicationName": "FooService", // Name of the application when publishing a file to the bucket. This will be a part of the file name.
      "GcpBucket": "exception-bucket", // The bucket to publish the exceptions to
      "GcpProject": "my-project" // The name of the project where the bucket is located.
    }
  },
```

#### Pub/Sub Subscriber Model State Validation
Developers can now perform model state validation on their subscription message. To do so you must simply create a validator to validate your message type and then you are done. Use the Di Validactor decorator to register the validator.
Messages that are not of a valid model state will be `Ack'd` so they are no longer retried. An error log entry will be generated with the errors of the model state validation. If configured, the publshed bucket message will also contain the model state validation errors.

My Pub/Sub Subscription message.
```
namespace HumanaEdge.Webcore.Example.WebApi.PubSub
{
    /// <summary>
    /// A subscription contract the mirrors the shape of the expected Published Topic Message in GCP.
    /// </summary>
    public class FooContract
    {
        /// <summary>
        /// Some field in the contract.
        /// </summary>
        public string? Name { get; set; }
    }
}
```

My validator for the message.
```
using FluentValidation;
using HumanaEdge.Webcore.Core.DependencyInjection.Validators;

namespace HumanaEdge.Webcore.Example.WebApi.PubSub.Validators
{
    /// <summary>
    /// Here's some Foo.
    /// </summary>
    [Validator(typeof(FooContract))]
    public class FooContractValidator : AbstractValidator<FooContract>
    {
        /// <summary>
        /// ctor.
        /// </summary>
        public FooContractValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}

```
Now the exception `InvalidModelStateException` will be thrown and the aforementioned data will be captured.

#### Pub/Sub Exception
The subscription allows for implementations [PubSubException](src/core/Core.PubSub/PubSubException.cs) to be thrown with a [Reply](src/core/Core.PubSub/Reply.cs) either `Ack`ing (completing the transaction) or `Nack`ing (the message is to be retried) the message.
The default behavior for an exception thrown is to retry it which will be performed in accordance with the `Subscription` configuration in GCP.
1. Create a class that extends PubSubException and to be thrown.
    - [Recoverable exception](example/src/WebApi/PubSub/Subscription/RecoverableException.cs)
    - [Unrecoverable exception](example/src/WebApi/PubSub/Subscription/UnrecoverableException.cs)
1. Throw the custom exception.

## Pub/Sub Publisher
A `IPublisherClient<TMessage>` is used to publish messages of shape `TMessage` to a topic in a designated project.  The project settings are configured via the `appsettings.json`
The client incorporates telemetry of type `Publication` that indicates if the message was successfully published, how long it took to publish the message, and the message id of the published message.
1. [Create a class that matches the shape of the Topic Message](example/src/WebApi/PubSub/FooContract.cs)
1. [Register the service](example/src/WebApi/Startup.cs#L39)
1. [Add configuration settings to appsettings.json](example/src/WebApi/appsettings.json#L31)
1. [Inject IPublisherClient<TMessage>](example/src/WebApi/PubSub/Publication/UsePublisherClient.cs)

## Telemetry
Within each of our integration services, telemetry has been instrumented to give diagnostics on the health of each of these integrations.  By gathering data (like duration to complete, boolean indicating success, http status, etc) we can help pinpoint a particular failure for I/O operations and quickly determine if the failure was in the Microservice or an external source.
There are currently four types of telemetry.
1. `Publication` - published messages to a topic
1. `Subscription` - Incoming pulled subscription messages
1. `Request` - Incoming http requests
1. `Depdendency` - Outgoing http requests
   Each of these telemetry types can be easily queried in GCP by running the following:
   jsonPayload.metricEvent.TelemetryType = {TelemetryType}
   Example:
```
jsonPayload.metricEvent.TelemetryType = "Dependency"
```
Example Telemetry for `Dependency`
```
      "metricEvent":{
         "$type":"TelemetryEvent",
         "TelemetryType":"Dependency",
         "Timestamp":"2020-12-01T19:45:19.3710862+00:00",
         "Tags":{
            "Success":false,
            "Duration":205,
            "Uri":"https://apigw-np.humanaedge.com/api/cxp/v1/crosswalks",
            "ResultCode":"403",
            "HttpMethod":"GET"
         },
         "Name":"HttpDependencyTelemetry",
         "Alert":true
      },
```

### Alerting
In addition to the above basic data that is tracked passively just by leveraging our various libraries within webcore, an additional functionality of
"Alerting" is available to developers as a hook into the telemetry that will allow you to define API-or-service-specific conditions that will flip the boolean of `"Alert"` (as seen above).
For example, if the API you're building returns non-standard http status codes in their responses (such as humana core's various APIs), you can add a new `AlertCondition` into the service used to
communicate with those abnormal APIs that inspects the payload and looks for some conditions that you want to check for. If the condition is met, then the `Alert` boolean will be toggled to `true`.

For your convenience, there are a couple default `AlertCondition`s to get you started that can be found in `src/core/Core.Rest/Alerting/CommonRestAlertconditions`.
The `RestClient` by default has the `CommonRestAlertconditions.Standard()` applied to it, so all APIs using the `RestClient` will have this checking performed.

If you wish to construct your own condition, you're probably trying to set it up inside a service. Here's a quick example of what that might look like:
```cs
public async Task ExecuteAsync(CancellationToken cancellationToken)
{
  var request = new RestRequest(
    RelativePath,
    HttpMethod.Get)
      .ConfigureAlertCondition(MyAlert()); // overrides the RestClient's AlertCondition behavior with this one.
      
  var response = await _client.SendAsync(request, cancellationToken);
  return response.ConvertTo<SomeWeirdTypeWithNonStandardResponseCodes>(); // no more null conversion check here!
}

internal AlertCondition<BaseRestResponse?> MyAlert() // the type here must match or be a derivitive of the condition's parameter.
{
  bool MyCondition(BaseRestResponse? response) // the required signature of a "Condition".
  {
    if (response == null) // if the response we got was null, then its an alert!
    {
      return true;
    }
    
    // cast to the type of response based on service- options include: RestResponse, FileResponse
    var restResponse = response as RestResponse;
    if (!restResponse.IsSuccessful || restResponse.ResponseBytes == 0)
    {
      return true;
    }
    
    // maybe it was 200 successful, but the API being integrated with puts the real code in the body...
    var convertedResponse = restResponse.ConvertTo<SomeWeirdTypeWithNonStandardResponseCodes>();
    if (convertedResponse.ResponseCode == "105") // some "fictitious" bad response code.
    {
      return true;
    }
    
    // if everything checks out, no alerts!
    return false; 
  }
  
  return new AlertCondition<BaseRestResponse?>
  {
    Condition = MyCondition
    ThrowOnFailure = false // optional, but if you set it to true, it'll throw if an alert condition was met.
    Exception = new FailureToIntegrateException() // also optional, this will become an inner exception to the "AlertConditionMetException".
  }
}
```

One may ask: "Why would I need to do this? It just looks like another way to perform the validation we already have been doing!", and though that isn't incorrect, this also
opens up the possibility of being able to build simple GCP alerting conditions (the ones arriving via outlook to the distro list) based on solely the `if (Alert == true)` instead of the other
various possible metrics we currently have alerting setup for, effectively placing the control into the hands of developers.

One of the main premises to the Telemetry implementation is fitting each API with consistent diagnostic outputs to create monitoring and alerting around.  The telemetry is currently emitted to the GCP logs where log based metrics can be created to help monitor the overall health of our API's.  
It is highly advisable to have Telemetry incorporated for each integration point within an application, to reiterate the point from earlier, to help understand when a failure reported by a microservice is internal (the API has faulted) vs external (some service I've integrated with is the culprit).  This will increase the efficiency of troubleshooting and deugging.
**When determining why an API is failing (http status 500+, messages not being processed) the telemetry should be the first logs a developer should review.  Then utilizing the `TraceId` (from the W3C Trace Context) from the logs, the entirety of the request can be tracked as it crosses boundaries (from one service to another) and ultimately determine in which service the failure occured.**

## Library Service Configuration
You'll notice a consistent pattern for configuring Webcore libraries that utilize GCP Resources.  Each GCP Resource (Subscription, Published Topic, Encryption, Secrets Manager, etc) all require information about the provisioned resource (typically a ProjectId and other detailed Resource information) in order to properly utilize the resource.  This pattern consists of creating a new concrete class that extends Core.* Options class thus allowing for multiple instances of the configuration options, meaning that more than 1 of the same type of resource (and subsequent Webcore service) can exist.
An example would be configuring `ISecrestsService<TMessage>`
This pattern is powerful for it reduces boilerplate code for each Options POCO and utilizes the Options Pattern for easily configuring environment specific settings, as the resources configuration setting typically change from environment-to-environment (Dev to SIT to UAT to Production).

## Secrets and Local Configuration Overrides
Secrets should, without exception, never be stored in `appsettings.json` or `_env.son`.  Storing secrets in those files and commiting them to the repo exposes sensitive information.
A centralized location has been created that allows for a developer to override configuration settings and store secrets for local development in `appsettings.overrides.json`.
The launch settings for each service needs an `APP_ROOT` environment variable defined with the location of a directory for that microservice
Steps to configure overrides:
1. [Ensure APP_ROOT is defined for you OS](example/src/WebApi/Properties/launchSettings.json#27) and include a directory name that is unique to the application (`/webcore` in this case).
    - MacOs Example: `"APP_ROOT": "/Users/%USER%/dev/approots/webcore"`
1. Create a folder under that directory called `config`
1. Create a file named `appsettings.overrides.json`
1. Insert any configuration overrides or secrets needed to run the application locally.
```
{
  "FirestoreRepositoryOptions": {
    "ProjectId": "sbx-poc-bryan-336" // points to a sandbox environment
  },
  "PubSubOptions": {
    "ProjectId": "sbx-poc-bryan-336", // points to a sandbox environment
    "Name": "ah-crm-docs-user-action" // points to a sandbox environment
  },
  "InnovareClientOptions": {
    "Authentication": {
      "ClientId": "secret", // A secret 
      "ClientSecret": "another great secret" // A secret 
    }
  },
  "NexusClientOptions": {
    "ApiKey": {
      "HeaderValue": "secret-api-key-value"
    }
  },
  "CreClientOptions": {
    "Authentication": {
      "ClientIdValue": "fjdila;fjdial;fjdila5", // A secret 
      "ClientSecretValue": "fjeiaof;jeial;feafo;" // A secret 
    }
  }
}

```

## PII Logging and Masking
The HumanaEdge.Webcore.Framework.Logging package itself includes the Destructurama package which allows attributed based masking for PII-sensitive properties of any objects that are serialized via Serilog into Stackdriver. So, for example, one can safely log an Entity object for diagnostic purposes into Stackdriver because all the PII-sensitive properties will be masked appropriately. Here is a complete example (as of 2020-12-31) of supported masks that can be applied to properties.

```
        /// <summary>
        /// A sample credit card class to demonstrate Destructurama.Attributed masking.
        /// </summary>
        public class CreditCard
        {
            /// <summary>
            /// 123456789 results in "***".
            /// </summary>
            [LogMasked]
            public string? DefaultMasked { get; set; }

            /// <summary>
            ///  123456789 results in "REMOVED".
            /// </summary>
            [LogMasked(Text = "REMOVED")]
            public string? CustomMasked { get; set; }

            /// <summary>
            ///  123456789 results in "123***".
            /// </summary>
            [LogMasked(ShowFirst = 3)]
            public string? ShowFirstThreeThenDefaultMasked { get; set; }

            /// <summary>
            ///  123456789 results in "123******".
            /// </summary>
            [LogMasked(ShowFirst = 3, PreserveLength = true)]
            public string? ShowFirstThreeThenDefaultMaskedPreserveLength { get; set; }

            /// <summary>
            /// 123456789 results in "***789".
            /// </summary>
            [LogMasked(ShowLast = 3)]
            public string? ShowLastThreeThenDefaultMasked { get; set; }

            /// <summary>
            /// 123456789 results in "******789".
            /// </summary>
            [LogMasked(ShowLast = 3, PreserveLength = true)]
            public string? ShowLastThreeThenDefaultMaskedPreserveLength { get; set; }

            /// <summary>
            ///  123456789 results in "123REMOVED".
            /// </summary>
            [LogMasked(Text = "REMOVED", ShowFirst = 3)]
            public string? ShowFirstThreeThenCustomMask { get; set; }

            /// <summary>
            ///  123456789 results in "REMOVED789".
            /// </summary>
            [LogMasked(Text = "REMOVED", ShowLast = 3)]
            public string? ShowLastThreeThenCustomMask { get; set; }

            /// <summary>
            ///  123456789 results in "******789".
            /// </summary>
            [LogMasked(ShowLast = 3, PreserveLength = true)]
            public string? ShowLastThreeThenCustomMaskPreserveLength { get; set; }

            /// <summary>
            ///  123456789 results in "123******".
            /// </summary>
            [LogMasked(ShowFirst = 3, PreserveLength = true)]
            public string? ShowFirstThreeThenCustomMaskPreserveLength { get; set; }

            /// <summary>
            /// 123456789 results in "123***789".
            /// </summary>
            [LogMasked(ShowFirst = 3, ShowLast = 3)]
            public string? ShowFirstAndLastThreeAndDefaultMaskInTheMiddle { get; set; }

            /// <summary>
            ///  123456789 results in "123REMOVED789".
            /// </summary>
            [LogMasked(Text = "REMOVED", ShowFirst = 3, ShowLast = 3)]
            public string? ShowFirstAndLastThreeAndCustomMaskInTheMiddle { get; set; }

            /// <summary>
            ///  NOTE PreserveLength=true is ignored in this case
            ///  123456789 results in "123REMOVED789".
            /// </summary>
            [LogMasked(Text = "REMOVED", ShowFirst = 3, ShowLast = 3, PreserveLength = true)]
            public string? ShowFirstAndLastThreeAndCustomMaskInTheMiddle2 { get; set; }
        }
```
The test for validating that these properties are indeed masked as specified when logged lives [here](https://gitlab.humanaedge.com/glb/webcore/-/blob/master/test/framework/Framework.Logging.Tests/PiiLoggingTests.cs).

In HumanaEdge.Integration.Nexus (a Nuget package which Nexus clients consume in order to integrate with Nexus), Entities uses these attributes as follows, so any Nexus client that uses these contracts can safely log Entity objects into Stackdriver without worrying about PII leakage.
```
    public class BaseEntityContract<TIds, TRole, TPhone, TEmail>
        where TIds : IdsContract
        where TRole : RoleContract
        where TPhone : PhoneContract
        where TEmail : EmailContract
    {
        public virtual string EntityId { get; set; }

        public TIds Ids { get; set; }

        [LogMasked(PreserveLength = true, ShowLast = 1)]
        public string FirstName { get; set; }

        [LogMasked(PreserveLength = true, ShowLast = 1)]
        public string LastName { get; set; }

        [LogMasked(PreserveLength = true)]
        public string MiddleName { get; set; }

        [LogMasked(PreserveLength = true, ShowLast = 1)]
        public string PreferredName { get; set; }

        [LogMasked(PreserveLength = true)]
        public string Title { get; set; }

        [LogMasked(PreserveLength = true)]
        public string Suffix { get; set; }

        public string Gender { get; set; }

        [LogMasked(PreserveLength = true, ShowLast = 5)]
        public virtual string BirthDate { get; set; }

        // Other properties snipped for brevity
    }

    public class IdsContract
    {
        [LogMasked(PreserveLength = true, ShowLast = 4)]
        public virtual string SSN { get; set; }

        [LogMasked(PreserveLength = true, ShowFirst = 1, ShowLast = 2)]
        public string MedicareId { get; set; }

        [LogMasked(PreserveLength = true, ShowFirst = 1, ShowLast = 2)]
        public string MemberId { get; set; }
    }
```
Please refer to Destructurama's [github](https://github.com/destructurama/attributed) for additional documentation and future enhacements that may become available upon refreshing this package.

## W3C Trace Context
By default all of our services (when utilizing webcore) will propagate trace identifiers.   Tracing is critical because it allows us to tie together logs and events from a single request as it crosses boundaries of microservices either through HTTP or Pub/Sub messages.
It is highly recommended that any developer read and familiarize themselves with the [W3C Trace Context](https://www.w3.org/TR/trace-context).

### Trace Identifiers
There are four trace identifiers within .Net Core that are used for tracing requests.
1. TraceId - is globally unique for a request.  When an http request or message topic is created a unique identifier is generated that will permeate through each log allowing us to track the request as it crosses microservice boundaries.
1. RequestId - is a unique identifier inside of a single service.
1. SpanId - Is similar to a `RequestId` (that it is unique to the request within the application) but adheres strictly to the W3C standard for length and uniqueness requirements.
1. ParentId - Is a concatenation of the `TraceId-Spanid` with a 2 digit flag known as sampling (this isn't an important detail as far as we are concerned).  Each time the request travels through a new application, that application will add it's `SpandId` to the parent when it is sent to a new service.

Example of request without parent.
```
   "RequestId":"0HM4ON1UQH83M:00000002",
   "SpanId":"1528a70aa5b4db41",
   "ParentId":"0000000000000000",
   "TraceId":"c0845941f9cf1d46bea2cb159a22e99a",
```

Example of request with parent.
```
   "RequestId":"1811122227684838",
   "SpanId":"5b45d6972c861f4f",
   "ParentId":"00-7aa2ee2b2d582741942c910c8087ea3f-49fbd672ff7921d7-00",
   "TraceId":"7aa2ee2b2d582741942c910c8087ea3f",
```

### Pub/Sub
In order to trace requests that produce Published Messages, the GCP published message includes the the 4 trace identifiers as `Attributes`.  These attributes are then ingested by the Subscriber and a new `Diagnostic.Activity` (which represents an operation with context for logging by building out the trace identifiers) is created for each message ingested.

### HTTP
HTTP requests follow the same pattern as Pub/Sub except they are passed as headers instead of attributes.  Fortunately .Net Core 3.0+ takes care of all of the implementation details right out-of-the-box.  It will pull the tracer identifiers from the `Diagnostic.Activity` and add them to the HTTP request.

## Secrets Management
Microservices leverage Google Cloud Platform's Secret Manager (GSM) for storing and retrieving secrets. Secrets are pulled at runtime during the application bootstrapping process (startup) while building the configuration file using [`IConfigurationBuilder` extension method](src/framework/Framework.SecretsManager/Extensions/ConfigurationBuilderExtensions.cs).
Secrets are maintained by developers in Gitlab's CI/CD variables each with an underscore indicating the specific environment the secrets will be utilized in.
- SECRETS_dev -> DEV (NP)
- SECRETS_sit -> SIT
- SECRETS_uat -> UAT
- SECRETS_prod -> PROD (Production)

During the continuous deployment process secrets are loaded from Gitlab to GSM prior to the application being deployed ensuring they are loaded once the application (K8 Workload) starts the bootstrapping process.

### Accessing in Code
Accessing secrets in code is done by the same method as accessing any other configuration value by utilizing the (Options Pattern)[https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0#use-ioptionssnapshot-to-read-updated-data].

### Using secrets locally
Each .Net Solution (repository) has (launchSettings.json)[example/src/WebApi/Properties/launchSettings.json] file that contains configuration settings for running locally.
Under the `environmentVariables` section a location to locally stored secrets is defined for `APP_ROOT` which will load a file named `config/appsettings.overrides.json`. Developers should store their secrets in this location. Storing secrets outside of the repository is a best practice for it prevents the possibility of the secrets accidentally being committed to source control.
Storing secrets in this manner allows for the developer to store local secrets for each Solution they work in.

#### **Be sure to update the folder name when creating a new project**.
Example:
- Api Template path = "APP_ROOT": "/Users/%USER%/dev/approots/api-template"
- New Solution = "APP_ROOT": "/Users/%USER%/dev/approots/new-solution-name"

The overrides file then should be in `/Users/%USER%/dev/approots/new-solution-name/config/appsettings.overrides.json`

## Distributed Caching - Redis
Webcore utilizes [StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/) as a redis client for caching. This is hidden behind the `IDistributedCache` interface.
The wrapper for service registration will set the implementation of `IDistributedCache` to `MemoryDistributedCache` when the .Net Core environment is set to `Development` to allow for developers
to quickly setup their local development environment by using in memory caching when the application is run locally.
Here are the steps outlined to leverage this library.
1. [Create the appsettings configuration](example/src/WebApi/appsettings.json#L56)
2. [Register the service with the Configuration Section Key](example/src/WebApi/Startup.cs#41)
3. [Inject IDistributedCache](example/src/Domain/CatFactsService.cs)

There are also some custom extension methods in `DistributedCacheExtensions.cs` that will handle serializing and deserializing the cache payload as well as combine the cache hit/miss retrieval into a single method `GetOrCreateAsync`. 

###_Beware_ that if a cache invalidation policy is not set then the cache will never expire. A lack of setting an expiration means that we do not want the cache to ever expire.
## Packing Webcore and restoring it locally
In order to perform development locally for Webcore libraries it is necessary to build, pack, and restore the libraries from your local machine. To simplify this process perform the following steps.

### Pack
Add the following function to your `.bashrc` or `.zshrc` file

```
function pack() {
 dotnet pack /p:Channel=alpha /p:BuildTimestamp=$(date -u '+%Y%m%d%H%M%S') -c Release -o /Users/$USER/.nuget/humanaedge
}
```
Nuget uses semantic versioning and thus when adding a build timestamp will make this local build the "latest" version available.

### Restore
Under the packageSource element in the NuGet.Config file located [here](https://docs.microsoft.com/en-us/nuget/consume-packages/configuring-nuget-behavior#config-file-locations-and-uses).
```
<packageSources>
    <add key="local-humana-edge" value="/Users/{enter-your-username}/.nuget/humanaedge" />
</packageSources>
```
Now when you restore your application that consumes Webcore locally it will utilize this local location to restore packages from.

### Steps
- Navigate to the root of Webcore's source code locally and run the above function of `pack`
    - `cd source/repos/webcore`
    - `pack`
- Now that the packages have been created and stored locally we must restore them. Navigate to the root of the consuming API.
    - `cd source/repos/ah-crm-document-upload`
    - `dotnet restore`
- Now that we have ran this in the background we need to refresh our IDE to pick up the changes
    - Right click on the solution in your IDE and unload the packages
    - Then reload the packages