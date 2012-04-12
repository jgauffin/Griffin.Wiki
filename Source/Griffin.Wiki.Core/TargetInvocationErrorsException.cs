using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Wiki.Core
{
    /// <summary>
    /// Thrown when one or more exceptions occurred during an operation which triggers multiple components (such as publishing events)
    /// </summary>
    public class TargetInvocationErrorsException : Exception
    {
        private readonly IEnumerable<Exception> _exceptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetInvocationErrorsException"/> class.
        /// </summary>
        /// <param name="msg">What you did when the exception(s) were thrown.</param>
        /// <param name="exceptions">The exceptions.</param>
        public TargetInvocationErrorsException(string msg, IEnumerable<Exception> exceptions)
            : base(msg)
        {
            if (msg == null) throw new ArgumentNullException("msg");
            if (exceptions == null) throw new ArgumentNullException("exceptions");
            _exceptions = exceptions;
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <returns>The error message that explains the reason for the exception, or an empty string("").</returns>
        public override string Message
        {
            get
            {
                var errors = string.Join("\r\n**********************\r\n", Exceptions.Select(x => x.ToString()));
                return base.Message + "\r\nAll exceptions:\r\n" + errors;
            }
        }

        /// <summary>
        /// Gets all thrown exceptions
        /// </summary>
        public IEnumerable<Exception> Exceptions
        {
            get { return _exceptions; }
        }
    }
}
