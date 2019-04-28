#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.DialogUtils.Filer
{
    class FilterCollection<T> : IReadOnlyList<FileFilter<T>>
    {
        private List<FileFilter<T>> _innner = new List<FileFilter<T>>();

        public FileFilter<T> this[int index] => _innner[index];

        public bool IsLocked { get; set; }

        public int Count => _innner.Count;

        public void Add(string description, string type, T identifier)
        {
            Add(new FileFilter<T>(description, type, identifier));
        }

        public void Add(FileFilter baseFilter, T identifier)
        {
            Add(new FileFilter<T>(baseFilter, identifier));
        }

        public void Add(FileFilter<T> filter)
        {
            if (!IsLocked)
                _innner.Add(filter);
        }

        public IEnumerator<FileFilter<T>> GetEnumerator()
        {
            return _innner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _innner.GetEnumerator();
        }
    }

    readonly struct FileFilter<T>
    {
        private readonly FileFilter _innner;
        public FileFilter(string description, string type, T identifier)
            :this (new FileFilter(description, type), identifier)
        {
        }


        public FileFilter(FileFilter baseFilter, T identifier)
        {
            _innner = baseFilter;
            Identifier = identifier;
        }

        public string Description => _innner.Description;
        public string Type => _innner.Type;
        public T Identifier { get; }

        public string DescriptionText => _innner.DescriptionText;

    }

    readonly struct FileFilter
    {
        public string Description { get; }
        public string Type { get; }


        public string DescriptionText => $"{Description}({Type})";

       internal FileFilter(string description, string type)
        {
            Description = description;
            Type = type;
        }

    }

}
