using System;
using System.Runtime.Serialization;

namespace Doppler.Extensions.Configuration
{
    /// <summary>
    ///  Represents Doppler specific errors that occur during application execution.
    /// </summary>
    [Serializable]
    internal class DopplerException : Exception
    {
        public DopplerException()
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <seealso cref="DopplerException"/> class with a specified error.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>

        public DopplerException(string message) : base(message)
        {
        }

    }
}