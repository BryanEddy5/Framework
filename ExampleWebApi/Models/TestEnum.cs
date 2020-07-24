using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleWebApi.Models
{
    /// <summary>
    /// TestEnum is used to test "treat enums as strings" in our APIs, both controller methods and swagger doc.
    /// </summary>
    public enum TestEnum
    {
        /// <summary>
        /// Red.
        /// </summary>
        Red,

        /// <summary>
        /// Yellow.
        /// </summary>
        Yellow,

        /// <summary>
        /// Green.
        /// </summary>
        Green,

        /// <summary>
        /// Blue.
        /// </summary>
        Blue
    }
}