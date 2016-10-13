using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskCore.Plugin;
using TaskCore.Test01;

namespace TaskCore.Test
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class BlogsObj : TPlugin
    {
        public BlogsObj()
        {

        }

        public override void TPlugin_Load()
        {
            var sbLog = new StringBuilder(string.Empty);
            try
            {
                sbLog.Append($"这里是BlogsObj，获取配置文件：{this.XmlConfig.Name}");

                //代码块
                //

               // new WriteLog()._WriteLog($"{DateTime.Now}测试引用nuget包");

            }
            catch (Exception ex)
            {
                sbLog.Append($"异常信息：{ex.Message}");
            }
            finally
            {

                PublicClass._WriteLog(sbLog.ToString(), this.XmlConfig.Name);
            }
        }
    }
}
