    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Permissions;
using System.Diagnostics;
using System.Collections;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EDNavgation
{
    class Program
    {
        static void Main(string[] args)
        {
            EDrunning.EDChecker();
            //Navigation.SearchSystem(); //导航测试用
            Navigation.Calculation();
            Console.ReadKey();
        }
    }
    // [PermissionSet(SecurityAction.Demand, Name = "FullTrust")] 系统权限相关
    class EDrunning  //检测游戏是否在运行
    {
        public static void EDChecker()
        {
            string EDname = "EDLaunch";
            string EDname2 = "EliteDangerous64";
            if (Process.GetProcessesByName(EDname).Length >= 1) 
            {
                Console.WriteLine("ED Launch is running");
            }
            else
            {
                Console.WriteLine("ED Lanuch is not running");
                //return;
            }
            if (Process.GetProcessesByName(EDname2).Length >= 1)
            {
                Console.WriteLine("ED 64 is running");
                EDlogReader.Reader();
                EDReadlog.Reading();
            }
            else
            {
                Console.WriteLine("ED 64 is not running");
                EDlogReader.Reader();
                EDReadlog.Reading();
                //return;
            }

            //  此处由于开发需要把return注释掉了，由于log是需要在游戏启动之后才会生成，所以需要等待游戏启动
            //  当然这个步骤是在我的思路下写出来的，我还没去研究其他类似软件的源码

            //返回到console，等待调用方法 
            
            //if (Process.GetProcessesByName = true) ;
            //Process[] EDrunning = Process.GetProcessesByName(EDname);
            //double test = int.Parse(EDrunning);
            //Console.WriteLine(EDrunning);
            //double test = Process.GetProcessesByName(EDname);
            //string test = Process[]GetProcessesByName(EDname);
            //if (Process.GetProcessesByName(EDname) = 0)
            //{}
            //以上是已经弃用的代码
        }
    }
    class EDlogReader
    {
        // [PermissionSet(SecurityAction.Demand, Name = "FullTrust")] //系统权限
        public static string Reader()
        {
            //string logpath = @"C:\Users\Hantao Zhong\Saved Games\Frontier Developments\Elite Dangerous"; 备用代码
            //string[] lol = Directory.GetFiles(@"C:\%USERPROFILE%\Saved Games\Frontier Developments\Elite Dangerous"); 
            var pathWithEnv = @"%USERPROFILE%\Saved Games\Frontier Developments\Elite Dangerous";
            var logDir = Environment.ExpandEnvironmentVariables(pathWithEnv);
            string[] logPath = Directory.GetFiles(logDir);
            string loglocation = null;
            double count = 0;
            foreach (string filename in logPath)
            {
                //发行版本中请注释以下Console.WriteLine()
                //Console.WriteLine(filename);
                count++;
                //Console.WriteLine(count);
                if (logPath.Length == 0)
                {
                    Console.WriteLine("出现了致命错误：没有发现任何log文件。");
                    Console.WriteLine("解决办法：请尝试运行一次ED并且进入游戏！");
                }
                // 一个判定，如果该文件夹下找不到任何文件，提示玩家运行一次ED
                else
                {
                    if (count == logPath.Length)
                    {
                        Console.WriteLine("发现最新ED的log" + filename); //06.04.2017 已经确认可以正常输出最后一个文件路径
                        loglocation = filename;
                    }
                }

            }
            //Console.WriteLine(loglocation);
            return loglocation;
        }
    }
    class EDReadlog //读取ED的log并获取指定行来获取玩家位置
    {
        public static void Reading()
        {
            // log的文件结构采用json的模式
            // 先读取第一个头文件值：timestamp，寻找最后一次更新并且包含有这样一条
            // "event":"FSDJump", "StarSystem":"<星系名称>", "StarPos":[-42.438,-3.156,59.656],
            // 最后一项时间戳的最后一条有效包含FSDJump的列便会有StarSystem，但是这里还不够，EDSM
            // 并没有所有的星体数据，所以此时StarPos就变得尤为重要，核心算法需要调用这里的StarPos
            // 根据这里的StarPos（星系坐标）来进行计算并且找到最近的中子星或者wiki.ed-board.net
            // 标记的有趣的星体或者POI目的地。

            string logPath = EDlogReader.Reader();
            

            // Reading这里要返回三个数值，FSDJump with timestamp、StarSystem以及StarPos
            // 第一个返回的数值相当于时间戳，第二个返回值是字符串，第三个是一个数组
            Console.WriteLine("debug Reader()返回值: " + EDlogReader.Reader());
            return;
        }
    }
    class Navigation
    {
        
        public static string SearchSystem(string searchResult)
        {
            // 修改此处代码为获取玩家位置坐标 临时测试用代码 
            //string EDSMhttp = "https://www.edsm.net/api-v1/sphere-systems" ; From EDSM: If you need to do some testing on our API, please use the http://beta.edsm.net:8080/ endpoint.
            // string EDSMhttp_debug = "http://beta.edsm.net:8080/api-v1/sphere-systems";
            // string EDSMhttp_debug = "http://beta.edsm.net:8080/api-v1/cube-systems"; //Other method
            string EDSMhttp_debug = "http://beta.edsm.net:8080/api-v1/system"; // EDSM TEST API
            string userSystemName = "Quince"; //玩家所在位置
            //double radius = 23.33; //玩家船只最大跃迁距离
            //string searchResult = ""; //空的，留作输出
            string searchName = "systemName=" + userSystemName;
            string searchParameter = "showCoordinates=1";
            //string searchRadius = "radius=" + radius;
            // string searchRadius = "size=" + radius;//other method
            // Console.WriteLine("以"+searchName+"这个星系为中心半径"+radius+"LY搜索");
            Console.WriteLine("以" + searchName + "这个星系进行坐标确认");
            // string finalSearch = searchName + "&" + searchRadius;
            string finalSearch = searchName + "&" + searchParameter;
            Console.WriteLine("最终搜索参数"+finalSearch);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(EDSMhttp_debug);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] data = Encoding.UTF8.GetBytes(finalSearch);
            Console.WriteLine(data);
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                searchResult = reader.ReadToEnd();
                Console.WriteLine(searchResult);
            }
            Console.WriteLine("搜索结果" + searchResult);
            return searchResult;
            // 导航过程中用导航点通过EDSM数据库搜索 HTTP请求
            // 14.04.2017 5:05 UTC+8
            // 目前这个方法采用EDSM API Get systems in a sphere
            // 另外还有一个方法是 EDSM API Get systems in a cube 
            // 数据的准确性和这两个方法的区别还有待考证

            // 导航步骤 假设ANACONDA 50LY PRE JUMP FROM SOL TO SA A*
            // 玩家位置 从EDSM获取坐标（或者本地数据库） 通过坐标在本地数据库搜索500lY（50LY*10）内（或者EDSM在线搜索） 返回星系名字
            // 在本地中子星数据库搜索 返回星系名字+坐标 if no 扩大搜索范围到50LY*20 1000LY  还是没有 返回数据库数据不足         ***EDSM会不会被这个搞炸了？
            // 通过SOL和SA A*坐标计算出三位内直线方程式SR 计算辅助路径点（每隔1000LY所放置的一个虚拟坐标点，用于参照）
            // 计算搜索出来的中子星与SR线的距离 同时计算PLAYER到中子星的距离 择优选择 为第一个导航点
            // 继续计算 以第一个导航点 直线到SR的线 的点为路径提交EDSM搜索
            // repeat 直到在200LY（50LY*2）内发现目标星系
            // 可能还要额外加一个目的地确认搜索算法？

            // pre-alpha v1 算法 等做出算法之后需要通过ED内实际验证 通过EDD同步飞行数据进行测试 19.04.2017 03:28
            // 接下来写一个读取result json的 导出
            
            
        }
        public class Coordinate //json 初始化 
        {
            public string name { get; set; }
            public double x { get; set; }
            public double y { get; set; }
            public double z { get; set; }
            public object coords { get; set; }
        }
        public static void Calculation()
        {
            string returnResultFromSearch = Navigation.SearchSystem("");
            Console.WriteLine("返回值"+returnResultFromSearch);
            // WTFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
            // STM 到底怎么反实例
            // 完全看不懂那些百度出来的教程
            // 我心累dud
            // 20.04.2017 02:57
            Coordinate target = JsonConvert.DeserializeObject<Coordinate>(returnResultFromSearch);//一次解析 解析返回json
            //Console.WriteLine(""+target.x+target.y+target.z+target.name+target.coords);
            string jsonReturn = Convert.ToString(target.coords);
            Console.WriteLine(""+jsonReturn);
            Coordinate target_cood = JsonConvert.DeserializeObject<Coordinate>(jsonReturn);//二次解析 解析json内coodr
            Console.WriteLine("目标"+target.name+"X坐标为"+ target_cood.x+"Y坐标为" + target_cood.y+ "Z坐标为" + target_cood.z);
            // 反实例，写三维
            // 假设此处玩家在SOL 0 0 0， 等待neko的代码
            double anaconda = 50; //暂时假设这艘anaconda能跳50LY
            double playerX = 0;
            double playerY = 0;
            double playerZ = 0;
            double playerR = coordConvertToR(playerX, playerY, playerZ);
            double targetR = coordConvertToR(target_cood.x, target_cood.y, target_cood.z);
            //double distanceBetween = System.Math.Truncate(System.Math.Abs(playerR - targetR));
            double distanceBetween = System.Math.Abs(playerR - targetR);
            Console.WriteLine("两地直线距离" + distanceBetween + "LY");
            double choosePoint = System.Math.Truncate(System.Math.Abs(distanceBetween / (anaconda *4)));
            Console.WriteLine("高速路搜索点"+choosePoint+"个");
        }

        public static double coordConvertToR(double X, double Y, double Z)
        {
            double r = System.Math.Sqrt(X * X + Y * Y + Z * Z);
            Console.WriteLine(r);
            return r;
        }



    }
}
