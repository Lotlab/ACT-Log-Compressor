using Lotlab.PluginCommon;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace LogCompressor
{
    [Serializable]
    public class Config : PropertyNotifier
    {
        bool autoCompressByDate = false;
        /// <summary>
        /// 启用按日期自动压缩
        /// </summary>
        public bool AutoCompressByDate
        {
            get => autoCompressByDate;
            set
            {
                autoCompressByDate = value;
                OnPropertyChanged();
                autoSave();
            }
        }
        int autoCompressDate = 30;
        /// <summary>
        /// 自动压缩日期
        /// </summary>
        public int AutoCompressDate
        {
            get => autoCompressDate;
            set
            {
                autoCompressDate = value;
                OnPropertyChanged();
                autoSave();
            }
        }

        bool autoCompressBySize = false;
        /// <summary>
        /// 启用按大小自动压缩
        /// </summary>
        public bool AutoCompressBySize
        {
            get => autoCompressBySize;
            set
            {
                autoCompressBySize = value;
                OnPropertyChanged();
                autoSave();
            }
        }
        int autoCompressSize = 1024;
        /// <summary>
        /// 自动压缩大小
        /// </summary>
        public int AutoCompressSize
        {
            get => autoCompressSize;
            set
            {
                autoCompressSize = value;
                OnPropertyChanged();
                autoSave();
            }
        }

        bool autoDeleteByDate = false;
        /// <summary>
        /// 启用按日期自动压缩
        /// </summary>
        public bool AutoDeleteByDate
        {
            get => autoDeleteByDate;
            set
            {
                autoDeleteByDate = value;
                OnPropertyChanged();
                autoSave();
            }
        }
        int autoDeleteDate = 365;
        /// <summary>
        /// 自动压缩日期
        /// </summary>
        public int AutoDeleteDate
        {
            get => autoDeleteDate;
            set
            {
                autoDeleteDate = value;
                OnPropertyChanged();
                autoSave();
            }
        }

        bool autoDeleteBySize = false;
        /// <summary>
        /// 启用按大小自动压缩
        /// </summary>
        public bool AutoDeleteBySize
        {
            get => autoDeleteBySize;
            set
            {
                autoDeleteBySize = value;
                OnPropertyChanged();
                autoSave();
            }
        }
        int autoDeleteSize = 2048;
        /// <summary>
        /// 自动压缩大小
        /// </summary>
        public int AutoDeleteSize
        {
            get => autoDeleteSize;
            set
            {
                autoDeleteSize = value;
                OnPropertyChanged();
                autoSave();
            }
        }

        string fileName = "";

        public Config(string file)
        {
            fileName = file;
            autoSaveEnable = true;
        }
        /// <summary>
        /// 序列化构造器
        /// </summary>
        Config()
        {
            autoSaveEnable = false;
        }

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            using (var str = new XmlTextWriter(fileName, System.Text.Encoding.UTF8))
            {
                str.Formatting = Formatting.Indented;
                serializer.Serialize(str, this);
            }
        }

        bool autoSaveEnable = false;
        void autoSave()
        {
            if (autoSaveEnable)
                Save();
        }

        public static Config Load(string fileName)
        {
            if (!File.Exists(fileName))
                return new Config(fileName);

            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var cfg = serializer.Deserialize(fs) as Config;
                cfg.fileName = fileName;
                cfg.autoSaveEnable = true;
                return cfg;
            }
        }
    }
}
