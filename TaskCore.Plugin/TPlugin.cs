using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TaskCore.Plugin
{
    /// <summary>
    /// 插件基类
    /// </summary>
    public class TPlugin : IDisposable
    {

        public TPlugin()
        {

            XmlConfig = _InitConfig();
        }

        #region  初始化Xml配置文件 _InitConfig +XmlConfig

        /// <summary>
        /// xml配置信息
        /// </summary>
        public XmlConfig XmlConfig;


        /// <summary>
        /// 初始化配置信息
        /// </summary>
        /// <param name="configPath">配置文件对应路径</param>
        /// <returns></returns>
        public virtual XmlConfig _InitConfig(string configPath = "")
        {
            XmlConfig config = new XmlConfig();
            config.Timer = 1;
            config.Name = this.GetType().Name;
            try
            {

                if (string.IsNullOrEmpty(configPath))
                {

                    //默认各个dllXml配置
                    var defaultConfigFolder = "PluginXml";
                    var baseAddr = Directory.GetCurrentDirectory();
                    configPath = Path.Combine(baseAddr, defaultConfigFolder, config.Name + ".xml");
                }

                var doc = XDocument.Load(configPath);
                config.doc = doc;
                var taskMain = doc.Root;

                config.Timer = Convert.ToInt32(taskMain.Element(XName.Get("Timer", "")).Value);
                config.Name = taskMain.Element(XName.Get("Name", "")).Value;
                config.Des = taskMain.Element(XName.Get("Des", "")).Value;

                config.UserName = taskMain.Element(XName.Get("UserName", "")).Value;
                config.UserPwd = taskMain.Element(XName.Get("UserPwd", "")).Value;
                config.ApiKey = taskMain.Element(XName.Get("ApiKey", "")).Value;
                config.ApiUrl = taskMain.Element(XName.Get("ApiUrl", "")).Value;
                config.CloseTask = taskMain.Element(XName.Get("CloseTask", "")).Value == "1";

            }
            catch (Exception ex)
            {
                config.TpError = ex.Message;
                PublicClass._WriteLog($"{config.Name}初始化配置信息异常：{ex.Message}", "BaseLog");
                throw new Exception(ex.Message);
            }
            return config;
        }
        #endregion

        #region 初始化-开始加载  _Load

        /// <summary>
        /// 初始化-开始起
        /// </summary>
        public virtual void TPlugin_Load()
        {

            PublicClass._WriteLog("测试");
        }

        #endregion

        #region 释放资源

        public void Dispose()
        {

            GC.SuppressFinalize(this);//不需要再调用本对象的Finalize方法
        }

        public virtual void Dispose(Action action)
        {

            action();
        }

        #endregion
    }

    #region 配置文件 XmlConfig

    public class XmlConfig
    {
        public XmlConfig()
        {

        }

        /// <summary>
        /// 定制器时间（分钟）
        /// </summary>
        public int Timer { get; set; }

        /// <summary>
        /// 运行名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述(第一次获取dll描述，后面获取xml配置文件描述)
        /// </summary>
        public string Des { get; set; }

        /// <summary>
        /// 接口账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 接口密码
        /// </summary>
        public string UserPwd { get; set; }

        /// <summary>
        /// 接口秘钥
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// 接口地址
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// 是否关闭任务
        /// </summary>
        public bool CloseTask { get; set; }

        /// <summary>
        /// 插件中错误
        /// </summary>
        public string TpError { get; set; }


        /// <summary>
        /// xml信息
        /// </summary>
        public XDocument doc { get; set; }
    }

    #endregion
}
