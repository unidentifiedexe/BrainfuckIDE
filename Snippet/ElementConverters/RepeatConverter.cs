#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Snippets.ElementConverters
{

    [DataContract]
    public class RepeatConverter : IElementConverter
    {

        private string? _startBracket;
        private string? _endBracket;
        private string? _startString;
        private string? _endString;
        private string? _seedString;
        private string? _intervalString;
        private string? _name;

        [DataMember]
        public bool IsIgnoreBracketIfZero { get; set; }
        [DataMember]
        public string StartBracket { get => _startBracket ?? string.Empty; set => _startBracket = value; }
        [DataMember]
        public string EndBracket { get => _endBracket ?? string.Empty; set => _endBracket = value; }
        [DataMember]
        public string StartString { get => _startString ?? string.Empty; set => _startString = value; }
        [DataMember]
        public string EndString { get => _endString ?? string.Empty; set => _endString = value; }
        [DataMember]
        public string SeedString { get => _seedString ?? string.Empty; set => _seedString = value; }
        [DataMember]
        public string IntervalString { get => _intervalString ?? string.Empty; set => _intervalString = value; }

        [DataMember]
        public string Name { get => _name ?? string.Empty; set => _name = value; }

        public string Convert(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            if (!(int.TryParse(text, out var num))) return $"{StartBracket}{text}{EndBracket}";
            if (num < 0) return text;
            if (num == 0 && IsIgnoreBracketIfZero)
                return $"{StartString}{EndString}";

            var mainStr = string.Join(IntervalString, Enumerable.Repeat(SeedString, num));
            return $"{StartBracket}{StartString}{mainStr}{EndString}{EndBracket}";
        }

    }
}
