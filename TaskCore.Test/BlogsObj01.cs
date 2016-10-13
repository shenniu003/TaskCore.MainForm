using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskCore.Plugin;

namespace TaskCore.Test
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class BlogsObj01 : TPlugin
    {
        public BlogsObj01()
        {

        }

        public override void TPlugin_Load()
        {
            var sbLog = new StringBuilder(string.Empty);
            try
            {
                sbLog.Append($"这里是BlogsObj01，获取配置文件：{this.XmlConfig.Name}");

                //代码块
                //
            }
            catch (Exception ex)
            {
                sbLog.Append($"异常信息：{ex.Message}");
            }
            finally
            {
                //Console.WriteLine($"这里是Blogs，获取配置文件：{this.XmlConfig.Name}");
                PublicClass._WriteLog(sbLog.ToString(), this.XmlConfig.Name);
            }
        }
    }
}
