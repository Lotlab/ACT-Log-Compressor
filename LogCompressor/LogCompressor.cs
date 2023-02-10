using Lotlab.PluginCommon;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Collections.Generic;
using Advanced_Combat_Tracker;
using System.Threading.Tasks;
using System.Linq;

namespace LogCompressor
{
    class LogCompressor
    {
        public PluginControlViewModel vm { get; } = new PluginControlViewModel();

        string _logPath = "";

        public string logPath
        {
            get => _logPath; 
            set
            {
                _logPath = value;
                logger.LogInfo("日志文件夹:" + value);

                updateLogPath();
                autoHandleFiles();
            }
        }

        public SimpleLoggerSync logger { get; } = new SimpleLoggerSync("");

        public ObservableCollection<LogFileItem> filesList { get; } = new ObservableCollection<LogFileItem>();

        readonly object filesLock = new object();

        public Config config { get; }

        public LogCompressor()
        {
            vm.Logs = logger.ObserveLogs;
            BindingOperations.EnableCollectionSynchronization(filesList, filesLock);
            vm.Files = filesList;

            vm.CompressSelected.OnExecute += compressSelected;
            vm.DecompressSelected.OnExecute += decompressSelected;
            vm.ApplyConfig.OnExecute += ApplyConfig;

            // 载入配置
            var configPath = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "Config", "LogCompressor.config.xml");
            try
            {
                config = Config.Load(configPath);
            }
            catch (Exception e)
            {
                config = new Config(configPath);
                logger.LogError(e);
            }
            vm.Config = config;

            // 自动压缩
        }

        private void ApplyConfig(object obj)
        {
            try
            {
                autoHandleFiles();
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        void autoHandleFiles()
        {
            logger.LogInfo("准备自动执行");

            long sumSize = 0; // 总大小
            long uncompressedSize = 0; // 总未压缩大小
            List<LogFileItem> itemList = new List<LogFileItem>();
            lock (filesLock)
            {
                foreach (var item in filesList)
                {
                    itemList.Add(item);
                    sumSize += item.Size;
                    if (!item.Compressed)
                        uncompressedSize += item.Size;
                }
            }

            Task.Run(() => {
                var now = DateTime.Now;
                itemList.Sort((a, b) => a.Date.CompareTo(b.Date));
                // 自动压缩
                foreach (var item in filesList)
                {
                    var compress = false;
                    if (config.AutoCompressByDate && config.AutoCompressDate > 0)
                        compress |= (now - item.Date).Days > config.AutoCompressDate;
                    if (config.AutoCompressBySize && config.AutoCompressSize > 0)
                        compress |= uncompressedSize > config.AutoCompressSize * 1024 * 1024;
                    if (compress)
                    {
                        var origSize = item.Size;
                        compressItem(item);
                        sumSize -= (origSize - item.Size);
                        uncompressedSize -= origSize;
                    }
                    else
                    {
                        break;
                    }
                }
                // 自动删除
                foreach (var item in filesList)
                {
                    var delete = false;
                    if (config.AutoDeleteByDate && config.AutoDeleteDate > 0)
                        delete |= (now - item.Date).Days > config.AutoDeleteDate;
                    if (config.AutoDeleteBySize && config.AutoDeleteSize > 0)
                        delete |= sumSize > config.AutoDeleteSize * 1024 * 1024;
                    if (delete)
                    {
                        deleteItem(item);
                        sumSize -= item.Size;

                        lock (filesLock) {
                            filesList.Remove(item);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                logger.LogInfo("执行完毕");
            });
        }

        private void decompressSelected(object obj)
        {
            var collection = obj as IEnumerable<object>;
            Task.Run(() => {
                foreach (var item in collection)
                {
                    var file = item as LogFileItem;
                    decompressItem(file);
                }
            });
        }

        private void compressSelected(object obj)
        {
            var collection = obj as IEnumerable<object>;
            Task.Run(() => {
                foreach (var item in collection)
                {
                    var file = item as LogFileItem;
                    compressItem(file);
                }
            });
        }

        private void decompressItem(LogFileItem file)
        {
            try
            {
                file.Decompress();
                logger.LogInfo("解压 " + file.FileName);
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        private void compressItem(LogFileItem file)
        {
            try
            {
                file.Compress();
                logger.LogInfo("压缩 " + file.FileName);
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        private void deleteItem(LogFileItem file)
        {
            try
            {
                file.Delete();
                logger.LogInfo("删除 " + file.FileName);
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        void updateLogPath()
        {
            var files = Directory.EnumerateFiles(logPath, "Network_*.log*");

            lock (filesLock)
            {
                foreach (var item in files)
                {
                    try
                    {
                        filesList.Add(new LogFileItem(item));
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e);
                    }
                }
            }
        }
    }
}
