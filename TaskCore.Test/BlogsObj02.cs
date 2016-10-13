using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskCore.Plugin;

namespace TaskCore.Test
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class BlogsObj02 : TPlugin
    {
        public BlogsObj02()
        {

        }

        public override void TPlugin_Load()
        {
            //Console.WriteLine($"这里是Blogs，获取配置文件：{this.XmlConfig.Name}");
            PublicClass._WriteLog($"这里是BlogsObj02，获取配置文件：{this.XmlConfig.Name}", this.XmlConfig.Name);
        }
    }
}
