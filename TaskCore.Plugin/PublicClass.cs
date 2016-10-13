using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace TaskCore.Plugin
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    /// <summary>
    /// 封装方法
    /// </summary>
    public class PublicClass
    {

        #region _ExcuteTask 批量任务执行器 +int

        /// <summary>
        /// 批次任务执行方法
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="func">func方法</param>
        /// <param name="list">待执行数据</param>
        /// <param name="taskLen">任务量</param>
        /// <param name="timeOut">任务超时时间 默认30s</param>
        /// <returns></returns>
        public static int _ExcuteTask<T>(Func<List<T>, int> func, List<T> list, int taskLen = 10, int timeOut = 30) where T : class
        {
            var result = 0;
            //任务量
            var nTotal = list.Count;
            taskLen = nTotal > taskLen ? taskLen : nTotal;
            var tasks = new Task<int>[taskLen];
            var page = nTotal / taskLen + (nTotal % taskLen > 0 ? 1 : 0);  //每个分得得需要执行的总条数 最有一个执行剩余所有
            for (var ji = 1; ji <= taskLen; ji++)
            {
                //使用分页方法获取待执行数据
                var list01 = list.Skip((ji - 1) * page).Take(page).ToList();
                if (list01.Count <= 0) { break; }
                var task = Task.Run(() =>
                {

                    return func(list01);
                });
                tasks[ji - 1] = task;
            }
            //等待执行
            Task.WaitAll(tasks, 1000 * 1 * timeOut);
            //获取执行成功条数
            result = tasks.Where(b => b.IsCompleted).Sum(b => b.Result);

            return result;
        }

        /// <summary>
        /// 批次任务执行方法
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="func">func方法</param>
        /// <param name="list">待执行数据</param>
        /// <param name="taskLen">任务量</param>
        /// <param name="timeOut">任务超时时间 默认30s</param>
        /// <returns></returns>
        public static List<TT> _ExcuteTask<T, TT>(Func<List<T>, List<TT>> func, List<T> list, int taskLen = 10, int timeOut = 30)
            where T : class
            where TT : class
        {
            var result = new List<TT>();
            //任务量
            var nTotal = list.Count;
            taskLen = nTotal > taskLen ? taskLen : nTotal;
            var tasks = new Task<List<TT>>[taskLen];
            var page = nTotal / taskLen + (nTotal % taskLen > 0 ? 1 : 0);  //每个分得得需要执行的总条数 最有一个执行剩余所有
            for (var ji = 1; ji <= taskLen; ji++)
            {
                //使用分页方法获取待执行数据
                var list01 = list.Skip((ji - 1) * page).Take(page).ToList();
                if (list01.Count <= 0) { break; }
                var task = Task.Run(() =>
                {

                    return func(list01);
                });
                tasks[ji - 1] = task;
            }
            //等待执行
            Task.WaitAll(tasks, 1000 * 1 * timeOut);
            //获取执行成功条数
            foreach (var item in tasks.Where(b => b.IsCompleted))
            {
                result.AddRange(item.Result);
            }
            return result;
        }

        #endregion

        #region _GetPluginFile  获取插件文件（即*.dll）   +FileInfo[]

        /// <summary>
        /// 获取插件文件（即*.dll）,默认程序根路径
        /// </summary>
        /// <param name="pluginFolderName">插件文件夹名称（默认：Plugin）</param>
        /// <returns>插件*.dll文件</returns>
        public static FileInfo[] _GetPluginFile(string pluginFolderName = "Plugin")
        {
            //组合插件文件夹路径
            string baseDirctory = Directory.GetCurrentDirectory();
            string pluginPath = Path.Combine(baseDirctory, pluginFolderName);

            DirectoryInfo di = new DirectoryInfo(pluginPath);
            if (!di.Exists)
            {
                di.Create();
            }

            //获取插件文件夹下面的所有dll文件
            return di.GetFiles("*.dll");
        }

        #endregion

        #region   _LoadPlugin<T>  加载dll对象  +T

        /// <summary>
        /// 加载插件对象
        /// </summary>
        /// <typeparam name="T">dll里面的对象父级</typeparam>
        /// <param name="pluginPath">dll路径</param>
        /// <param name="tName">dll对象名称</param>
        /// <returns></returns>
        public static T _LoadPlugin<T>(string pluginPath, string tName) where T : class, new()
        {

            if (string.IsNullOrEmpty(pluginPath) || string.IsNullOrEmpty(tName)) { return default(T); }


            try
            {
                //加载程序集
                Assembly sm = Assembly.Load(new AssemblyName(pluginPath));  //.LoadFile(pluginPath);
                //获取指定类型
                Type type = sm.GetType(tName);
                //实例化
                T t = Activator.CreateInstance(type) as T;
                if (t == null) { return default(T); }

                //初始化加载方法
                //t._Load();
                return t;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        #endregion


        #region  _WriteLog 文本日志 +void

        public static object objLog = new object();

        /// <summary>
        /// 文本日志
        /// </summary>
        /// <param name="logContent"></param>
        /// <param name="logFolderName"></param>
        /// <param name="postfix">后缀 默认.txt</param>
        /// <param name="isOp">是否需要分隔符</param>
        /// <param name="isAppend">是否追加信息 默认：true追加</param>
        public static void _WriteLog(string logContent, string logFolderName = "BaseLog", bool isOp = true, bool isAppend = true, string postfix = ".txt")
        {
            #region 文本日志

            var dateTime = DateTime.Now;

            var logFloder = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "Log",
                            logFolderName,
                            dateTime.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(logFloder))
            {
                Directory.CreateDirectory(logFloder);
            }
            var path = Path.Combine(
                        logFloder,
                        dateTime.ToString("HH") + "-" + (logFolderName.ToLower().IndexOf(".txt") > 0 ? logFolderName : logFolderName + postfix));
            lock (objLog)
            {
                File.AppendAllText(
                     path,
                     string.Format("时间：{0}\r\n{1}\r\n----------------------------------------------------------------\r\n", dateTime, logContent));
            }
            #endregion
        }
        #endregion

        #region  _HttpGet  Get请求   +Task<string>

        /// <summary>
        /// _HttpGet
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="nTimeOut">超时时间</param>
        /// <returns></returns>
        public static async Task<string> _HttpGet(string url, int nTimeOut = 30)
        {
            try
            {

                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 0, 0, 1 * nTimeOut);
                return await client.GetStringAsync(url);
            }
            catch (Exception ex)
            {

                return await Task.FromResult<string>(ex.Message);
            }
        }
        #endregion

        #region  _HttpPost  Post请求   +Task<string>

        /// <summary>
        /// _HttpPost
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="nTimeOut">超时时间</param>
        /// <returns></returns>
        public static async Task<string> _HttpPost(string url, string param, int nTimeOut = 30)
        {
            try
            {

                HttpClient client = new HttpClient();
                client.Timeout = new TimeSpan(0, 0, 0, 1 * nTimeOut);
                HttpContent content = new StringContent(param);
                return await client.PostAsync(url, content).Result.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {

                return await Task.FromResult<string>(ex.Message);
            }
        }
        #endregion

    }
}
