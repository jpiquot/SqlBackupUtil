using System;
using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Backup directories Option. Implements the <see cref="Option{DateTime}"/>
    /// </summary>
    /// <seealso cref="Option{DateTime}"/>
    internal class BeforeOption : Option<DateTime?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeOption"/> class.
        /// </summary>
        /// <param name="defaultValues">The default values.</param>
        public BeforeOption(IOptions<SqlBackupSettings> defaultValues)
            : base(
                  new[] { "--before", "-b" },
                  () => defaultValues.Value.Before,
                  "Only backup files that are before the given date.")
        {
        }
    }
}