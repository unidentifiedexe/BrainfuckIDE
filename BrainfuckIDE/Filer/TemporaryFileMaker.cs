#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsUtility.Path;
using System.Diagnostics;

namespace BrainfuckIDE.Filer
{
    static public class TemporaryFileMaker
    {
        const string _temporaryFileExtension = ".bftp";

        /// <summary> 一時ファイルの一覧を取得します。 </summary>
        public static IEnumerable<string> GetTemporaryFiles()
        {
            if (!Directory.Exists(LocalEnvironmental.TemporaryDirectory))
                return Enumerable.Empty<string>();
            else 
                return Directory.GetFiles(LocalEnvironmental.TemporaryDirectory, $"*{_temporaryFileExtension}").Where(IsCollectFormat);
        }
        public static IEnumerable<string> GetUnManagedTemporaryFiles()
        {
            var current = LocalEnvironmental.CurrentProcess;
            var ohterProcessHashes = Process.GetProcessesByName(current.ProcessName)
                .Where(p => p.Id != current.Id)
                .ToArray();

            return GetTemporaryFiles().Where(IsUnManged);


            bool IsUnManged(string path)
            {
                return ohterProcessHashes.All(p => !IsInProcess(path, p));
            }
            
        }

        public static string Create()
        {
            var dirPath = LocalEnvironmental.TemporaryDirectory;
            Directory.CreateDirectory(dirPath);
            const int tryCount = 100;

            for (int i = 0; i < tryCount; i++)
            {
                var newFileName = GetRandomFileName();
                var newPath = Path.Combine(dirPath, newFileName);

                if (!File.Exists(newPath))
                {
                    using var f = File.Create(newPath);
                    return newPath;
                }
            }

            throw new Exception("一時ファイルの作成に失敗しました");

        }

        public static bool IsCollectFormat(string filePath)
        {
            if (filePath is null) throw new ArgumentNullException(nameof(filePath));
            return TryGetHash(filePath, out _);
        }

        public static bool IsInProcess(string filePath, Process process)
        {
            if (filePath is null) throw new ArgumentNullException(nameof(filePath));
            if (process is null) throw new ArgumentNullException(nameof(process));

            if (!TryGetHash(filePath, out var hash))
                return false;

            return TemporaryFileHash.CreateFrom(process).Equals(hash);

        }

        private static bool TryGetHash(string filePath, out TemporaryFileHash val)
        {
            if (Path.GetExtension(filePath) != _temporaryFileExtension)
            {
                val = TemporaryFileHash.Empty;
                return false;
            }
            else return TemporaryFileHash.TryParseFromPath(filePath, out val);
        }


        private static string GetRandomFileName()
        {
            var randomFileName = Path.GetRandomFileName();
            randomFileName = Path.ChangeExtension(randomFileName, _temporaryFileExtension);
            randomFileName = AddTempFileHash(randomFileName);
            return randomFileName;
        }

        private static string AddTempFileHash(string originPath)
        {
            var process = LocalEnvironmental.CurrentProcess;
            var hash = $"_{TemporaryFileHash.CreateFrom(process).ToHashString()}";

            return PathChnager.AddFileName(originPath, hash);
        }


        private struct TemporaryFileHash
        {
            public static TemporaryFileHash CreateFrom(Process process)
            {
                if (process is null)
                    throw new ArgumentNullException(nameof(process));

                return new TemporaryFileHash(process.Id, process.StartTime.Ticks);
            }


            public static bool TryParseFromPath(string path, out TemporaryFileHash val)
            {
                val = Empty;
                var fileName = Path.GetFileNameWithoutExtension(path);

                var splitedStrings = fileName.Split('_');
                if (splitedStrings.Length < 3) return false;

                Array.Reverse(splitedStrings);

                if (!long.TryParse(splitedStrings[0], out var startTimeTick))
                    return false;
                if (!int.TryParse(splitedStrings[1], out var processId))
                    return false;

                val = new TemporaryFileHash(processId, startTimeTick);
                return true;

            }

            private TemporaryFileHash(int processId, long startTimeTick)
            {
                ProcessId = processId;
                StartTimeTick = startTimeTick;
            }

            public static TemporaryFileHash Empty { get; } = new TemporaryFileHash();


            public int ProcessId { get; }

            public long StartTimeTick { get; }

            public bool IsEmpty => StartTimeTick == 0;


            public string ToHashString()
            {
                return $"{ProcessId}_{StartTimeTick}";
            }

        }
    }

}
