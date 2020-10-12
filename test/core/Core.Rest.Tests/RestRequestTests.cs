using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AutoFixture;
using HumanaEdge.Webcore.Core.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Xunit;

namespace HumanaEdge.Webcore.Core.Rest.Tests
{
    /// <summary>
    /// Unit tests for <see cref="RestRequest" />.
    /// </summary>
    public class RestRequestTests : BaseTests
    {
        /// <summary>
        /// Validates the behavior of <see cref="RestRequest.UseAcceptHeader(MediaType)" />.
        /// </summary>
        [Fact]
        public void AddAcceptHeader_RestRequest()
        {
            // arrange
            var fakeMediaType = MediaType.Json;
            var fakeRelativePath = FakeData.Create<string>();
            var fakeHttpMethod = FakeData.Create<HttpMethod>();
            var restRequest = new RestRequest(fakeRelativePath, fakeHttpMethod)
                .UseAcceptHeader(fakeMediaType);
            var expected = new[] { fakeMediaType.MimeType };

            // act
            var actual = restRequest.Headers["Accept"];

            // assert
            Assert.Equal(expected, actual.ToArray());
        }

        /// <summary>
        /// Validates the behavior of <see cref="RestRequest.UseAcceptHeader(MediaType)" />.
        /// </summary>
        [Fact]
        public void AddMultipleAcceptHeader_RestRequest()
        {
            // arrange
            var fakeMediaTypes = new[] { MediaType.Json, MediaType.Json };
            var fakeRelativePath = FakeData.Create<string>();
            var fakeHttpMethod = FakeData.Create<HttpMethod>();
            var restRequest = new RestRequest(fakeRelativePath, fakeHttpMethod);
            foreach (var mediaType in fakeMediaTypes)
            {
                restRequest.UseAcceptHeader(mediaType);
            }

            var expectedMediaTypes = fakeMediaTypes.Select(x => x.MimeType).ToArray();

            // act
            var actual = restRequest.Headers["Accept"];

            // assert
            Assert.Equal(expectedMediaTypes, actual.ToArray());
        }

        /// <summary>
        /// Validates the behavior of creating a <see cref="RestRequest" /> and setting the properties.
        /// </summary>
        [Fact]
        public void CreateNewRestRequest_ValidatePropertiesSet()
        {
            // arrange
            var expectedRelativePath = FakeData.Create<string>();
            var expectedHttpMethod = FakeData.Create<HttpMethod>();

            // act
            var actual = new RestRequest(expectedRelativePath, expectedHttpMethod);

            // assert
            Assert.Equal(expectedRelativePath, actual.RelativePath);
            Assert.Equal(expectedHttpMethod, actual.HttpMethod);
        }

        /// <summary>
        /// Validates the behavior of <see cref="RestRequest.UseHeader(string, string)" />.
        /// </summary>
        [Fact]
        public void UseHeader_AddHeaderToRequest_Validation()
        {
            // arrange
            var fakeHeaderKey = FakeData.Create<string>();
            var expectedHeaderValues = FakeData.Create<string[]>();
            var fakeRelativePath = FakeData.Create<string>();
            var fakeHttpMethod = FakeData.Create<HttpMethod>();
            var restRequest = new RestRequest(fakeRelativePath, fakeHttpMethod);
            foreach (var header in expectedHeaderValues)
            {
                restRequest.UseHeader(fakeHeaderKey, header);
            }

            // act
            var actual = restRequest.Headers[fakeHeaderKey];

            // assert
            Assert.Equal(expectedHeaderValues, actual.ToArray());
        }

        /// <summary>
        /// Validates the behavior of <see cref="RestRequest.AddQueryParams(IDictionary{string,string})" />
        /// for a query string with a collection.
        /// </summary>
        [Fact]
        public void UseQueryParams_AddQueryParamsToRequest_Collection_Validation()
        {
            // arrange
            var fakeQueryParams = FakeData.Create<Dictionary<string, string>>();
            var fakeQueryParamsCollection = new Dictionary<string, string>();

            foreach (var kvp in fakeQueryParams)
            {
                var fakeDictionaryValue = FakeData.Create<string>();
                fakeQueryParamsCollection[kvp.Key] = string.Join(",", fakeQueryParams[kvp.Key], fakeDictionaryValue);
            }

            var fakeRelativePath = FakeData.Create<string>();
            var fakeHttpMethod = FakeData.Create<HttpMethod>();
            var restRequest = new RestRequest(fakeRelativePath, fakeHttpMethod);
            restRequest.AddQueryParams(fakeQueryParamsCollection);
            var baseUri = new Uri("https://www.testing.com");
            var uri = new Uri(baseUri, restRequest.RelativePath);

            // act
            var actual = QueryHelpers.ParseQuery(uri.Query).ToDictionary(x => x.Key, x => x.Value.ToString());

            // assert
            Assert.Equal(actual, fakeQueryParamsCollection);
        }

