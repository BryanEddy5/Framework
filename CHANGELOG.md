# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
Additional information on the libraries is located in the README.md

## [1.3.0]
### Added
- new pair of `SOAP` projects for building `SOAP`-ey integrations.
- new configurable `Alert` functionality for our `Telemetry`.
- new `RestClientOptions.Builder` option: `.ConfigureMiddlewareAsync()`<br/>
  for adding asynchronous operations that should happen with every outgoing request.
- Structured logging for `MessageAppException` has been added.  Exceptions can now utilize structured logging for the message payload.
- `BadRequestResponse` for deserializing 400 Bad requests.
- GCP Pub/Sub Susbcribers - Maximum number of retries of a message for the subscription to prevent immediate and almost infinite number of retries that is the default behavior of GCP Subscriptions.
- GCP Pub/Sub Subscribers - All exceptions are now published to a bucket via a middleware implementation.

### Changed
- Upgraded all libraries to multi-target both .Net 5.0 and .Net 3.1 or .Net Standard 2.1.
  This allows consumers to upgrade to .Net 5.0 at their own pace.
  All consumers will reap the benefits of the latest releases of Webcore without having to upgrade to .Net 5.0.
- The standard ResiliencyPolicy used by our microservice REST clients is now baked into RestClientOptions.Builder.
  This allows our clients to use that same default ResiliencePolicy without having to explicitly set it up themselves.

### Fixed
- Use of generics for the generic type of `PublisherClient`, `SubscriptionClient`, and `SecretsService` would fetch only a single configuration due to using the `Name` property of the type and not the unique `FullName` property.
Ex. `IPublisherClient<Generic<Foo>>` and `IPublisherClient<Generic<Bar>>` would fetch the same configuration settings due to the `nameof(Generic).Name` being equivalent for each instance.
  
## [1.2.0]
### Add
- Inject Secrets from GCP Secrets Manager
- Pub/Sub W3C Trace Context added to all Webcore published message and subscribtions
- PII sanitized logging
- Integration Test library added for running integration tests
    - `ITestClientFactory` generates a named client for sending http requests.
    - `ITestClient` simplifies sending http requests
    - `ITestData` for retrieving environment specific test data and configurations.
    
### Fixed
- Moved Request middleware for reporting telemetry to the front of the http pipeline to properly report http status codes and messages set by the exception handling middleware.

## [1.1.0]
### Added
- Secrets Manager for fetching secrets from GCP Secrets Manager
- Subscriber client for subscribing to GCP Pub/Sub Subscriptions
- `PublisherClient<TMessage>` for publishing to GCP Pub/Sub Topics
- `DiComponent` that allows for type targeting for dependency injection service registration
- `ValidatorAttribute` for registering FluentValidation validator classes.
- Telemetry for both Subscriptions and Publications
- RestClient FormUrlEncoding for request bodies.
- `DiOptions` for registering Options Pattern configurations.
- `PubSubException` gives developers control over the acking/nacking of a message.

### Deprecated
- `DependencyInjectedComponent` replaced by `DiComponent`

## [1.0.0]
### Added
- Creation of a centralized repository containing the framework for other services to be built upon.
- Examples API for the Webcore libraries
- `RestClient` for making RESTful requests and multiple content types.
- `BaseStartup<TEntry>` for consolidating application configuration and bootsrapping for all microservices
    - Swagger
    - Exception handling middleware
    - Request Telemetry
- Telemetry for outbound and inbound http requests
- Encryption service for encrypting/decrypting using the GCP KMS
- Dependency injection registration through the use of `DependencyInjectedComponent`
- `MessageAppException` for communicating http status codes and messages to consumers
- Pagination for standardized paginated responses
- `BaseTests` simplifying Unit Testing for developers.
- Logging centralized with base configuration and allows for developers to extend the configruation.


