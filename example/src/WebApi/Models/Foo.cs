using System.ComponentModel.DataAnnotations;

namespace HumanaEdge.Webcore.Example.WebApi.Models
{
    /// <summary>
    /// Here's some Foo.
    /// </summary>
    public class Foo
    {
        /// <summary>
        /// Some nested property.
        /// </summary>
        public BuzzClass? Buz { get; set; }

        /// <summary>
        /// A top level property.
        /// </summary>
        [Required]
        public string? Bar { get; set; }

        /// <summary>
        /// The class for that nested property.
        /// </summary>
        public class BuzzClass
        {
            /// <summary>
            /// The property.
            /// </summary>
            public string? Bar { get; set; }
        }
    }
}