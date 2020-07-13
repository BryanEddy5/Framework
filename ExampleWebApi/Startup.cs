using HumanaEdge.Webcore.Framework.Web;
using Microsoft.Extensions.Configuration;

namespace ExampleWebApi
{
    /// <summary>
    /// ExampleWebApi's Startup class.
    /// </summary>
    public class Startup : BaseStartup<Startup>
    {
        /// <summary>
        /// designated ctor.
        /// </summary>
        /// <param name="configuration"><see cref="IConfiguration"/>.</param>>
        public Startup(IConfiguration configuration)
            : base(configuration)
        {
        }
    }
}