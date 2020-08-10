using System;

namespace SqlBackupUtil
{
    /// <summary>
    /// Check error exception
    /// </summary>
    public class CheckException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CheckException()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CheckException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CheckException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
