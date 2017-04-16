using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Permissions;
using System.Diagnostics;
using System.Collections;

namespace EDrouter
{
    class Program
    {
        static void Main(string[] args)
        {
            EDrunning.EDChecker();
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
}
