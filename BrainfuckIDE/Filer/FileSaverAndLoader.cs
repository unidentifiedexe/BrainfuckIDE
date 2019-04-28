#nullable enable

using BrainfuckIDE.Editor.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public bool IsSaved(Guid hash)
        {
            return _saveFileTextHash == hash;
        }

        public void Save(SourceText sourceText)
        {
            Save(_saveFilePath, sourceText.Text);
            _saveFileTextHash = sourceText.Guid;
            ForceDeleteTemporaryFile();
        }
        public void SaveToTemporary(SourceText sourceText)
        {
            if (sourceText.Guid == _temporaryFileTextHash) return;
            if (string.IsNullOrEmpty(_temporaryFilePath))
                _temporaryFilePath = CreateTemporaryFile(LocalEnvironmental.TemporaryDirectory, ".bf");

            Save(_temporaryFilePath, sourceText.Text);
            _temporaryFileTextHash = sourceText.Guid;
        }

        public void SaveAs(SourceText sourceText, string filePath)
        {
            Save(filePath, sourceText.Text);
            _saveFilePath = filePath;
            _saveFileTextHash = sourceText.Guid;
            ForceDeleteTemporaryFile();
        }

        public SourceText OpenFrom(string loadPath)
        {
            var text = File.ReadAllText(loadPath);
            _saveFilePath = loadPath;
            _saveFileTextHash = Guid.NewGuid();
            ForceDeleteTemporaryFile();
            return new SourceText(_saveFileTextHash, text);
        }



        /// <summary> 一時ファイルがセーブされているかどうかを確認せず一時ファイルを削除する </summary>
        public void ForceDeleteTemporaryFile()
        {
            File.Delete(_temporaryFilePath);
            _temporaryFilePath = string.Empty;
            _temporaryFileTextHash = Guid.Empty;
        }

        static private void Save(string path, string text)
        {
            File.WriteAllText(path, text);
        }
        static private string CreateTemporaryFile(string dirPath, string extension)
        {
            const int tryCount = 100;

            for (int i = 0; i < tryCount; i++)
            {
                var newFileName = Path.ChangeExtension(Path.GetRandomFileName(), extension);
                var newPath = Path.Combine(dirPath, newFileName);

                if (!File.Exists(newPath))
                {
                    File.Create(newPath);
                    return newPath;
                }
            }

            throw new Exception("一時ファイルの作成に失敗しました");

        }
    }
}
