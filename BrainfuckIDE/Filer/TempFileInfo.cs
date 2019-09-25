using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BrainfuckIDE.Filer
{
    /// <summary> 一時ファイルを保存/読み出しを行うためのクラスです </summary>
    [DataContract]
    public class TempFileInfo
    {
        [DataMember(Name = "original_file_path")]
        public string OriginalFilePath { get; set; }

        [DataMember(Name = "source_code")]
        public string SourceCode { get; set; }
    }

    
}
