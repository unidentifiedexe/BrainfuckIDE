#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace BrainfuckIDE.Editor.CodeAnalysis
{
    static class SpaceStringMap
    {
        private static readonly List<string> _nestStringList = new List<string>() { string.Empty };

        static public string GetNestString(int nest)
        {
            if (nest <= 0) return string.Empty;
            while (nest >= _nestStringList.Count)
                AddNextNestString();
            return _nestStringList[nest];
        }

        static private void AddNextNestString()
        {
            _nestStringList.Add(_nestStringList.Last() + " ");
        }

        static public string GetNestedString(string str, int nest)
        {
            return $"{GetNestString(nest)}{str}";
        }


        static public int CountFrontSpace(string text)
        {
            return text.TakeWhile(p => p == ' ').Count();
        }

    }

}
