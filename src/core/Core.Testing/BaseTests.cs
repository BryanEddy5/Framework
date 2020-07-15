using System;
using System.Threading;
using AutoFixture;
using Moq;

namespace HumanaEdge.Webcore.Core.Testing
{
    /// <summary>
    /// A base class to be leveraged in other solutions that has a common configuration for all unit tests.
    /// </summary>
    public class BaseTests : IDisposable
    {
        /// <summary>
        /// Creates the mock repository and runs any other test setup (via overrides).
        /// </summary>
        public BaseTests()
        {
            Moq = new MockRepository(MockBehavior.Strict);
            CancellationTokenSource = new CancellationTokenSource();
            FakeData = new Fixture();
        }

        /// <summary>
        /// A cancellation token source for unit tests of async code.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; }

        /// <summary>
        /// Generates fake data and services used for testing.
        /// </summary>
        public Fixture FakeData { get; }

        /// <summary>
        /// A standard configuration of to establish a strict behavior for all mocked services.
        /// </summary>
        public MockRepository Moq { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            Moq.VerifyAll();
            CancellationTokenSource?.Dispose();
        }
    }
}