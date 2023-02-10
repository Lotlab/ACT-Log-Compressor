using Lotlab.PluginCommon;
using System;
using System.Text.RegularExpressions;
using System.IO;

using ZstdSharp;

namespace LogCompressor
{
    class LogFileItem : PropertyNotifier
    {
        /// <summary>
        /// 压缩文件扩展名
        /// </summary>
        const string CompressFileExt = ".zst";
        /// <summary>
        /// 原始文件路径
        /// </summary>
        string originalPath { get; set; }
        /// <summary>
        /// 当前文件路径
        /// </summary>
        string currentPath { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; }

        bool compressed = false;
        /// <summary>
        /// 是否已压缩
        /// </summary>
        public bool Compressed { get => compressed; private set { compressed = value; OnPropertyChanged(); OnPropertyChanged(nameof(Status)); } }

        /// <summary>
        /// 状态文本
        /// </summary>
        public string Status => Compressed ? "已压缩" : "未压缩";

        /// <summary>
        /// 日志时间
        /// </summary>
        public DateTime Date { get; }
        /// <summary>
        /// 日志时间文本
        /// </summary>
        public string DateString => Date.ToShortDateString();

        long size = 0;
        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get => size; private set { size = value; OnPropertyChanged(); OnPropertyChanged(nameof(SizeString)); } }

        public string SizeString => string.Format("{0:F2} MB", (float)Size / 1024 / 1024);

        public LogFileItem(string fileName)
        {
            currentPath = fileName;
            if (fileName.EndsWith(CompressFileExt))
            {
                Compressed = true;
                originalPath = fileName.Substring(0, fileName.Length - CompressFileExt.Length);
            }
            else
            {
                Compressed = false;
                originalPath = fileName;
            }
            FileName = Path.GetFileName(originalPath);
            Size = new FileInfo(fileName).Length;

            Regex regex = new Regex("Network_[0-9]+_([0-9]+)");
            var match = regex.Match(originalPath);
            if (!match.Success) return;

            var dateStr = match.Groups[1].Value;
            Date = DateTime.ParseExact(dateStr, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 压缩当前文件
        /// </summary>
        public void Compress()
        {
            if (Compressed)
                return;

            var compressedPath = currentPath + CompressFileExt;
            using (var input = File.OpenRead(currentPath))
            {
                using (var output = File.OpenWrite(compressedPath))
                {
                    using (var compressionStream = new CompressionStream(output, 5))
                    {
                        input.CopyTo(compressionStream);
                    }
                    Size = output.Length;
                }
            }

            File.Delete(currentPath);
            currentPath = compressedPath;
            Compressed = true;
        }

        /// <summary>
        /// 解压当前文件
        /// </summary>
        public void Decompress()
        {
            if (!Compressed)
                return;

            using (var input = File.OpenRead(currentPath))
            {
                using (var output = File.OpenWrite(originalPath))
                {
                    using (var compressionStream = new DecompressionStream(input, 5))
                    {
                        compressionStream.CopyTo(output);
                    }
                    Size = output.Length;
                }
            }

            File.Delete(currentPath);
            currentPath = originalPath;
            Compressed = false;
        }

        /// <summary>
        /// 删除当前文件
        /// </summary>
        public void Delete()
        {
            File.Delete(currentPath);
        }
    }
}
