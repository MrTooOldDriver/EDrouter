using System;
using Reader;

namespace EDRouter
{
    class Program
    {
        static void Main(string[] args)
        {
            //EDrunning.EDChecker();
            //Navigation.SearchSystem(); //导航测试用
            //string firstResultTest = Navigation.FirstCalculation();
            //string firstResultTest ="";
            //Console.WriteLine("首次搜索输出" + firstResultTest);

            Console.WriteLine(Navigation.Navigation.FirstCalculation());

            //Console.WriteLine(Math.Atan(-1.7326) * (180/Math.PI));
            Console.ReadKey();

            //Navigation.SecondaryCalculation(firstResultTest); 第一套算法
            //Navigation.SecondaryCalculationType2(firstResultTest); //第二套算法

            //ArrayList Final = Navigation.SecondaryCalculationType3(firstResultTest);

            //for (int counter = 0; counter < Final.Count; counter++)
            //{
            //    Console.WriteLine("第" + counter + 1 + "个导航点是：" + Final[counter].ToString());
            //}

            Console.WriteLine("程序完成");
            Console.ReadKey();
            //De-Comment to Enable Debug Modules
            Debug.Module();
        }
    }
    // [PermissionSet(SecurityAction.Demand, Name = "FullTrust")] 系统权限相关

    //class EDRunning to EDRunning.cs using EDRunning namespace

    //class EDlogReader to Reader.cs using Reader namespace

    //class LogExtractor to Reader.cs using Reader namespace

    /************************************************************************************************************************
             *                                                        文档
             *  想获取相关数据的时候，请请求Parser下的类成员。
             *  包含了：
             *  Parser.JSONHandler().TimeStamp          | 返回时间戳                                             | [字符串型] 
             *  Parser.JSONHandler().Event              | 返回事件类型（目前只有Location或者FSDJump）            | [字符串型] 
             *  Parser.JSONHandler().StarSystem         | 返回当前星系名称                                       | [字符串型] 
             *  Parser.JSONHandler().StarPos            | 返回当前星系坐标                                       | [双精度浮点数组型] 
             *  Parser.JSONHandler().SystemAllegianc    | 返回当前星系效忠的势力                                 | [字符串型] 
             *  Parser.JSONHandler().SystemEconomy      | 返回当前星系经济类型                                   | [字符串型] 
             *  Parser.JSONHandler().SystemGovernment   | 返回当前星系政府类型                                   | [字符串型] 
             *  Parser.JSONHandler().SystemSecurity     | 返回当前星系安全等级                                   | [字符串型] 
             *  Parser.JSONHandler().Body               | 返回当前所在星体名字（仅在Location的Event下有效）      | [字符串型] 
             *  Parser.JSONHandler().BodyType           | 返回当前所在星体类型（仅在Location的Event下有效）      | [字符串型] 
             *  Parser.JSONHandler().JumpDist           | 返回此次跳跃的距离（仅在FSDJump的Event下有效）         | [双精度浮点型] 
             *  Parser.JSONHandler().FuelUsed           | 返回此次跳跃消耗的燃料（仅在FSDJump的Event下有效）     | [双精度浮点型] 
             *  Parser.JSONHandler().FuelLevel          | 返回当前的燃料状态（仅在FSDJump的Event下有效）         | [双精度浮点型] 
            ************************************************************************************************************************/

    //class Navigation to NavigationCore.cs using Navigation

    class Data
    {
        public string TimeStamp { get; set; }
        public string Event { get; set; }
        public string StarSystem { get; set; }
        public double[] StarPos { get; set; }
        public string SystemAllegiance { get; set; }
        public string SystemEconomy { get; set; }
        public string SystemGovernment { get; set; }
        public string SystemSecurity { get; set; }
        public string Body { get; set; }
        public string BodyType { get; set; }
        public double JumpDist { get; set; }
        public double FuelUsed { get; set; }
        public double FuelLevel { get; set; }
    }

    class Debug
    {
        public static void Module()
        {
            Parser.JSONHandler();
            Console.WriteLine(Parser.JSONHandler().StarSystem);
        }
    }
}
