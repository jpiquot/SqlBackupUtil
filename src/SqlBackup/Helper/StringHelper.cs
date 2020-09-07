using System;
using System.Globalization;

namespace SqlBackup
{
    internal static class StringHelper
    {
        private const string sqlFormat = "yyyy-MM-dd HH:mm:ss";

        internal static string? Format(this string? format, params object?[] args)
            => (format == null) ? null : string.Format(CultureInfo.CurrentCulture, format, args);

        internal static string? ToSqlString(this DateTime? date)
            => date?.ToString(sqlFormat, CultureInfo.InvariantCulture);
    }
}