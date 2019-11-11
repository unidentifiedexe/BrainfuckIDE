#nullable enable
using CsUtility.Primitive;
using ICSharpCode.AvalonEdit.Snippets;
using JsonHelper.Attributes;
using Snippets;
using Snippets.ElementConverters;
using Snippets.RepleaceElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Snippets.SnippetSeeds
{
    [DataContract]
    class ConverterSnippetSeed : ISnippetSeed , IAddableSnippetSeed
    {
        #region フィールド

        private string? _name;
        private string? _title;
        private string? _shortcut;
        private string[]? _code;
        private IRepleaceElement[]? _elements;
        private IElementConverter[]? _converters;

        #endregion

        #region プロパティ


        [DataMember]
        public string Name
        {
            get => _name ?? string.Empty; 
            set => _name = value;
        }
        [DataMember]
        public string Shortcut
        {
            get => _shortcut ?? string.Empty;
            set => _shortcut = value;
        }

        [DataMember]
        public string Title
        {
            get => _title ?? string.Empty;
            set => _title = value;
        }
        [DataMember]
        [DefaultCollectionType(typeof(string[]))]
        public IEnumerable<string> Code
        {
            get => _code ?? Array.Empty<string>();
            set => _code = value.ToArray();
        }


        [DataMember]
        [DefaultCollectionType(typeof(IRepleaceElement[]))]
        public IEnumerable<IRepleaceElement> Elements
        {
            get => _elements ?? Array.Empty<IRepleaceElement>();
            set => _elements = value?.ToArray();
        }
        [DataMember]
        [DefaultCollectionType(typeof(IElementConverter[]))]
        public IEnumerable<IElementConverter> Converters
        {
            get => _converters ?? Array.Empty<IElementConverter>();
            set => _converters = value.ToArray();
        }


        #endregion


        public ISnippet CreateSnippet()
        {

            var snippets = GetSnippet(Code, Elements, Converters);
            return new CommonSnippet(Shortcut, Title, snippets);
        }

        public ISnippet CreateSnippet(
            IEnumerable<IRepleaceElement> additivElements,
            IEnumerable<IElementConverter> additivConverters)
        {
            var element = Elements;
            if (additivElements != null)
                element = element.Concat(additivElements);
            var converters = Converters;
            if (additivConverters != null)
                converters = converters.Concat(additivConverters);
            var snippets = GetSnippet(Code, element, converters);
            return new CommonSnippet(Shortcut, Title, snippets);
        }


        private Snippet GetSnippet(
            IEnumerable<string> code,
            IEnumerable<IRepleaceElement> elements, 
            IEnumerable<IElementConverter> converters)
        {
            var snippet = new Snippet();

            var strs = string.Join("\n", code);
            foreach (var item in ConvertToElement(strs, elements, converters))
            {
                snippet.Elements.Add(item);
            }
            return snippet;
        }

        private static IEnumerable<SnippetElement> ConvertToElement(string str, IEnumerable<IRepleaceElement> elements, IEnumerable<IElementConverter> converters)
        {
            Regex reg = new Regex("{.+?}");
            var matches = reg.Matches(str).OfType<Match>().OrderBy(p => p.Index);

            var getor = new BoundElementsGetor(elements, converters);

            var startPos = 0;

            foreach (var item in matches)
            {
                var info = new ElementInfo(item.Value);
                var element = getor.GetElement(info);

                if (element == null)
                    continue;

                if (item.Index != startPos)
                    yield return new SnippetTextElement() { Text = str.Substring(startPos, item.Index - startPos) };

                yield return element;

                startPos = item.Index + item.Length;
            }
            var left = str.Substring(startPos);

            if (left.Length > 0) yield return new SnippetTextElement() { Text = left };
        }
      
        private class CommonSnippet : SnippetBase
        {
            public override string Shortcut { get; protected set; }
            public override string Title { get; protected set; }

            public CommonSnippet(string shortcut, string title, Snippet snippet)
            {
                Shortcut = shortcut;
                Title = title;
                _snippet = snippet;
            }
        }


        private class BoundElementsGetor
        {
            private readonly Dictionary<string, SnippetReplaceableTextElement> _replaceElements;
            private readonly Dictionary<string, IElementConverter> _converters;
            private readonly Dictionary<SnippetReplaceableTextElement, bool> _isUsed;

            public BoundElementsGetor(IEnumerable<IRepleaceElement> elements, IEnumerable<IElementConverter> converters)
            {
                _replaceElements = elements.ToLookup(p => p.Name, p => new SnippetReplaceableTextElement { Text = p.DefaultText })
                    .ToDictionary(p => p.Key, p => p.First());
                _converters = converters.ToLookup(p => p.Name).ToDictionary(p => p.Key, p => p.First());
                _isUsed = _replaceElements.Values.ToDictionary(p => p, _ => false);
            }


            public SnippetElement? GetElement(ElementInfo info)
            {
                if (!_replaceElements.TryGetValue(info.Element, out var element))
                    return null;
                if (info.Converter?.Any(p => !_converters.ContainsKey(p)) ?? false)
                    return null;


                if (info.Converter == null)
                {
                    if (_isUsed[element])
                    {
                        return new SnippetBoundElement { TargetElement = element };
                    }
                    else
                    {
                        _isUsed[element] = true;
                        return element;
                    }
                }
                else if (!info.Converter.Any())
                {
                    return new SnippetBoundElement { TargetElement = element };
                }
                else
                {
                    var converters = info.Converter.Select(p => _converters[p]);
                    return new ConverterBoundElement(converters) { TargetElement = element };
                }
            }
        }

        private struct ElementInfo
        {
            public string Element { get; }
            public IEnumerable<string>? Converter { get; }

            public ElementInfo(string sector)
            {
                var trimed = sector.TrimStart('{').TrimEnd('}');

                var hoge = trimed.Split(':').Select(p => p.Trim()).ToArray();

                Element = hoge.First();
                Converter = hoge.Skip(1).FirstOrDefault()?.Split(',').Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p.Trim()).ToArray();
            }


        }
    }
}
