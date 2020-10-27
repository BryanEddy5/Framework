using System.Linq;
using FluentValidation;
using HumanaEdge.Webcore.Core.DependencyInjection;
using HumanaEdge.Webcore.Core.DependencyInjection.Validators;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.DependencyInjection.Extensions;
using HumanaEdge.Webcore.Framework.DependencyInjection.Tests.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace HumanaEdge.Webcore.Framework.DependencyInjection.Tests
{
    /// <summary>
    /// Unit tests for <see cref="DependencyInjection" />.
    /// </summary>
    public class DependencyInjectionTests : BaseTests
    {
        private readonly IHost _host;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public DependencyInjectionTests()
        {
            _host = Host.CreateDefaultBuilder()
                .UseDependencyInjection<DependencyInjectionTests>()
                .Build();
        }

        /// <summary>
        /// Verifies the behavior of <see cref="DependencyInjectedComponentAttribute" /> with all
        /// <see cref="LifetimeScopeEnum.Scoped" /> values.
        /// </summary>
        [Fact]
        public void GetService_IScopedService()
        {
            // arrange

            // act
            var actual = _host.Services.GetService<IScopedService>();
            var actual2 = _host.Services.GetService<IScopedService>();

            // assert
            Assert.NotNull(actual);
            Assert.NotNull(actual2);
            Assert.IsType<ScopedComponent>(actual);
            Assert.IsType<ScopedComponent>(actual2);
            Assert.Equal(actual, actual2);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="DependencyInjectedComponentAttribute" /> with all
        /// <see cref="LifetimeScopeEnum.Singleton" /> values.
        /// </summary>
        [Fact]
        public void GetService_ISingletonService()
        {
            // arrange

            // act
            var actual = _host.Services.GetService<ISingletonService>();
            var actual2 = _host.Services.GetService<ISingletonService>();

            // assert
            Assert.NotNull(actual);
            Assert.NotNull(actual2);
            Assert.IsType<SingletonComponent>(actual);
            Assert.IsType<SingletonComponent>(actual2);
            Assert.Equal(actual, actual2);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="DependencyInjectedComponentAttribute" /> with all
        /// <see cref="LifetimeScopeEnum.Transient" /> values.
        /// </summary>
        [Fact]
        public void GetService_ITransientService()
        {
            // arrange

            // act
            var actual = _host.Services.GetService<ITransientService>();
            var actual2 = _host.Services.GetService<ITransientService>();

            // assert
            Assert.NotNull(actual);
            Assert.NotNull(actual2);
            Assert.IsType<TransientComponent>(actual);
            Assert.IsType<TransientComponent>(actual2);
            Assert.NotEqual(actual, actual2);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="DependencyInjectedComponentAttribute" /> with no implementation.
        /// </summary>
        [Fact]
        public void GetService_NonRegisteredComponent()
        {
            // arrange

            // act
            var actual = _host.Services.GetService<INonRegisteredComponent>();

            // assert
            Assert.Null(actual);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="DependencyInjectedComponentAttribute" /> for multiple implementations of
        /// an interface.
        /// </summary>
        [Fact]
        public void MultipleServicesRegistration()
        {
            // arrange act
            var actual = _host.Services.GetServices<IMultiService>().ToArray();

            // assert
            Assert.NotEmpty(actual);
            Assert.IsType<MultiService1>(actual[0]);
            Assert.IsType<MultiService2>(actual[1]);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="ValidatorAttribute"/>.
        /// </summary>
        [Fact]
        public void ValidatorServiceRegistration()
        {
            // arrange
            var fakeFoo = new Foo { Name = null };

            // act
            var actual = _host.Services.GetService<IValidator<Foo>>();
            var validationResult = actual.Validate(fakeFoo);

            // act
            Assert.NotNull(actual);
            Assert.IsType<FooValidator>(actual);
            Assert.False(validationResult.IsValid);
        }

#pragma warning disable SA1201 // Elements should appear in the correct order
        /// <summary>
        /// Test service interface for transient component.
        /// </summary>
        public interface ITransientService
        {
        }

        /// <summary>
        /// Test service interface for scoped component.
        /// </summary>
        public interface IScopedService
        {
        }

        /// <summary>
        /// Test service interface for singleton component.
        /// </summary>
        public interface ISingletonService
        {
        }

        /// <summary>
        /// Test service interface for non-registered component.
        /// </summary>
        public interface INonRegisteredComponent
        {
        }

        /// <summary>
        /// Test component for testing service with multiple components.
        /// </summary>
        public interface IMultiService
        {
        }

        /// <summary>
        /// Test component for dependency injection with transient lifetime.
        /// </summary>
        [DependencyInjectedComponent]
        public class TransientComponent : ITransientService
        {
        }

        /// <summary>
        /// Test component for dependency injection with scoped lifetime.
        /// </summary>
        [DependencyInjectedComponent(LifetimeScopeEnum.Scoped)]
        public class ScopedComponent : IScopedService
        {
        }

        /// <summary>
        /// Test component for dependency injection with singleton lifetime.
        /// </summary>
        [DependencyInjectedComponent(LifetimeScopeEnum.Singleton)]
        public class SingletonComponent : ISingletonService
        {
        }

        /// <summary>
        /// Test component for testing non-registration of component.
        /// </summary>
        public class NonRegisteredComponent : INonRegisteredComponent
        {
        }

        /// <summary>
        /// Test component for testing non-registration of component.
        /// </summary>
        [DependencyInjectedComponent]
        public class MultiService1 : IMultiService
        {
        }

        /// <summary>
        /// Test component for testing non-registration of component.
        /// </summary>
        [DependencyInjectedComponent]
        public class MultiService2 : IMultiService
        {
        }
#pragma warning restore SA1201 // Elements should appear in the correct order
    }
}