        /// <summary>
        /// Validates the behavior of <see cref="RestRequest.AddQueryParams(IDictionary{string,string})" />.
        /// </summary>
        [Fact]
        public void UseQueryParams_AddQueryParamsToRequest_Validation()
        {
            // arrange
            var fakeQueryParams = FakeData.Create<Dictionary<string, string>>();
            var fakeRelativePath = FakeData.Create<string>();
            var fakeHttpMethod = FakeData.Create<HttpMethod>();
            var restRequest = new RestRequest(fakeRelativePath, fakeHttpMethod);
            restRequest.AddQueryParams(fakeQueryParams);
            var baseUri = new Uri("https://www.testing.com");
            var uri = new Uri(baseUri, restRequest.RelativePath);

            // act
            var actual = QueryHelpers.ParseQuery(uri.Query).ToDictionary(x => x.Key, x => x.Value.ToString());

            // assert
            Assert.Equal(actual, fakeQueryParams);
        }

        /// <summary>
        /// Validates the behavior of <see cref="RestRequest.AddQueryParams(IDictionary{string,string})" />
        /// when duplicate keys are present.
        /// </summary>
        [Fact]
        public void UseQueryParams_AddQueryParamsToRequestWithDuplicateKey_Validation()
        {
            // arrange
            var fakeKey = FakeData.Create<string>();
            var fakeQueryStringValue1 = FakeData.Create<string>();
            var fakeQueryStringValue2 = FakeData.Create<string>();
            var fakeRelativePath = FakeData.Create<string>();
            var fakeHttpMethod = FakeData.Create<HttpMethod>();
            var restRequest = new RestRequest(fakeRelativePath, fakeHttpMethod);
            var baseUri = new Uri("https://www.testing.com");
            restRequest.AddQueryParams(fakeKey, fakeQueryStringValue1);
            restRequest.AddQueryParams(fakeKey, fakeQueryStringValue2);
            var uri = new Uri(baseUri, restRequest.RelativePath);

            var expected = string.Join(",", fakeQueryStringValue1, fakeQueryStringValue2);

            // act
            var actual = QueryHelpers.ParseQuery(uri.Query).ToDictionary(x => x.Key, x => x.Value.ToString());

            // assert
            Assert.Equal(actual.First().Value, expected);
        }

        /// <summary>
        /// Validates the behavior of <see cref="RestRequest.AddQueryParams(IDictionary{string,string})" />
        /// when duplicate keys are present.
        /// </summary>
        [Fact]
        public void UseQueryParams_AddQueryParamsToRequestWithDuplicateKeyInDictionary_Validation()
        {
            // arrange
            var fakeQueryParams = FakeData.Create<Dictionary<string, string>>();
            var fakeQueryParamsDuplicateKeys = new Dictionary<string, string>();
            foreach (var kvp in fakeQueryParams)
            {
                fakeQueryParamsDuplicateKeys.Add(kvp.Key, FakeData.Create<string>());
            }

            var fakeRelativePath = FakeData.Create<string>();
            var fakeHttpMethod = FakeData.Create<HttpMethod>();
            var restRequest = new RestRequest(fakeRelativePath, fakeHttpMethod);
            var baseUri = new Uri("https://www.testing.com");
            restRequest.AddQueryParams(fakeQueryParams);
            restRequest.AddQueryParams(fakeQueryParamsDuplicateKeys);
            var uri = new Uri(baseUri, restRequest.RelativePath);

            var expectedDictionary = fakeQueryParams.ToDictionary(
                kvp => kvp.Key,
                kvp =>
                    string.Join(
                        ",",
                        kvp.Value,
                        fakeQueryParamsDuplicateKeys[kvp.Key]));

            // act
            var actual = QueryHelpers.ParseQuery(uri.Query).ToDictionary(x => x.Key, x => x.Value.ToString());

            // assert
            Assert.Equal(actual, expectedDictionary);
        }
    }
}