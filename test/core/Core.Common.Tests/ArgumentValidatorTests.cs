using System;
using AutoFixture;
using HumanaEdge.Webcore.Core.Common.Tests.Serialization.Stubs;
using HumanaEdge.Webcore.Core.Common.Validators;
using HumanaEdge.Webcore.Core.Testing;
using Xunit;

namespace HumanaEdge.Webcore.Core.Common.Tests
{
    /// <summary>
    /// Unit tests for <see cref="ArgumentValidator" />.
    /// </summary>
    public class ArgumentValidatorTests : BaseTests
    {
        /// <summary>
        /// Validates the behavior of <see cref="ArgumentValidator.AssertNotNullOrEmpty{T}(T)" />
        /// for a string that is not empty or null.
        /// </summary>
        [Fact]
        public void EmptyString_NoException_Foo()
        {
            // arrange
            var fakeString = FakeData.Create<Foo>();

            // act
            var actual = fakeString.AssertNotNullOrEmpty();

            // assert
            Assert.NotNull(actual);
        }

        /// <summary>
        /// Validates the behavior of <see cref="ArgumentValidator.AssertNotNullOrEmpty{T}(T)" />
        /// for a string that is not empty or null.
        /// </summary>
        [Fact]
        public void EmptyString_NoException_String()
        {
            // arrange
            var fakeString = FakeData.Create<string>();

            // act assert
            var actual = fakeString.AssertNotNullOrEmpty();
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }

        /// <summary>
        /// Validates the behavior of <see cref="ArgumentValidator.AssertNotNullOrEmpty{T}(T)" /> for
        /// empty strings.
        /// </summary>
        [Fact]
        public void EmptyString_ThrowsException()
        {
            // arrange
            var nullString = string.Empty;

            // act assert
            Assert.Throws<ArgumentNullException>(() => nullString.AssertNotNullOrEmpty());
        }

        /// <summary>
        /// Validates the behavior of <see cref="ArgumentValidator.AssertNotNullOrEmpty{T}(T)" /> for
        /// null objects.
        /// </summary>
        [Fact]
        public void NullObject_ThrowsException()
        {
            // arrange
            object nullObject = null;

            // act assert
            Assert.Throws<ArgumentNullException>(() => nullObject!.AssertNotNullOrEmpty());
        }

        /// <summary>
        /// Validates the behavior of <see cref="ArgumentValidator.AssertNotNullOrWhiteSpace(string)" />
        /// for a null string.
        /// </summary>
        [Fact]
        public void NullString_NoException()
        {
            // arrange
            string fakeString = null;

            // act assert
            Assert.Throws<ArgumentException>(() => fakeString!.AssertNotNullOrWhiteSpace());
        }

        /// <summary>
        /// Validates the behavior of <see cref="ArgumentValidator.AssertNotNullOrEmpty{T}(T)" /> for
        /// null strings.
        /// </summary>
        [Fact]
        public void NullString_ThrowsException()
        {
            // arrange
            string nullString = null;

            // act assert
            Assert.Throws<ArgumentNullException>(() => nullString!.AssertNotNullOrEmpty());
        }

        /// <summary>
        /// Validates the behavior of <see cref="ArgumentValidator.AssertNotNullOrWhiteSpace(string)" />
        /// for a string that has whitespace.
        /// </summary>
        [Fact]
        public void WhitespaceString_Exception()
        {
            // arrange
            var fakeString = " ";

            // act assert
            Assert.Throws<ArgumentException>(() => fakeString.AssertNotNullOrWhiteSpace());
        }

        /// <summary>
        /// Validates the behavior of <see cref="ArgumentValidator.AssertNotNullOrWhiteSpace(string)" />.
        /// </summary>
        [Fact]
        public void WhitespaceString_NoException()
        {
            // arrange
            var fakeString = FakeData.Create<string>();
            var expected = " ";

            // act assert
            var actual = fakeString.AssertNotNullOrWhiteSpace();

            // assert
            Assert.NotEqual(expected, actual);
        }
    }
}