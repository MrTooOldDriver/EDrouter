using System;
using System.Collections;
using Newtonsoft.Json;

using EDRouter;
using System.IO;

namespace Reader
{
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

    class LogExtractor //读取ED的log并获取指定行来获取玩家位置
    {
        public static String Read()
        {
            // log的文件结构采用json的模式
            // 先读取第一个头文件值：timestamp，寻找最后一次更新并且包含有这样一条
            // "event":"FSDJump", "StarSystem":"<星系名称>", "StarPos":[-42.438,-3.156,59.656],
            // 最后一项时间戳的最后一条有效包含FSDJump的列便会有StarSystem，但是这里还不够，EDSM
            // 并没有所有的星体数据，所以此时StarPos就变得尤为重要，核心算法需要调用这里的StarPos
            // 根据这里的StarPos（星系坐标）来进行计算并且找到最近的中子星或者wiki.ed-board.net
            // 标记的有趣的星体或者POI目的地。

            string logPath = EDlogReader.Reader();

            // 创建一个新的动态数组
            ArrayList logArray = new ArrayList();

            // 循环遍历log文件并且存储到动态数组中
            int counterArrayController = 0;
            string line = string.Empty;

            System.IO.StreamReader file = new System.IO.StreamReader(logPath);
            while ((line = file.ReadLine()) != null)
            {
                logArray.Add(line);
                counterArrayController++;
            }

            // 读取最后一项相关条目并且返回相关数据
            //Console.WriteLine("Journal的长度是：" + logArray.Count);
            //Console.WriteLine("正在查询指挥官最后一处位置...");

            string Output = String.Empty;
            int counterArrayReader = logArray.Count - 1;
            for (int a = counterArrayReader; a > 0; a--)
            {
                string logLine = logArray[a].ToString();
                if (logLine.Contains("StarPos"))
                {
                    Output = logLine;
                    a = -1;
                }
            }

            // Reading这里要返回三个数值，FSDJump with timestamp、StarSystem以及StarPos
            // 第一个返回的数值相当于时间戳，第二个返回值是字符串，第三个是一个数组
            return Output;
        }
    }

    class Parser
    {
        public static Data JSONHandler()
        {
            string Line = LogExtractor.Read();
            //string List = string.Empty;
            //List<Data> outputList = JsonConvert.DeserializeObject<Data>(List);
            Data List = JsonConvert.DeserializeObject<Data>(Line);
            Console.WriteLine("指挥官当前位置是：" + List.StarSystem);

            return List;
        }

    }
}