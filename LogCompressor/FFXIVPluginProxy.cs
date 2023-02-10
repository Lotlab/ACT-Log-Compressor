using Lotlab.PluginCommon;
using Lotlab.PluginCommon.FFXIV;
using System;
using System.Reflection;

namespace LogCompressor
{
    class FFXIVPluginProxy : ACTPluginProxy
    {
        public FFXIVPluginProxy(object instance): base(instance)
        {
            var _iocContainer = FieldGet("_iocContainer", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var ioc = new MinIOCProxy(_iocContainer);
            var type = GetTypeOfName("FFXIV_ACT_Plugin.Config.ISettingsMediator");
            var _setting = ioc.GetService(type);
            Settings = new SettingsMediatorProxy(_setting);
        }

        public SettingsMediatorProxy Settings { get; }
    }

    class MinIOCProxy : ClassProxy
    {
        public MinIOCProxy(object instance) : base(instance)
        {
        }

        public object GetService(Type type)
        {
            return CallMethod(type);
        }
    }

    class SettingsMediatorProxy : ClassProxy
    {
        public SettingsMediatorProxy(object instance) : base(instance)
        {
            ParseSettings = new ParseSettingsProxy(PropertyGet(nameof(ParseSettings)));
            DataCollectionSettings = new DataCollectionSettingsEventArgsProxy(PropertyGet(nameof(DataCollectionSettings)));
        }

        public ParseSettingsProxy ParseSettings { get; }

        public DataCollectionSettingsEventArgsProxy DataCollectionSettings { get; }
    }

    class ParseSettingsProxy : ClassProxy
    {
        public ParseSettingsProxy(object instance) : base(instance)
        {
        }

        public bool DisableDamageShield => (bool)FieldGet();

        public bool DisableCombinePets => (bool)FieldGet();

        public int LanguageID => (int)FieldGet();

        public int ParseFilter => (int)FieldGet();

        public bool SimulateIndividualDoTCrits => (bool)FieldGet();

        public bool ShowRealDoTTicks => (bool)FieldGet();

        public bool ShowDebug => (bool)FieldGet();

        public bool EnableBenchmarks => (bool)FieldGet();
    }

    class DataCollectionSettingsEventArgsProxy : ClassProxy
    {
        public DataCollectionSettingsEventArgsProxy(object instance) : base(instance)
        {
        }

        public int ProcessID => (int)FieldGet();

        public string LogFileFolder => (string)FieldGet();

        public bool LogAllNetworkData => (bool)FieldGet();

        public bool DisableCombatLog => (bool)FieldGet();

        public string NetworkIP => (string)FieldGet();

        public bool UseWinPCap => (bool)FieldGet();

        public bool UseSocketFilter => (bool)FieldGet();
    }
}
