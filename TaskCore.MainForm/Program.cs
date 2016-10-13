using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaskCore.Plugin;

namespace TaskCore.MainForm
{
    /// <summary>
    /// author 神牛步行3
    /// contact 841202396@qq.com
    /// des TaskCore.MainForm跨平台插件由神牛步行3提供
    /// </summary>
    public class Program
    {

        private static Dictionary<string, MoAssembly> dicTasks = new Dictionary<string, MoAssembly>();

        public static void Main(string[] args)
        {
            //注册编码，防止乱码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //初始化程序集文件
            _Init();

            //是否继续开启任务，默认没有待执行任务，不提示
            if (dicTasks.Count <= 0) { _LoopAlert("是否退出？(Y/N)"); return; }
            _LoopAlert("是否开始执行任务？(Y / N)");

            //执行任务
            foreach (var item in dicTasks.Values)
            {
                //使用Task防止异常后相互影响
                Task.Run(() =>
                {
                    try
                    {

                        //创建任务对象
                        var tp = item.Asm.CreateInstance(item.FullName) as TPlugin;
                        if (!string.IsNullOrEmpty(tp.XmlConfig.TpError)) { _Alert($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm")}：{tp.XmlConfig.Name} - 异常信息：{tp.XmlConfig.TpError}"); }
                        else
                        {

                            //timer定时器
                            var timer = new Timer((param) =>
                            {
                                var msg = $"{DateTime.Now.ToString("yyyy/MM/dd HH:mm")}：{tp.XmlConfig.Name}";
                                try
                                {
                                    var tpObj = param as TPlugin;
                                    //是否关闭暂停任务
                                    if (tpObj.XmlConfig.CloseTask) { return; }
                                    _Alert($"{msg} - 开始执行...{tp.XmlConfig.Timer}分钟一次");
                                    //任务入口
                                    tpObj.TPlugin_Load();
                                }
                                catch (Exception ex) { _Alert($"{msg} - 异常信息：{ex.Message}"); }
                            }, tp, 0, 1000 * 60 * tp.XmlConfig.Timer);
                        }

                    }
                    catch (Exception ex)
                    {
                        _Alert($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm")}：{item.Name} - 异常信息：{ex.Message}");
                    }
                });
            }
            _LoopAlert("正在监控执行的任务，是否退出？(Y / N)");
        }

        /// <summary>
        /// 初始化程序集文件
        /// </summary>
        private static void _Init()
        {
            try
            {

                _Alert("初始化任务中...");
                //获取文件
                var files = PublicClass._GetPluginFile("");
                if (files.Length <= 0) { _Alert("未能找到可用的程序集,请检查配置"); return; }

                //读取任务文件
                _Alert("读取CrossFiles.xml配置中...");
                var baseAddr = Path.Combine(Directory.GetCurrentDirectory(), "PluginXml", "CrossFiles.xml");
                var doc = XDocument.Load(baseAddr);
                var fileables = files.AsEnumerable();
                var taskFiles = new List<FileInfo>();
                foreach (var item in doc.Root.Nodes())
                {
                    var crossFile = item.ToString().ToUpper();
                    var choiceFiles = fileables.Where(b => crossFile.Contains(b.Name.ToUpper()));
                    if (!choiceFiles.Any()) { continue; }

                    taskFiles.AddRange(choiceFiles);
                }

                //展示文件信息
                _Alert($"待遍历{taskFiles.Count}个文件信息...");
                foreach (var item in taskFiles.OrderBy(b => b.CreationTime))
                {
                    var asmName = new AssemblyName($"{item.Name.Replace(".dll", "")}");
                    Assembly sm = Assembly.Load(asmName);
                    if (sm == null) { continue; }
                    var ts = sm.GetTypes();
                    //判断特定的任务类，加入任务dic
                    foreach (var t in ts.Where(b => b.Name != "TPlugin" && b.GetMethod("TPlugin_Load") != null))
                    {

                        dicTasks.Add(
                            t.FullName,
                            new MoAssembly
                            {
                                Asm = sm,
                                FullName = t.FullName,
                                Name = t.Name
                            });
                    }
                }
                _Alert($"获取待执行任务量：{dicTasks.Count}个");
            }
            catch (Exception ex)
            {
                _Alert($"异常信息：{ ex.Message}");
            }
        }

        /// <summary>
        /// 消息提醒
        /// </summary>
        /// <param name="msg">提示信息</param>
        /// <param name="isReadLine">是否需要用户输入指令</param>
        /// <returns>用户录入的指令</returns>
        private static string _Alert(string msg = "神牛步行3-消息提醒", bool isReadLine = false)
        {
            Console.WriteLine(msg);
            if (isReadLine) { return Console.ReadLine(); }
            return "";
        }

        private static void _LoopAlert(string msg = "是否开始执行任务？(Y/N)")
        {
            do
            {
                var readKey = _Alert(msg, true);
                if (readKey.ToUpper().Contains("Y")) { break; }
            } while (true);
        }
    }

    public class MoAssembly
    {
        public Assembly Asm { get; set; }
        public string FullName { get; set; }

        public string Name { get; set; }
    }
}
