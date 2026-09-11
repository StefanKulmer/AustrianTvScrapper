using System;
using System.Collections.Generic;
using System.Text;

namespace OrfDataProvider
{
    public class SubtitleType
    {
        public static readonly SubtitleType Sami = new SubtitleType { Prefix = "sami" };
        public static readonly SubtitleType Srt = new SubtitleType { Prefix = "srt" };
        public static readonly SubtitleType Stl = new SubtitleType { Prefix = "stl" };
        public static readonly SubtitleType Ttml= new SubtitleType { Prefix = "ttml" };
        public static readonly SubtitleType Vtt = new SubtitleType { Prefix = "vtt" };
        public static readonly SubtitleType Xml = new SubtitleType { Prefix = "xml" };

        public static readonly SubtitleType[] All =
[
    Sami,
            Srt,
            Stl,
            Ttml,
            Vtt,
            Xml
];

        private SubtitleType()
        {
        }

        public required string Prefix { get; init; }
    }
}
