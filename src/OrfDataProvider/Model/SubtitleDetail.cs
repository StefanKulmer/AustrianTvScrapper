using System;
using System.Collections.Generic;
using System.Text;

namespace OrfDataProvider.Model
{
    public class SubtitleDetail
    {
        public required SubtitleType Type { get; init; }
        public required string Url { get; init; }
        public required byte[] Data { get; init; }
    }
}
