## [1.3.0]
### Added
- new `RestClientOptions.Builder` option: `.ConfigureMiddlewareAsync()`<br/>
  for adding asynchronous operations that should happen with every outgoing request.

### Changed
- Upgraded all libraries to multi-target both .Net 5.0 and .Net 3.1 or .Net Standard 2.1.
  This allows consumers to upgrade to .Net 5.0 at their own pace.
  All consumers will reap the benefits of the latest releases of Webcore without having to upgrade to .Net 5.0.

### Fixed
- Use of generics for the generic type of `PublisherClient`, `SubscriptionClient`, and `SecretsService` would fetch only a single configuration due to using the `Name` property of the type and not the unique `FullName` property.
Ex. `IPublisherClient<Generic<Foo>>` and `IPublisherClient<Generic<Bar>>` would fetch the same configuration settings due to the `nameof(Generic).Name` being equivalent for each instance.