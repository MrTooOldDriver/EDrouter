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
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data.MySqlClient;
using System.Windows.Forms; //非GUI一部分，开发所用
using System.Drawing; //开发用

namespace EDNavgation
{
    class Program
    {
        static void Main(string[] args)
        {
            EDrunning.EDChecker();
            //Navigation.SearchSystem(); //导航测试用
            string firstResultTest = Navigation.FirstCalculation();

            Console.WriteLine("首次搜索输出" + firstResultTest);
            
            //Navigation.SecondaryCalculation(firstResultTest); 第一套算法
            Navigation.SecondaryCalculationType2(firstResultTest); //第二套算法

            Console.WriteLine("程序完成");
            Console.ReadKey();
            //De-Comment to Enable Debug Modules
            Debug.Module();
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
                LogExtractor.Read();
                Parser.JSONHandler();
            }
            else
            {
                Console.WriteLine("ED 64 is not running");
                EDlogReader.Reader();
                LogExtractor.Read();
                Parser.JSONHandler();
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

    class Parser
    {
        public static Data JSONHandler()
        {
            string Line = LogExtractor.Read();
            //string List = string.Empty;
            //List<Data> outputList = JsonConvert.DeserializeObject<Data>(List);
            Data List = JsonConvert.DeserializeObject<Data>(Line);
            Console.WriteLine("指挥官当前位置是：" + List.StarSystem);

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

            return List;
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
            string userSystemName = "Pyramio GO-K c24-8"; //目标星系


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

        public static string FirstCalculation() //算法复杂，待功能完善之后进行优化
        {
            string returnResultFromSearch = Navigation.SearchSystem("");
            //Console.WriteLine("返回值"+returnResultFromSearch);
            Coordinate target = JsonConvert.DeserializeObject<Coordinate>(returnResultFromSearch);//一次解析 解析返回json

            //Console.WriteLine(""+target.x+target.y+target.z+target.name+target.coords);

            string jsonReturn = Convert.ToString(target.coords);
            //Console.WriteLine(""+jsonReturn);
            Coordinate target_cood = JsonConvert.DeserializeObject<Coordinate>(jsonReturn);//二次解析 解析json内coodr

            //Console.WriteLine("目标"+target.name+"X坐标为"+ target_cood.x+"Y坐标为" + target_cood.y+ "Z坐标为" + target_cood.z);

            // 反实例，写三维
            // 假设此处玩家在SOL 0 0 0， 等待neko的代码

            Console.WriteLine("**************"+Parser.JSONHandler().StarPos);

            double anaconda = 53.44; //暂时假设这艘anaconda能跳53.44LY
            double playerX = 46.375;
            double playerY = -448.6875;
            double playerZ = -127.125; //debug 暂时设定玩家位置
            double anaconda_boost = anaconda * 4; //上高速之后的搜索方法
            double searchVar = 0.5; //搜索范围变量 从1-0.2 

            target_cood.x = target_cood.x - playerX;
            target_cood.y = target_cood.y - playerY;
            target_cood.z = target_cood.z - playerZ; //更新目标绝对坐标系至相对坐标系 （相对出发点）

            // target_cood 现在是相对坐标系
            // player 现在是相对坐标系的原点 0 0 0
            //直角坐标系操作

            Console.WriteLine(target_cood.x + "," + target_cood.y + "," + target_cood.z + ","); //debug
            //double playerR = coordConvertToR(playerX, playerY, playerZ); //潜在多余或错误算法，暂时删除
            double targetR = coordConvertToR(target_cood.x, target_cood.y, target_cood.z);
            //double distanceBetween = System.Math.Truncate(System.Math.Abs(playerR - targetR));//错误
            //double distanceBetween = System.Math.Abs(playerR - targetR); 错误算法
            //Console.WriteLine("两地直线距离" + targetR + "LY");
            //double choosePoint = System.Math.Truncate(System.Math.Abs(targetR / (anaconda *4)));
            double choosePoint = System.Math.Truncate(System.Math.Abs(targetR / (anaconda *6))); //跳跃次数处理
            //Console.WriteLine("高速路预搜索点"+choosePoint+"个");

            //直角坐标系至球坐标系 player为原点 0 0 0
            //Note radial distance=r ; polar angle θ (theta)=t ; azimuthal angle φ (phi)=p;
            double target_Scood_R = coordConvertToR(target_cood.x,target_cood.y,target_cood.z);
            double target_Scood_T = coordConvertToT(target_cood.x, target_cood.y, target_cood.z);
            double target_Scood_P = coordConvertToP(target_cood.x, target_cood.y, target_cood.z);
            Console.WriteLine("S_cood, R=" + target_Scood_R + "; T=" + target_Scood_T + "; P=" + target_Scood_P);
            //target_Scood 现在是球坐标系状态 相对坐标

            //潜在BUG，目前都在用double，可以获取高精度但是一旦奇怪的数字进这套算法系统可能崩溃
            //对策 改float类

            //搜索校验值 最大（var=1）为左右上下180度大扇面 最小var=0.2 左右上下36度小扇面
            double Verify_T_Upper = target_Scood_T + (180 / searchVar);
            double Verify_T_Lower = target_Scood_T - (180 / searchVar);
            double Verify_P_Upper = target_Scood_P + (180 / searchVar);
            double Verify_P_Lower = target_Scood_P - (180 / searchVar);
            //这段代码好像没用上??? ←我SB了，其实用上了，在下面做中子星多选项和单选项筛选的时候

            //Debug用的textbox
            TextBox textbox1 = new TextBox();

            ArrayList FinalList = new ArrayList();

            //debug. using loacl database.
            string sql;
            string ConnectionString = "server=127.0.0.1;Database=neutrondb;uid=user;pwd=123456789";
            MySqlConnection conn = new MySqlConnection(ConnectionString);
            string ResultFromDatabase;
            
            //debug

            Point3D Navpoint = new Point3D(0,0,0);
            ArrayList FirstList = new ArrayList();
            ArrayList SecondList = new ArrayList();

            // ArrayList choosePointToE = new ArrayList();
            for (double counter = 0; counter <= 1000; counter++)
            {
                if (counter > choosePoint)
                    break;
                double arrPoint = 0;
                arrPoint = arrPoint + (counter * anaconda * 6);

                Navpoint.X = Convert.ToSingle((ScoordConvertToX(arrPoint, target_Scood_T, target_Scood_P)) + playerX);
                Navpoint.Y = Convert.ToSingle((ScoordConvertToY(arrPoint, target_Scood_T, target_Scood_P)) + playerY);
                Navpoint.Z = Convert.ToSingle((ScoordConvertToZ(arrPoint, target_Scood_T, target_Scood_P)) + playerZ);
                //Navpoint 绝对坐标系

                double VarTestInt = 6;

                //double searchBetween_X_Upper = Navpoint.X + (anaconda * 6); //*6是变量
                double searchBetween_X_Upper = Navpoint.X + (anaconda * VarTestInt);
                double searchBetween_X_Lower = Navpoint.X - (anaconda * VarTestInt);
                double searchBetween_Y_Upper = Navpoint.Y + (anaconda * VarTestInt); 
                double searchBetween_Y_Lower = Navpoint.Y - (anaconda * VarTestInt);
                double searchBetween_Z_Upper = Navpoint.Z + (anaconda * VarTestInt); 
                double searchBetween_Z_Lower = Navpoint.Z - (anaconda * VarTestInt);

                conn.Open();

                sql = "SELECT * FROM db WHERE X BETWEEN " + searchBetween_X_Lower + " AND " + searchBetween_X_Upper + " AND Y BETWEEN " + searchBetween_Y_Lower + " AND " + searchBetween_Y_Upper + " AND Z BETWEEN " +searchBetween_Z_Lower + " AND "+ searchBetween_Z_Upper; 
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd = new MySqlCommand(sql, conn);
                Console.WriteLine("打开连接,开始第"+counter+"搜索");

                bool cancel = false;

               MySqlDataReader Reader = cmd.ExecuteReader();
                while (Reader.Read())
                {
                    if (Reader.HasRows)
                    {
                        Console.WriteLine("已经发现");
                        double tryFirst = coordConvertToR(
                            target_cood.x - Convert.ToDouble(Reader.GetString("X")),
                            target_cood.y - Convert.ToDouble(Reader.GetString("Y")),
                            target_cood.z - Convert.ToDouble(Reader.GetString("Z"))
                                                          );
                        if (tryFirst < targetR)
                        {
                            cancel = true;
                            Console.WriteLine("****核实确认****"+Reader.GetString("Name"));
                            FirstList.Add(Reader.GetString("Name"));
                        }
                        else
                        {
                            Console.WriteLine("核实不通过");
                        }
                    }
                    //string result = Reader.GetString("Name");
                    //Console.WriteLine(result+ "在第" +counter);
                }
                if (cancel == true)
                    conn.Close();
                if (cancel == true)
                    break;
                conn.Close();

                //上高速前若多个选项需要细致校对
                //优选条件 离玩家近 或离判定点近
                //第一下neutron 然后接下来要怎么搜索 
                //搜索一下，反馈 要是有，一个一个校验距离搜索点的距离和离目标距离，择优选择
                //要是第一下没有，就扩大双倍距离（这里取决于效率值了）然后有了
                //还没有就直接计算下一个点搜索



            }
           // Console.WriteLine("发现了" + FirstList.Count + "个选项");
            //string FirstResult="";
            string FirstResult = "";

            if (FirstList.Count == 1) //临时
            {
                Console.WriteLine("本次只找到了一个选项，正在进行最终可靠性测试");
                Console.WriteLine("导航参考点:" + Navpoint.X + " AND " + Navpoint.Y + " AND " + Navpoint.Z);

                conn.Open();
                string sqlSearch;
                double X;
                double Y;
                double Z;
                double NeutronP;
                double NeutronT;
                sqlSearch = "SELECT X FROM db WHERE Name Like '%" + FirstList[0].ToString() + "%'";
                Console.WriteLine(FirstList[0].ToString());
                MySqlCommand cmd = new MySqlCommand(sqlSearch, conn);
                X = (double)cmd.ExecuteScalar();
                sqlSearch = "SELECT Y FROM db WHERE Name Like '%" + FirstList[0].ToString() + "%'";
                cmd = new MySqlCommand(sqlSearch, conn);
                Y = (double)cmd.ExecuteScalar();
                sqlSearch = "SELECT Z FROM db WHERE Name Like '%" + FirstList[0].ToString() + "%'";
                cmd = new MySqlCommand(sqlSearch, conn);
                Z = (double)cmd.ExecuteScalar();
                conn.Close();
                NeutronP = coordConvertToP(X - playerX, Y - playerY, Z - playerZ);
                NeutronT = coordConvertToT(X - playerX, Y - playerY, Z - playerZ);
                if (Verify_P_Lower <= NeutronP && NeutronP <= Verify_P_Upper && Verify_T_Lower <= NeutronT && NeutronT <= Verify_T_Upper)
                {
                    Console.WriteLine("校验通过");
                    return(FirstList[0].ToString());
                }
                else
                {
                    Console.WriteLine("校验不通过，放弃这个点搜索");
                }
            }
            else //多结果筛选，离中心线（球坐标系T P 表达）越近，越容易被筛选 13.05.2017
            {
                string FromList;
                string FromSecondList;
                for (int counter = 0; counter  < FirstList.Count; counter++)
                {
                    FromList = FirstList[counter].ToString();
                    Console.WriteLine("正在检测" + FromList + "是否最佳");
                    string sqlSearch;
                    double X;
                    double Y;
                    double Z;
                    double NeutronP;
                    double NeutronT;
                    conn.Open();
                    sqlSearch = "SELECT X FROM db WHERE Name Like '%" + FromList + "%'";
                    Console.WriteLine(FromList);
                    MySqlCommand cmd = new MySqlCommand(sqlSearch, conn);
                    X = (double)cmd.ExecuteScalar();
                    sqlSearch = "SELECT Y FROM db WHERE Name Like '%" + FromList + "%'";
                    cmd = new MySqlCommand(sqlSearch, conn);
                    Y = (double)cmd.ExecuteScalar();
                    sqlSearch = "SELECT Z FROM db WHERE Name Like '%" + FromList + "%'";
                    cmd = new MySqlCommand(sqlSearch, conn);
                    Z = (double)cmd.ExecuteScalar();
                    conn.Close();
                    NeutronP = coordConvertToP(X - playerX, Y - playerY, Z - playerZ);
                    NeutronT = coordConvertToT(X - playerX, Y - playerY, Z - playerZ);
                    if (Verify_P_Lower <= NeutronP && NeutronP <= Verify_P_Upper && Verify_T_Lower <= NeutronT && NeutronT <= Verify_T_Upper)
                    {
                        SecondList.Add(FromList);
                        Console.WriteLine("***第一校验通过***");
                    }
                    else
                    {
                        Console.WriteLine("校验不通过，放弃对" +FromList+ "的选择");
                    }
                }

                double final_pDiff=0;
                double final_tDiff=0;
                string final_Name="";
                

                for (int counter=0; counter < SecondList.Count; counter++)
                {
                    FromSecondList = FirstList[counter].ToString();
                    string sqlSearch;
                    double X;
                    double Y;
                    double Z;
                    double pDiff;
                    double tDiff;
                    double NeutronP;
                    double NeutronT;
                    conn.Open();
                    sqlSearch = "SELECT X FROM db WHERE Name Like '%" + FromSecondList + "%'";
                    Console.WriteLine(FromSecondList);
                    MySqlCommand cmd = new MySqlCommand(sqlSearch, conn);
                    X = (double)cmd.ExecuteScalar();
                    sqlSearch = "SELECT Y FROM db WHERE Name Like '%" + FromSecondList + "%'";
                    cmd = new MySqlCommand(sqlSearch, conn);
                    Y = (double)cmd.ExecuteScalar();
                    sqlSearch = "SELECT Z FROM db WHERE Name Like '%" + FromSecondList + "%'";
                    cmd = new MySqlCommand(sqlSearch, conn);
                    Z = (double)cmd.ExecuteScalar();
                    conn.Close();
                    NeutronP = coordConvertToP(X - playerX, Y - playerY, Z - playerZ);
                    NeutronT = coordConvertToT(X - playerX, Y - playerY, Z - playerZ);
                    pDiff = Math.Abs(Math.Abs(NeutronP) - Math.Abs(target_Scood_P));
                    tDiff = Math.Abs(Math.Abs(NeutronT) - Math.Abs(target_Scood_T));
                    if (counter == 0)
                    {
                        final_pDiff = pDiff;
                        final_tDiff = tDiff;
                        final_Name = FromSecondList;
                        Console.WriteLine("第一次写入，不对比");
                    }
                    else
                    {
                        if (pDiff < final_pDiff && tDiff < final_tDiff)
                        {
                            final_tDiff = pDiff;
                            final_tDiff = tDiff;
                            final_Name = FromSecondList;
                        }
                        else
                        {
                            Console.WriteLine(FromSecondList + "不是最优选项");
                        }
                    }
                    Console.WriteLine("TEST" + final_Name);
                    FirstResult = final_Name;
                }
                Console.WriteLine("TEST" + final_Name);
                FirstResult = final_Name;
            }
            return FirstResult;
        }

        //public ArrayList YEEE = new ArrayList();

        //ArrayList SecondaryCalculation = new ArrayList(); 做好了再说
        public static void SecondaryCalculation(string FirstResult)
        {
            Console.WriteLine("输入:" + FirstResult);
            ArrayList FinalOutput = new ArrayList();
            FinalOutput.Add(FirstResult);
            string sql;
            string ConnectionString = "server=127.0.0.1;Database=neutrondb;uid=user;pwd=123456789";
            MySqlConnection conn = new MySqlConnection(ConnectionString);
            double X;
            double Y;
            double Z;
            conn.Open();
            sql = "SELECT X FROM db WHERE Name Like '%" + FirstResult + "%'";
            Console.WriteLine(FirstResult);
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            X = (double)cmd.ExecuteScalar();
            sql = "SELECT Y FROM db WHERE Name Like '%" + FirstResult + "%'";
            cmd = new MySqlCommand(sql, conn);
            Y = (double)cmd.ExecuteScalar();
            sql = "SELECT Z FROM db WHERE Name Like '%" + FirstResult + "%'";
            cmd = new MySqlCommand(sql, conn);
            Z = (double)cmd.ExecuteScalar();
            conn.Close();

            //******************************************************************************************************************** 

            string returnResultFromSearch = Navigation.SearchSystem("");

            //******************************************************************************************************************** 
            //Console.WriteLine("返回值" + returnResultFromSearch);
            Coordinate target = JsonConvert.DeserializeObject<Coordinate>(returnResultFromSearch);//一次解析 解析返回json

            //Console.WriteLine(""+target.x+target.y+target.z+target.name+target.coords);

            string jsonReturn = Convert.ToString(target.coords);
            //Console.WriteLine("" + jsonReturn);
            Coordinate target_cood = JsonConvert.DeserializeObject<Coordinate>(jsonReturn);//二次解析 解析json内coodr

            Console.WriteLine("目标" + target.name + "X坐标为" + target_cood.x + "Y坐标为" + target_cood.y + "Z坐标为" + target_cood.z);

            double anaconda = 53.44; //暂时假设这艘anaconda能跳53.44LY
            double playerX = 46.375;
            double playerY = -448.6875;
            double playerZ = -127.125; //debug 暂时设定玩家位置
            double anaconda_boost = anaconda * 4; //上高速之后的搜索方法
            double searchVar = 0.5; //搜索范围变量 从1-0.2 

            target_cood.x = target_cood.x - playerX;
            target_cood.y = target_cood.y - playerY;
            target_cood.z = target_cood.z - playerZ; //更新目标绝对坐标系至相对坐标系 （相对出发点）
            //这里还用playerR的原因很简单，就是基于线条搜索而不是中子星点搜索

            double target_Scood_R = coordConvertToR(target_cood.x, target_cood.y, target_cood.z);
            double target_Scood_T = coordConvertToT(target_cood.x, target_cood.y, target_cood.z);
            double target_Scood_P = coordConvertToP(target_cood.x, target_cood.y, target_cood.z);

            //*******************************************************************************************************************

            double searchPoint_Scood_R = coordConvertToR(X, Y, Z);

            Start:

            //target_Scood_R = coordConvertToR(X, Y, Z);
            double targetR = target_Scood_R;
            Console.WriteLine("搜索距离上一个中子星"+searchPoint_Scood_R+"LY");

            //更新搜索点

            double searchPoint_X = ScoordConvertToX(searchPoint_Scood_R, target_Scood_T, target_Scood_P) + playerX;
            double searchPoint_Y = ScoordConvertToY(searchPoint_Scood_R, target_Scood_T, target_Scood_P) + playerY;
            double searchPoint_Z = ScoordConvertToZ(searchPoint_Scood_R, target_Scood_T, target_Scood_P) + playerZ;

            //更新完毕 绝对坐标系

            double VarTestInt = 6;

            //double searchBetween_X_Upper = Navpoint.X + (anaconda * 6); //*6是变量
            double searchBetween_X_Upper = searchPoint_X + (anaconda * VarTestInt );
            double searchBetween_X_Lower = searchPoint_X - (anaconda * VarTestInt );
            double searchBetween_Y_Upper = searchPoint_Y + (anaconda * VarTestInt );
            double searchBetween_Y_Lower = searchPoint_Y - (anaconda * VarTestInt );
            double searchBetween_Z_Upper = searchPoint_Z + (anaconda * VarTestInt );
            double searchBetween_Z_Lower = searchPoint_Z - (anaconda * VarTestInt );
            //中子星加速

            ArrayList FirstList = new ArrayList();
            ArrayList SecondList = new ArrayList();

            //*******************************************************************************************************************
            //参数准备区↑

            

            conn.Open();

            sql = "SELECT * FROM db WHERE X BETWEEN " + searchBetween_X_Lower + " AND " + searchBetween_X_Upper + " AND Y BETWEEN " + searchBetween_Y_Lower + " AND " + searchBetween_Y_Upper + " AND Z BETWEEN " + searchBetween_Z_Lower + " AND " + searchBetween_Z_Upper;
            cmd = new MySqlCommand(sql, conn);
            //Console.WriteLine("打开连接,开始第" + counter + "搜索");

           // bool cancel = false;

            MySqlDataReader Reader = cmd.ExecuteReader();
            while (Reader.Read())
            {
                if (Reader.HasRows)
                {
                    //Console.WriteLine("已经发现Secondary");
                    double tryFirst = coordConvertToR(
                        target_cood.x - Convert.ToDouble(Reader.GetString("X")),
                        target_cood.y - Convert.ToDouble(Reader.GetString("Y")),
                        target_cood.z - Convert.ToDouble(Reader.GetString("Z"))
                                                      );
                    if (tryFirst < targetR)
                    {
                        //cancel = true;
                        Console.WriteLine("****核实确认Secondary****" + Reader.GetString("Name"));
                        FirstList.Add(Reader.GetString("Name"));
                    }
                    else
                    {
                        Console.WriteLine("Secondary核实不通过");
                    }
                }
                //string result = Reader.GetString("Name");
                //Console.WriteLine(result+ "在第" +counter);
            }
            conn.Close();

            //*************************第一次筛选，范围选择*************************

            double Verify_T_Upper = target_Scood_T + (180 / searchVar);
            double Verify_T_Lower = target_Scood_T - (180 / searchVar);
            double Verify_P_Upper = target_Scood_P + (180 / searchVar);
            double Verify_P_Lower = target_Scood_P - (180 / searchVar);
            FirstList.Remove(FirstResult);

            Console.WriteLine("SecondaryCal总共找到了" + FirstList.Count);
            if (FirstList.Count == 0)
            {
                searchPoint_Scood_R=searchPoint_Scood_R + anaconda;
                goto Start;
            }
            else
            {
                Console.WriteLine("确认发现Secondary");
                if (FirstList.Count == 1) //当只有一个星系的时候，偏向于继续使用加速
                {
                    Console.WriteLine("Secondary本次只找到了一个选项，正在进行最终可靠性测试");
                    Console.WriteLine("Secondary导航参考点:" + searchPoint_X + " AND " + searchPoint_Y + " AND " + searchPoint_Z);

                    conn.Open();
                    string sqlSearch;
                    double Neutron_X;
                    double Neutron_Y;
                    double Neutron_Z;
                    double NeutronP;
                    double NeutronT;
                    sqlSearch = "SELECT X FROM db WHERE Name Like '%" + FirstList[0].ToString() + "%'";
                    Console.WriteLine(FirstList[0].ToString());
                    cmd = new MySqlCommand(sqlSearch, conn);
                    Neutron_X = (double)cmd.ExecuteScalar();
                    sqlSearch = "SELECT Y FROM db WHERE Name Like '%" + FirstList[0].ToString() + "%'";
                    cmd = new MySqlCommand(sqlSearch, conn);
                    Neutron_Y = (double)cmd.ExecuteScalar();
                    sqlSearch = "SELECT Z FROM db WHERE Name Like '%" + FirstList[0].ToString() + "%'";
                    cmd = new MySqlCommand(sqlSearch, conn);
                    Neutron_Z = (double)cmd.ExecuteScalar();
                    conn.Close();
                    NeutronP = coordConvertToP(Neutron_X - playerX, Neutron_Y - playerY, Neutron_Z - playerZ);
                    NeutronT = coordConvertToT(Neutron_X - playerX, Neutron_Y - playerY, Neutron_Z - playerZ);
                    if (Verify_P_Lower <= NeutronP && NeutronP <= Verify_P_Upper && Verify_T_Lower <= NeutronT && NeutronT <= Verify_T_Upper)
                    {
                        FinalOutput.Add(FirstList[0].ToString());
                        Console.WriteLine("Secondary校验通过");
                    }
                    else
                    {
                        Console.WriteLine("Secondary校验不通过，放弃这个点搜索");
                    }
                }
                else //多结果筛选，离中心线（球坐标系T P 表达）越近，越容易被筛选 13.05.2017
                {
                    Console.WriteLine("Secondary本次发现多个选项，正在进行筛选");
                    string FromList;
                    string FromSecondList;
                    for (int counter = 0; counter < FirstList.Count; counter++)
                    {
                        FromList = FirstList[counter].ToString();
                        Console.WriteLine("Secondary正在检测" + FromList + "是否最佳");
                        string sqlSearch;
                        double Neutron_X;
                        double Neutron_Y;
                        double Neutron_Z;
                        double NeutronP;
                        double NeutronT;
                        conn.Open();
                        sqlSearch = "SELECT X FROM db WHERE Name Like '%" + FromList + "%'";
                        Console.WriteLine(FromList);
                        cmd = new MySqlCommand(sqlSearch, conn);
                        Neutron_X = (double)cmd.ExecuteScalar();
                        sqlSearch = "SELECT Y FROM db WHERE Name Like '%" + FromList + "%'";
                        cmd = new MySqlCommand(sqlSearch, conn);
                        Neutron_Y = (double)cmd.ExecuteScalar();
                        sqlSearch = "SELECT Z FROM db WHERE Name Like '%" + FromList + "%'";
                        cmd = new MySqlCommand(sqlSearch, conn);
                        Neutron_Z = (double)cmd.ExecuteScalar();
                        conn.Close();
                        NeutronP = coordConvertToP(Neutron_X - playerX, Neutron_Y - playerY, Neutron_Z - playerZ);
                        NeutronT = coordConvertToT(Neutron_X - playerX, Neutron_Y - playerY, Neutron_Z - playerZ);
                        if (Verify_P_Lower <= NeutronP && NeutronP <= Verify_P_Upper && Verify_T_Lower <= NeutronT && NeutronT <= Verify_T_Upper)
                        {
                            SecondList.Add(FromList);
                            Console.WriteLine("Secondary***第一校验通过***");
                        }
                        else
                        {
                            Console.WriteLine("Secondary校验不通过，放弃对" + FromList + "的选择");
                        }
                    }

                    double final_pDiff = 0;
                    double final_tDiff = 0;
                    string final_Name = "";


                    for (int counter = 0; counter < SecondList.Count; counter++)
                    {
                        FromSecondList = FirstList[counter].ToString();
                        string sqlSearch;
                        double Neutron_X;
                        double Neutron_Y;
                        double Neutron_Z;
                        double pDiff;
                        double tDiff;
                        double NeutronP;
                        double NeutronT;
                        conn.Open();
                        sqlSearch = "SELECT X FROM db WHERE Name Like '%" + FromSecondList + "%'";
                        Console.WriteLine(FromSecondList);
                        cmd = new MySqlCommand(sqlSearch, conn);
                        Neutron_X = (double)cmd.ExecuteScalar();
                        sqlSearch = "SELECT Y FROM db WHERE Name Like '%" + FromSecondList + "%'";
                        cmd = new MySqlCommand(sqlSearch, conn);
                        Neutron_Y = (double)cmd.ExecuteScalar();
                        sqlSearch = "SELECT Z FROM db WHERE Name Like '%" + FromSecondList + "%'";
                        cmd = new MySqlCommand(sqlSearch, conn);
                        Neutron_Z = (double)cmd.ExecuteScalar();
                        conn.Close();
                        NeutronP = coordConvertToP(Neutron_X - playerX, Neutron_Y - playerY, Neutron_Z - playerZ);
                        NeutronT = coordConvertToT(Neutron_X - playerX, Neutron_Y - playerY, Neutron_Z - playerZ);
                        pDiff = Math.Abs(Math.Abs(NeutronP) - Math.Abs(target_Scood_P));
                        tDiff = Math.Abs(Math.Abs(NeutronT) - Math.Abs(target_Scood_T));
                        if (counter == 0)
                        {
                            final_pDiff = pDiff;
                            final_tDiff = tDiff;
                            final_Name = FromSecondList;
                            Console.WriteLine("Secondary第一次写入，不对比");
                        }
                        else
                        {
                            if (pDiff < final_pDiff && tDiff < final_tDiff)
                            {
                                final_tDiff = pDiff;
                                final_tDiff = tDiff;
                                final_Name = FromSecondList;
                            }
                            else
                            {
                                Console.WriteLine(FromSecondList + "Secondary不是最优选项");
                            }
                        }
                        Console.WriteLine("SecondaryTEST" + final_Name);
                        FirstResult = final_Name;
                    }
                    Console.WriteLine("SecondaryTEST" + final_Name);
                    FirstResult = final_Name;
                    FinalOutput.Add(final_Name);
                }
            }

            Console.WriteLine("是否继续？");
           // bool Continue = Navigation.Continue(target.name, FinalOutput[FinalOutput.Count - 1].ToString(), anaconda);
           // if (Continue == true)
           // {
           //     FirstResult = FinalOutput[FinalOutput.Count - 1].ToString();
           //     goto Start;
            //}
            //else
            //{
             //   Console.WriteLine("一共找到了" + FinalOutput.Count);
            //}

        }

        public static void SecondaryCalculationType2(string FirstResult)
        {
            Console.WriteLine("输入:" + FirstResult);
            ArrayList FinalOutput = new ArrayList();
            FinalOutput.Add(FirstResult);
            string sql;
            string ConnectionString = "server=127.0.0.1;Database=neutrondb;uid=user;pwd=123456789";
            MySqlConnection conn = new MySqlConnection(ConnectionString);
            double NowNeutron_X;
            double NowNeutron_Y;
            double NowNeutron_Z;

            double Player_X = 46.375;
            double Player_Y = -448.6875;
            double Player_Z = -127.125;

            double Target_X;
            double Target_Y;
            double Target_Z;

            double TargetReferenceToNowNeutronCoodr_X;
            double TargetReferenceToNowNeutronCoodr_Y;
            double TargetReferenceToNowNeutronCoodr_Z;

            double TargetReferenceToNowNeutronSCoodr_R;
            double TargetReferenceToNowNeutronSCoodr_T;
            double TargetReferenceToNowNeutronSCoodr_P;

            double TargetReferenceToPlayerCoodr_X;
            double TargetReferenceToPlayerCoodr_Y;
            double TargetReferenceToPlayerCoodr_Z;

            double TargetReferenceToPlayerSCoodr_R;
            double TargetReferenceToPlayerSCoodr_T;
            double TargetReferenceToPlayerSCoodr_P;

            double NowNeutronReferenceToPlayer_X;
            double NowNeutronReferenceToPlayer_Y;
            double NowNeutronReferenceToPlayer_Z;

            double anaconda = 53.44; //暂时假设这艘anaconda能跳53.44LY
            double anaconda_boost = anaconda * 4; //上高速之后的搜索方法
            double searchVar = 0.5; //搜索范围变量 从1-0.2 

            conn.Open();

            sql = "SELECT X FROM db WHERE Name Like '%" + FirstResult + "%'";
            Console.WriteLine(FirstResult);
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            NowNeutron_X = (double)cmd.ExecuteScalar();
            sql = "SELECT Y FROM db WHERE Name Like '%" + FirstResult + "%'";
            cmd = new MySqlCommand(sql, conn);
            NowNeutron_Y = (double)cmd.ExecuteScalar();
            sql = "SELECT Z FROM db WHERE Name Like '%" + FirstResult + "%'";
            cmd = new MySqlCommand(sql, conn);
            NowNeutron_Z = (double)cmd.ExecuteScalar();

            conn.Close();

            string returnResultFromSearch = Navigation.SearchSystem("");
            Coordinate target = JsonConvert.DeserializeObject<Coordinate>(returnResultFromSearch);//一次解析 解析返回json
            string jsonReturn = Convert.ToString(target.coords);
            Coordinate target_cood = JsonConvert.DeserializeObject<Coordinate>(jsonReturn);//二次解析 解析json内coodr

            Target_X = target_cood.x;
            Target_Y = target_cood.y;
            Target_Z = target_cood.z;

            TargetReferenceToNowNeutronCoodr_X = Target_X - NowNeutron_X;
            TargetReferenceToNowNeutronCoodr_Y = Target_Y - NowNeutron_Y;
            TargetReferenceToNowNeutronCoodr_Z = Target_Z - NowNeutron_Z;

            TargetReferenceToPlayerCoodr_X = Target_X - Player_X;
            TargetReferenceToPlayerCoodr_Y = Target_Y - Player_Y;
            TargetReferenceToPlayerCoodr_Z = Target_Z - Player_Z;

            TargetReferenceToNowNeutronSCoodr_R = coordConvertToR(TargetReferenceToNowNeutronCoodr_X, TargetReferenceToNowNeutronCoodr_Y, TargetReferenceToNowNeutronCoodr_Z);
            TargetReferenceToNowNeutronSCoodr_T = coordConvertToT(TargetReferenceToNowNeutronCoodr_X, TargetReferenceToNowNeutronCoodr_Y, TargetReferenceToNowNeutronCoodr_Z);
            TargetReferenceToNowNeutronSCoodr_P = coordConvertToP(TargetReferenceToNowNeutronCoodr_X, TargetReferenceToNowNeutronCoodr_Y, TargetReferenceToNowNeutronCoodr_Z);

            TargetReferenceToPlayerSCoodr_R = coordConvertToR(TargetReferenceToPlayerCoodr_X, TargetReferenceToPlayerCoodr_Y, TargetReferenceToPlayerCoodr_Z);
            TargetReferenceToPlayerSCoodr_T = coordConvertToT(TargetReferenceToPlayerCoodr_X, TargetReferenceToPlayerCoodr_Y, TargetReferenceToPlayerCoodr_Z);
            TargetReferenceToPlayerSCoodr_P = coordConvertToP(TargetReferenceToPlayerCoodr_X, TargetReferenceToPlayerCoodr_Y, TargetReferenceToPlayerCoodr_Z);

            NowNeutronReferenceToPlayer_X = NowNeutron_X - Player_X;
            NowNeutronReferenceToPlayer_Y = NowNeutron_Y - Player_Y;
            NowNeutronReferenceToPlayer_Z = NowNeutron_Z - Player_Z;

            Console.WriteLine("当前中子星名字是" + FirstResult + ",距离目的地还有" + TargetReferenceToNowNeutronSCoodr_R + "LY");
            //Console.ReadKey();

            double VarTestInt = 12;
            double searchBetween_X_Upper = NowNeutron_X + (anaconda * VarTestInt);
            double searchBetween_X_Lower = NowNeutron_X - (anaconda * VarTestInt);
            double searchBetween_Y_Upper = NowNeutron_Y + (anaconda * VarTestInt);
            double searchBetween_Y_Lower = NowNeutron_Y - (anaconda * VarTestInt);
            double searchBetween_Z_Upper = NowNeutron_Z + (anaconda * VarTestInt);
            double searchBetween_Z_Lower = NowNeutron_Z - (anaconda * VarTestInt);

            sql = "SELECT * FROM db WHERE X BETWEEN " + searchBetween_X_Lower + " AND " + searchBetween_X_Upper + " AND Y BETWEEN " + searchBetween_Y_Lower + " AND " + searchBetween_Y_Upper + " AND Z BETWEEN " + searchBetween_Z_Lower + " AND " + searchBetween_Z_Upper;
            cmd = new MySqlCommand(sql, conn);

            conn.Open();

            ArrayList FirstSelectList_Name = new ArrayList();
            ArrayList FirstSelectList_X = new ArrayList();
            ArrayList FirstSelectList_Y = new ArrayList();
            ArrayList FirstSelectList_Z = new ArrayList();

            MySqlDataReader Reader = cmd.ExecuteReader();
            while (Reader.Read())
            {
                //Console.WriteLine("已经发现Secondary");
                double FromPlayerToTryNeutron = coordConvertToR(
                    Convert.ToDouble(Reader.GetString("X"))-Player_X,
                    Convert.ToDouble(Reader.GetString("Y"))-Player_Y,
                    Convert.ToDouble(Reader.GetString("Z"))-Player_Z);

                double FromPlayerToNowNeutron = coordConvertToR(
                    NowNeutron_X - Player_X,
                    NowNeutron_Y - Player_Y, 
                    NowNeutron_Z - Player_Z);

                //Insert New Selection Method Double verify*******************
                double VerifyR = coordConvertToR(
                    Convert.ToDouble(Reader.GetString("X")) - Player_X,
                    Convert.ToDouble(Reader.GetString("Y")) - Player_Y,
                    Convert.ToDouble(Reader.GetString("Z")) - Player_Z);
                //RefCOODR!!!
                double VerifyPoint_X = ScoordConvertToX(VerifyR, TargetReferenceToPlayerSCoodr_T, TargetReferenceToPlayerSCoodr_P);
                double VerifyPoint_Y = ScoordConvertToY(VerifyR, TargetReferenceToPlayerSCoodr_T, TargetReferenceToPlayerSCoodr_P);
                double VerifyPoint_Z = ScoordConvertToZ(VerifyR, TargetReferenceToPlayerSCoodr_T, TargetReferenceToPlayerSCoodr_P);

                double TryNeutronReferenceToVerifyPoint_X = Convert.ToDouble(Reader.GetString("X")) - VerifyPoint_X;
                double TryNeutronReferenceToVerifyPoint_Y = Convert.ToDouble(Reader.GetString("Y")) - VerifyPoint_Y;
                double TryNeutronReferenceToVerifyPoint_Z = Convert.ToDouble(Reader.GetString("Z")) - VerifyPoint_Z;

                double DistanceBetweenVerifyPointAndTryNeutron = coordConvertToR(TryNeutronReferenceToVerifyPoint_X, TryNeutronReferenceToVerifyPoint_Y, TryNeutronReferenceToVerifyPoint_Z);

                //************************************************************
                if (FromPlayerToNowNeutron < FromPlayerToTryNeutron && DistanceBetweenVerifyPointAndTryNeutron<100)
                {
                    //cancel = true;
                    Console.WriteLine("星系" + Reader.GetString("Name") + "在当前中子星前，且在允许误差范围内,加入第一选择序列");

                    FirstSelectList_Name.Add(Reader.GetString("Name"));
                    FirstSelectList_X.Add(Reader.GetString("X"));
                    FirstSelectList_Y.Add(Reader.GetString("Y"));
                    FirstSelectList_Z.Add(Reader.GetString("Z"));
                }
                else
                {
                    Console.WriteLine("星系" + Reader.GetString("Name") + "在当前中子星后或不在允许误差范围内，不加入第一选择序列");
                }

            }

            conn.Close();
            Console.WriteLine("第一序列检测完成");

            try
            {
                int RemoveIndex = FirstSelectList_Name.IndexOf(FirstResult);
                Console.WriteLine("星系" + FirstResult + ",Index=" + RemoveIndex);

                FirstSelectList_Name.RemoveAt(RemoveIndex);
                FirstSelectList_X.RemoveAt(RemoveIndex);
                FirstSelectList_Y.RemoveAt(RemoveIndex);
                FirstSelectList_Z.RemoveAt(RemoveIndex);
            }
            catch
            {
            }

            if (FirstSelectList_Name.Count == 0)
            {
                Console.WriteLine("本次搜索没有输出结果");
                Console.ReadKey();
            }

            string FromFirstSelectList_Name;
            double FromFirstSelectList_X;
            double FromFirstSelectList_Y;
            double FromFirstSelectList_Z;
            double FromFirstSelectList_Rcoord_X;
            double FromFirstSelectList_Rcoord_Y;
            double FromFirstSelectList_Rcoord_Z;
            double FromNeutronToNext;

            ArrayList PrimarySelectList_Name = new ArrayList();
            ArrayList PrimarySelectList_X = new ArrayList();
            ArrayList PrimarySelectList_Y = new ArrayList();
            ArrayList PrimarySelectList_Z = new ArrayList();

            for (int counter = 0; counter < FirstSelectList_Name.Count; counter++)
            {
                FromFirstSelectList_Name = FirstSelectList_Name[counter].ToString();
                int IndexOfObject = FirstSelectList_Name.IndexOf(FromFirstSelectList_Name);
                FromFirstSelectList_X = Convert.ToDouble(FirstSelectList_X[IndexOfObject].ToString());
                FromFirstSelectList_Y = Convert.ToDouble(FirstSelectList_Y[IndexOfObject].ToString());
                FromFirstSelectList_Z = Convert.ToDouble(FirstSelectList_Z[IndexOfObject].ToString());
                Console.WriteLine("当前星系" + FromFirstSelectList_Name + ",X=" + FromFirstSelectList_X + ",Y=" + FromFirstSelectList_Y + ",Z=" + FromFirstSelectList_Z);

                FromFirstSelectList_Rcoord_X = FromFirstSelectList_X - NowNeutron_X ;
                FromFirstSelectList_Rcoord_Y = FromFirstSelectList_Y - NowNeutron_Y ;
                FromFirstSelectList_Rcoord_Z = FromFirstSelectList_Z - NowNeutron_Z ;

                FromNeutronToNext = coordConvertToR(FromFirstSelectList_Rcoord_X, FromFirstSelectList_Rcoord_Y, FromFirstSelectList_Rcoord_Z);
                if (FromNeutronToNext <= anaconda_boost) //如果在SuperCharge范围内的话，加入到Primary list,同时从FirstSelect中删除
                {
                    PrimarySelectList_Name.Add(FromFirstSelectList_Name);
                    PrimarySelectList_X.Add(FromFirstSelectList_X);
                    PrimarySelectList_Y.Add(FromFirstSelectList_Y);
                    PrimarySelectList_Z.Add(FromFirstSelectList_Z);

                    int RemoveIndexInSelect = FirstSelectList_Name.IndexOf(FromFirstSelectList_Name);

                    FirstSelectList_Name.RemoveAt(RemoveIndexInSelect);
                    FirstSelectList_X.RemoveAt(RemoveIndexInSelect);
                    FirstSelectList_Y.RemoveAt(RemoveIndexInSelect);
                    FirstSelectList_Z.RemoveAt(RemoveIndexInSelect);

                    Console.WriteLine("当前星系" + FirstSelectList_Name + "In SuperCharge Range,Remove from FirstSelectList, Add to PrimaryList.");
                }
                else
                {
                    Console.WriteLine("当前星系" + FirstSelectList_Name + "NOT In SuperCharge Range.");
                }
            }
            Console.WriteLine("Object In PrimaryList=" + PrimarySelectList_Name.Count + ".Object In FirstList(SecondaryList)=" + FirstSelectList_Name.Count);

            string FromPrimarySelectList_Name;
            double FromPrimarySelectList_X;
            double FromPrimarySelectList_Y;
            double FromPrimarySelectList_Z;
            double FromPLayerToNext;
            double ActuallyPush=0;

            for (int counter = 0; counter < PrimarySelectList_Name.Count; counter++)
            {
                FromPrimarySelectList_Name = PrimarySelectList_Name[counter].ToString();
                int IndexOfObject = FromPrimarySelectList_Name.IndexOf(FromPrimarySelectList_Name);

                FromPrimarySelectList_X = Convert.ToDouble(PrimarySelectList_X[IndexOfObject].ToString());
                FromPrimarySelectList_Y = Convert.ToDouble(PrimarySelectList_Y[IndexOfObject].ToString());
                FromPrimarySelectList_Z = Convert.ToDouble(PrimarySelectList_Z[IndexOfObject].ToString());

                Console.WriteLine(FromPrimarySelectList_Name + "DEBUG" + FromPrimarySelectList_X + "," + FromPrimarySelectList_Y + "," + FromPrimarySelectList_Z);

                FromPLayerToNext = coordConvertToR(FromPrimarySelectList_X -Player_X, FromPrimarySelectList_Y -Player_Y, FromPrimarySelectList_Z -Player_Z);
                if (counter == 0)
                {
                    ActuallyPush = FromPLayerToNext;
                    Console.WriteLine("PrimarySelect Start");
                }
                else
                {
                    if (ActuallyPush < FromPLayerToNext && ActuallyPush != 0)
                    {
                        ActuallyPush = FromPLayerToNext;
                        Console.WriteLine("PrimarySelect Update");
                    }
                    else
                    {
                        Console.WriteLine("PrimarySelect does't Update");
                    }
                }

            }

            //IF Primary list nothing, then use Secondary list
        }

        //Note radial distance=r ; polar angle θ (theta)=t ; azimuthal angle φ (phi)=p;
        public static double coordConvertToR(double X, double Y, double Z)
        {
            double r = System.Math.Sqrt(X * X + Y * Y + Z * Z);
            Console.WriteLine(r);
            return r;
        }

        public static double coordConvertToT(double X, double Y, double Z)
        {
            double t = Math.Acos(Z / System.Math.Sqrt(X * X + Y * Y + Z * Z));
            Console.WriteLine(t);
            return t;
        }

        public static double coordConvertToP(double X, double Y, double Z)
        {
            double p = Math.Atan(Y / X);
            Console.WriteLine(p);
            return p;
        }

        public static double ScoordConvertToX(double R, double T, double P)
        {
            double x = R * Math.Sin(T) * Math.Cos(P);
            //Console.WriteLine(x);
            return x;
        }

        public static double ScoordConvertToY(double R, double T, double P)
        {
            double y = R * Math.Sin(T) * Math.Sin(P);
            //Console.WriteLine(y);
            return y;
        }

        public static double ScoordConvertToZ(double R, double T, double P)
        {
            double z = R * Math.Cos(T);
            //Console.WriteLine(z);
            return z;
        }
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
