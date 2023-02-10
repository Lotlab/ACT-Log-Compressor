using Advanced_Combat_Tracker;
using Lotlab.PluginCommon.FFXIV;
using System;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.IO;

namespace LogCompressor
{
    public partial class ACTPlugin : IActPluginV1
    {
        /// <summary>
        /// 状态 Label
        /// </summary>
        Label statusLabel { get; set; } = null;

        LogCompressor compressor { get; } = new LogCompressor();

        /// <summary>
        /// ACT插件接口 - 初始化插件
        /// </summary>
        /// <remarks>
        /// 在这里初始化整个插件
        /// </remarks>
        /// <param name="pluginScreenSpace">插件所在的Tab页面</param>
        /// <param name="pluginStatusText">插件列表的状态标签</param>
        void IActPluginV1.InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            // 设置状态标签引用方便后续使用
            statusLabel = pluginStatusText;

            // 查找解析插件
            var plugins = ActGlobals.oFormActMain.ActPlugins;
            foreach (var item in plugins)
            {
                if (ACTPluginProxy.IsFFXIVPlugin(item.pluginObj))
                {
                    try
                    {
                        var ffxiv = new FFXIVPluginProxy(item.pluginObj);
                        compressor.logPath = ffxiv.Settings.DataCollectionSettings.LogFileFolder;
                    }
                    catch (Exception e)
                    {
                        compressor.logger.LogError(e);
                        compressor.logPath = Path.GetDirectoryName(ActGlobals.oFormActMain.LogFilePath);
                    }
                    break;
                }
            }

            // 初始化UI
            var control = new PluginControl();
            control.DataContext = compressor.vm;
            var host = new ElementHost()
            {
                Dock = DockStyle.Fill,
                Child = control
            };

            pluginScreenSpace.Text = "日志压缩";
            pluginScreenSpace.Controls.Add(host);

            // 更新状态标签的内容
            statusLabel.Text = "Plugin Inited.";
        }

        /// <summary>
        /// ACT插件接口 - 反初始化插件
        /// </summary>
        void IActPluginV1.DeInitPlugin()
        {
            // 更新状态
            if (statusLabel != null)
            {
                statusLabel.Text = "Plugin Exit.";
            }
            statusLabel = null;
        }
    }
}
