using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextManager
{
    /// <summary>
    /// テキストの差を読むクラス
    /// </summary>
    static public class TextComparetor
    {

        static public List<TextComparateInfo> Comparetor(string prev, string now)
        {
            return ComparetorHelper(prev, now);
        }

        static private unsafe List<TextComparateInfo> ComparetorHelper(string text1,string text2)
        {
            if (text1 == null || text2 == null) throw new NullReferenceException();
            
            var ret = new List<TextComparateInfo>();

            if (text1.Length == 0 && text2.Length == 0) return ret;
            checked
            {
                fixed (char* str1 = text1)
                fixed (char* str2 = text2)
                {
                    char* char1Start = str1;
                    char* char2Start = str2;
                    char* char1End = &str1[text1.Length];
                    char* char2End = &str2[text2.Length];

                    while (*char1Start == *char2Start && *char1Start != 0)
                    {
                        char1Start++;
                        char2Start++;
                    }
                    
                    while (*char1End == *char2End && char1End != char1Start && char2End != char2Start)
                    {
                        char1End--;
                        char2End--;
                    }

                    if (*char2End == 0)
                    {
                        char1End--;
                        char2End--;
                    }

                    var startIndex = (int)(char1Start - str1);

                    var str1ChangedLength = (int)(char1End - char1Start + 1);
                    var str2ChangedLength = (int)(char2End - char2Start + 1);

                    if (str1ChangedLength > 0)
                    {
                        //Remove
                        var removeInfo = new TextComparateInfo()
                        {
                            Type = TextComparateInfo.ComparateType.Remove,
                            Index = startIndex,
                            Length = str1ChangedLength,
                            ChangeText = new string(char1Start, 0, str1ChangedLength)
                        };
                        ret.Add(removeInfo);
                    }
                    if (str2ChangedLength > 0)
                    {
                        //Add
                        var addInfo = new TextComparateInfo()
                        {
                            Type = TextComparateInfo.ComparateType.Add,
                            Index = startIndex,
                            Length = str2ChangedLength,
                            ChangeText = new string(char2Start, 0, str2ChangedLength)
                        };
                        ret.Add(addInfo);
                    }
                    return ret;
                }
                
            }
            

        }

    }
    public class TextComparateInfo
    {
        public enum ComparateType
        {
            Add,
            Remove
        }

        public string ChangeText { get; set; }
        public int Index { get; set; }
        public int Length { get; set; }
        public ComparateType Type { get; set; }

        public override string ToString()
        {
            return $"{Type} ,[ {Index} , {Length} ], \"{ChangeText}\" ";
        }

        /// <summary> テキストの変更を取り消す </summary>
        /// <param name="text"></param>
        public string RestorationPrevious(string text)
        {
            if (Type == ComparateType.Remove)
            {
                return text.Insert(Index, ChangeText);
            }
            else
            {
                return text.Remove(Index, Length);
            }
        }


        /// <summary> テキストの変更を適用する </summary>
        /// <param name="text"></param>
        public string RestorationNext(string text)
        {
            if (Type == ComparateType.Remove)
            {
                return text.Remove(Index, Length);
            }
            else
            {
                return text.Insert(Index, ChangeText);
            }
        }
    }

}
