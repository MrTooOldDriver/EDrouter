using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Permissions;
using System.Diagnostics;
using System.Linq;
using System.Collections;

namespace EDrouter
{
    class Program
    {
        static void Main(string[] args)
        {
            EDrunning.EDfound();
            Console.ReadKey();
        }
    }
    // [PermissionSet(SecurityAction.Demand, Name = "FullTrust")] 系统权限相关
    class EDrunning  //检测游戏是否在运行
    {
        public static void EDfound()
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
                EDtxtReader.Reader();

            }
            else
            {
                Console.WriteLine("ED 64 is no running");
                EDtxtReader.Reader();
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
    class EDtxtReader
    {
        // [PermissionSet(SecurityAction.Demand, Name = "FullTrust")] //系统权限
        public static void Reader()
        {
            //string logpath = @"C:\Users\Hantao Zhong\Saved Games\Frontier Developments\Elite Dangerous"; 备用代码
            //string[] lol = Directory.GetFiles(@"C:\%USERPROFILE%\Saved Games\Frontier Developments\Elite Dangerous"); 
            var pathWithEnv = @"%USERPROFILE%\Saved Games\Frontier Developments\Elite Dangerous";
            var logPath = Environment.ExpandEnvironmentVariables(pathWithEnv);
            string[] lol = Directory.GetFiles(logPath);
            string loglocation = null;
            double count = 0;
            foreach (string filename in lol)
            {
                Console.WriteLine(filename);
                count++;
                Console.WriteLine(count);
                if (count == lol.Length)
                {
                    Console.WriteLine("发现最新ED的log" + filename); //27.03.2017 已经确认可以正常输出最后一个文件路径
                    loglocation = filename;

                }

            }
            Console.WriteLine(loglocation);
        }
    }
    class Readlog //读取ED的log并获取指定行来获取玩家位置
    {
        public static void Readding()
        {

        }
    }
}
