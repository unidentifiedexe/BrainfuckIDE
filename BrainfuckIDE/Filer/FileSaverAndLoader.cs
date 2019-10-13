#nullable enable

using BrainfuckIDE.Editor.Controls;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JsonHelper;
using System.Text.RegularExpressions;

namespace BrainfuckIDE.Filer
{
    class FileSaverAndLoader
    {
        private string _temporaryFilePath = string.Empty;

        private string _saveFilePath = string.Empty;

        private Guid _saveFileTextHash = Guid.Empty;
        private Guid _temporaryFileTextHash = Guid.Empty;

        /// <summary> 
        /// <para> 紐づけされているファイル名を取得します </para>
        /// <para> 設定されていない場合は "Untitled" 相当の文字列を返します </para>
        /// </summary>
        public string FilePath
        {
            get
            {
                if (HasSaveFilePath)
                    return _saveFilePath;
                else
                    return "Untitled";
            }
        }

        /// <summary> ファイル名が紐づけされているかどうかを取得します </summary>
        public bool HasSaveFilePath => !string.IsNullOrEmpty(_saveFilePath);



        /// <summary>
        /// 一時保存しているファイルが正規の保存ファイルと一致しているかどうかを取得します
        /// </summary>
        /// <returns></returns>
        public bool IsSaved() => IsSaved(_temporaryFileTextHash);
        /// <summary>
        /// 指定のテキストが保存済みと一致しているかどうかを取得します
        /// </summary>
        /// <param name="hash"> テキストのハッシュ値 </param>
        /// <returns></returns>
        public bool IsSaved(Guid hash)
        {
            //if (_saveFileTextHash == Guid.Empty) return false;
            return _saveFileTextHash == hash;
        }

        /// <summary> テキストデータを上書き保存します </summary>
        /// <param name="sourceText"> テキストデータ </param>
        /// <exception cref="Exception"> 保存先が未指定です </exception>
        /// <exception cref="Exception"> 保存に失敗した場合適当な例外が発生します </exception>
        public void Save(SourceText sourceText)
        {
            SaveAs(_saveFilePath, sourceText);
        }

        /// <summary> 一時ファイルにテキストデータを保存します </summary>
        /// <param name="sourceText"> テキストデータ </param>
        /// <exception cref="Exception"> 保存に失敗した場合適当な例外が発生します </exception>
        public void SaveToTemporary(SourceText sourceText)
        {
            if (sourceText.Guid == _temporaryFileTextHash) return;
            if (sourceText.Guid == _saveFileTextHash) return;
            if (string.IsNullOrEmpty(_temporaryFilePath))
                _temporaryFilePath = TemporaryFileMaker.Create();
            var tempFile = new TempFileInfo()
            {
                OriginalFilePath = _saveFilePath,
                SourceCode = sourceText.Text,
            };
            Json.Save(tempFile, _temporaryFilePath);
            _temporaryFileTextHash = sourceText.Guid;
        }


        /// <summary> 新たなファイル名を指定してテキストデータの保存を行います </summary>
        /// <param name="sourceText"></param>
        /// <param name="filePath"></param>
        /// <exception cref="Exception"> 保存に失敗した場合適当な例外が発生します </exception>
        public void SaveAs(string filePath, SourceText sourceText)
        {
            Save(filePath, sourceText.Text);
            _saveFilePath = filePath;
            _temporaryFileTextHash = _saveFileTextHash = sourceText.Guid;
            ForceDeleteTemporaryFile();
        }

        /// <summary> ファイルを開きテキストデータを取得します </summary>
        /// <param name="loadPath"> 読み取るファイルパス </param>
        /// <returns></returns>
        /// <exception cref="Exception"> 読み取りに失敗した場合適当な例外が発生します </exception>
        public SourceText OpenFrom(string loadPath)
        {
            if (TemporaryFileMaker.IsCollectFormat(loadPath))
                return LoadTemporry(loadPath);

            var text = File.ReadAllText(loadPath);
            _saveFilePath = loadPath;
            _temporaryFileTextHash = _saveFileTextHash = Guid.NewGuid();
            ForceDeleteTemporaryFile();
            return new SourceText(_saveFileTextHash, text);
        }
        private SourceText LoadTemporry(string filePath)
        {
            var tempFile = Json.Load<TempFileInfo>(filePath);
                ForceDeleteTemporaryFile();
                _temporaryFilePath = filePath;

            _saveFileTextHash = Guid.NewGuid();
            _saveFilePath = tempFile.OriginalFilePath;
            var newText = new SourceText(Guid.NewGuid(), tempFile.SourceCode);
            SaveToTemporary(newText);

            return newText;
        }


        /// <summary> 一時ファイルがセーブされているかどうかを確認せず一時ファイルを削除する </summary>
        public void ForceDeleteTemporaryFile()
        {
            if (!string.IsNullOrEmpty(_temporaryFilePath))
            {
                File.Delete(_temporaryFilePath);
                _temporaryFilePath = string.Empty;
            }
        }

        static private void Save(string path, string text)
        {
            File.WriteAllText(path, text);
        }
        

    }

}
