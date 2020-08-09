using System;
using System.Collections.Generic;
using System.CommandLine.Rendering;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBackupUtil
{
    internal static class Colorizer
    {
        public static TextSpan Underline(this string value) =>
            new ContainerSpan(StyleSpan.UnderlinedOn(),
                              new ContentSpan(value),
                              StyleSpan.UnderlinedOff());

        public static TextSpan Bold(this string value) =>
            new ContainerSpan(StyleSpan.BoldOn(),
                              new ContentSpan(value),
                              StyleSpan.BoldOff());
        public static TextSpan BoldWhite(this string value) =>
             new ContainerSpan(
                 ForegroundColorSpan.White(),
                 StyleSpan.BoldOn(),
                 new ContentSpan(value),
                 StyleSpan.BoldOff(),
                 ForegroundColorSpan.Reset()
                 );


        public static TextSpan Rgb(this string value, byte r, byte g, byte b) =>
            new ContainerSpan(ForegroundColorSpan.Rgb(r, g, b),
                              new ContentSpan(value),
                              ForegroundColorSpan.Reset());

        public static TextSpan LightGreen(this string value) =>
            new ContainerSpan(ForegroundColorSpan.LightGreen(),
                              new ContentSpan(value),
                              ForegroundColorSpan.Reset());

        public static TextSpan LightBlue(this string value) =>
            new ContainerSpan(ForegroundColorSpan.LightBlue(),
                              new ContentSpan(value),
                              ForegroundColorSpan.Reset());

        public static TextSpan White(this string value) =>
            new ContainerSpan(ForegroundColorSpan.White(),
                              new ContentSpan(value),
                              ForegroundColorSpan.Reset());
        public static TextSpan Orange(this string value) =>
            new ContainerSpan(ForegroundColorSpan.Rgb(255, 128, 0),
                              new ContentSpan(value),
                              ForegroundColorSpan.Reset());
        public static TextSpan DarkOrange(this string value) =>
            new ContainerSpan(ForegroundColorSpan.Rgb(128, 64, 0),
                              new ContentSpan(value),
                              ForegroundColorSpan.Reset());
        public static TextSpan DarkGrey(this string value) =>
            new ContainerSpan(ForegroundColorSpan.Rgb(100, 100, 100),
                              new ContentSpan(value),
                              ForegroundColorSpan.Reset());
    }
}
