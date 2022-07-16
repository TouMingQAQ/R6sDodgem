using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace R6s
{
    class Program
    {
        static string basePath = Environment.CurrentDirectory;
        static string userPath = @"/JSON/user.json";
        static List<User> R6User = new List<User>();
        static List<User> user_1 = new List<User>();
        static List<User> user_2 = new List<User>();
        static bool isHead = false;
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("第一次使用，请在程序根目录下JSON/user.json里修改需要排列的队员。");
                Console.WriteLine("如有修改，可以在修改user.json后重新加载排列队员即可");
                Console.WriteLine("车头模式下，user.json的第一队员是A队车头，第二队员是B队车头");
                Console.WriteLine("每次切换模式后，请重新加载排列队员");
                Console.WriteLine("作者：TouMing");
                Menu();

                switch (Console.ReadLine())
                {
                    case "1":
                        try
                        {
                            Start();
                            
                            Sort();
                            
                        }catch(Exception e)
                        {
                            Console.WriteLine("未知错误");
                            Console.WriteLine(e);
                        }
                        Console.Clear();
                        break;
                    case "2":
                        if (user_1.Count == 5 && user_2.Count == 5)
                        {
                            Users[] users = new Users[20];
                            for (int i = 0; i < 20; i++)
                            {
                                users[i] = SortUser(user_1, user_2);
                                user_1 = users[i].u[0];
                                user_2 = users[i].u[1];
                            }
                            for (int i = 1; i < users.Length; i++)
                            {
                                if (users[0].distance > users[i].distance)
                                {
                                    Users u = users[0];
                                    users[0] = users[i];
                                    users[i] = u;
                                }
                            }
                            user_1 = users[0].u[0];
                            user_2 = users[0].u[1];
                        }
                        else
                        {
                            Console.WriteLine("队员数量错误");
                            Thread.Sleep(1000 * 2);
                        }
                        Console.Clear();
                        break;
                    case "3":
                        Console.Write("输入游戏ID：");
                        try
                        {
                            Console.WriteLine("你的休闲分数：" + CodeGET(Console.ReadLine()));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("ID错误，不存在的ID或者格式错误");
                            Console.WriteLine("也有可能网络错误，请检查下网络状况");

                        }
                        Console.WriteLine("输入任意键继续....");
                        Console.ReadKey();
                        //Console.Clear();
                        break;
                    case "4":
                        R6User.Clear();
                        user_1.Clear();
                        user_2.Clear();
                        JObject json = JsonFileRead(userPath);
                        int num = json["num"].Value<int>();
                        for (int i = 0; i < num; i++)
                        {
                            string userName = json["date"][i]["user"].Value<string>();
                            int code = json["date"][i]["scores"].Value<int>();
                            
                            Console.WriteLine($"{userName}:{code}");
                            User user = new User();
                            user.Name = userName;
                            user.Code = code;
                            R6User.Add(user);                           
                        }
                        Sort();
                        ListPrint();
                        Console.WriteLine("输入任意键退出");
                        Console.ReadKey();
                        //Console.Clear();
                        break;
                    case "5":
                        //切换模式
                        if (isHead)
                            isHead = false;
                        else
                            isHead = true;
                        //Console.Clear();
                        break;
                    case "6":
                        //选择车头
                        Console.Clear();   
                        try
                        {
                            ListPrint();
                            Console.WriteLine("输入A队车头序号");
                            int index_1 = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("输入B队车头序号");
                            int index_2 = Convert.ToInt32(Console.ReadLine());
                            var u = user_1[0];
                            user_1[0] = user_1[index_1 - 1];
                            user_1[index_1 - 1] = u;
                            u = user_2[0];
                            user_2[0] = user_2[index_2 - 1];
                            user_2[index_2 - 1] = u;
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("参数错误");
                        }
                        Console.WriteLine("输入任意键退出");
                        Console.ReadKey();
                        
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("参数错误，请重新输入");
                        Thread.Sleep(1000 * 3);
                        //Console.Clear();
                        break;
                }
            }
        }
        static void Menu()
        {
            
            Console.WriteLine("—————————————————————");
            Console.WriteLine("|\t1、加载队员\t\t\t|");
            Console.WriteLine("|\t2、排列队员\t\t\t|");
            Console.WriteLine("|\t3、查询分数\t\t\t|");
            Console.WriteLine("|\t4、直接排列\t\t\t|");
            Console.WriteLine("|\t5、切换模式\t\t\t|");
            Console.WriteLine("|\t6、选择车头(有问题，不推荐使用) |");
            Console.WriteLine("|\t0、退出  \t\t\t|");
            Console.WriteLine("—————————————————————");
            if (isHead)
                Console.WriteLine("当前模式:车头模式");
            else
                Console.WriteLine("当前模式:一般模式");
            
            if (user_1.Count >= 5 && user_2.Count >= 5)
            {
                ListPrint();
                int num = 0;
                foreach (var n in user_1)
                {
                    num += n.Code;
                }
                Console.Write($"|\t\tA队和：{num}\t\t|");
                num = 0;
                foreach (var n in user_2)
                {
                    num += n.Code;

                }
                Console.Write($"\t\tB队和：{num}\t\t|");
                Console.WriteLine();
            }
           
        }
        static Users SortUser(List<User> user_1, List<User> user_2)
        {
            //建立两个表，且表一值必须大于二，否则置换
            int num = 0;
            {
                int add_1 = 0;
                foreach (var n in user_1)
                {
                    add_1 += n.Code;
                }
                int add_2 = 0;
                foreach (var n in user_2)
                {

                    add_2 += n.Code;
                }
                if (add_2 > add_1)
                {
                    List<User> users = user_1;
                    user_1 = user_2;
                    user_2 = users;
                }
                //两表各自和的相差值的一半
                num = (add_1 - add_2) / 2;
            }
            List<Vule> vules = new List<Vule>();
            int num_Head = 0;
            if (isHead)
                num_Head = 1;
            for (int i = num_Head; i < user_1.Count; i++)
            {
                for (int j = num_Head; j < user_2.Count; j++)
                {
                    Vule vule = new Vule();
                    vule.Num_Left = i;
                    vule.Num_Right = j;
                    vule.DistanceSet(user_1[i].Code, user_2[j].Code, num);
                    vules.Add(vule);
                }
            }
            for (int i = 0; i < vules.Count; i++)
            {
                for (int j = i + 1; j < vules.Count; j++)
                {
                    if (vules[i].Distance > vules[j].Distance)
                    {
                        Vule vule = vules[i];
                        vules[i] = vules[j];
                        vules[j] = vule;
                    }
                }
            }
            Console.WriteLine($"{user_1[vules[0].Num_Left].Name}与{user_2[vules[0].Num_Right].Name}置换|误差:{vules[0].Distance}");
            //distance = 
            User u = user_1[vules[0].Num_Left];
            user_1[vules[0].Num_Left] = user_2[vules[0].Num_Right];
            user_2[vules[0].Num_Right] = u;
            ListPrint();
            Users users1 = new Users();
            users1.u = new List<User>[2];
            users1.u[0] = user_1;
            users1.u[1] = user_2;
            users1.distance = vules[0].Distance;
            //List<User>[] user = new List<User>[2];
            //user[0] = user_1;
            //user[1] = user_2;
            return users1;


        }
        static void Start()
        {
            user_1.Clear();
            user_2.Clear();
            R6User.Clear();
            JObject json = JsonFileRead(userPath);
            int num = json["num"].Value<int>();
            for (int i = 0; i < num; i++)
            {
                string userName = json["date"][i]["user"].Value<string>();
                int code;
                try
                {
                    code = CodeGET(userName);
                }
                catch (Exception e)
                {
                    code = 0;
                    Console.WriteLine($"{userName}加载错误，请检查名称");
                }
                Console.WriteLine($"{userName}:{code}");
                User user = new User();
                user.Name = userName;
                user.Code = code;
                R6User.Add(user);
            }
            Console.WriteLine("初始数据加载完成");
        }
        static void Sort()
        {
            if (!isHead)
            {
                for (int i = 0; i < R6User.Count - 1; i++)
                {
                    for (int j = i + 1; j < R6User.Count; j++)
                    {
                        if (R6User[i].Code < R6User[j].Code)
                        {
                            User user = R6User[i];
                            R6User[i] = R6User[j];
                            R6User[j] = user;
                        }
                    }
                }
            }
            for (int i = 0; i < R6User.Count; i++)
            {
                if (i % 2 == 0)
                    user_1.Add(R6User[i]);
                else
                    user_2.Add(R6User[i]);
            }
        }
        static void ListPrint()
        {
            Console.WriteLine("——————————————————————————————————————————");
            Console.WriteLine($"|\t\tA队\t\t\t|\t\tB队\t\t\t|");
            for (int i = 0; i < 5; i++)
            {
                if (user_1[i].Name.Length <= 7)
                    Console.Write($"|\t\t{i + 1}:{user_1[i].Name}:{user_1[i].Code}\t\t|");
                else
                    Console.Write($"|\t\t{i + 1}:{user_1[i].Name}:{user_1[i].Code}\t|");

                if (user_2[i].Name.Length <=7)
                    Console.Write($"\t\t{ i + 1}:{ user_2[i].Name}:{ user_2[i].Code}\t\t|\n");
                else
                    Console.Write($"\t\t{i + 1}:{user_2[i].Name}:{user_2[i].Code}\t|\n");

            }
            Console.WriteLine("——————————————————————————————————————————");


        }
        //static void 
        static private int CodeGET(String userName)
        {
            string url = "https://www.r6s.cn/v2/stats/index?username=" + userName;
            string referer = "https://www.r6s.cn/stats.jsp?username=" + userName;
            var c = new RestClient(url);
            var req = new RestRequest();
            req.Method = Method.Get;
            req.AddHeader("Referer", referer);
            RestResponse resp = c.Execute(req);
            JObject json = JObject.Parse(resp.Content);
            int code = Convert.ToInt32(json["Casualstat"]["mmr"]);
            return code;
        }
        /// <summary>
        /// 读取json文件数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static JObject JsonFileRead(string path)
        {
            StreamReader reader = File.OpenText(basePath + path);
            JsonTextReader jsonTextReader = new JsonTextReader(reader);
            JObject jsonObject = (JObject)JToken.ReadFrom(jsonTextReader);
            reader.Close();
            return jsonObject;
        }
        /// <summary>
        /// 对json文件增添数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        static bool JsonFileWrite(string path, JObject value)
        {
            //path = basePath + path;
            try
            {
                JObject json = JsonFileRead(path);
                int num = json["num"].Value<int>();
                num++;
                json["num"] = num;
                json["date"].Last.AddAfterSelf(value);
                string str = Convert.ToString(json);
                File.WriteAllText(basePath + path, str);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    class User
    {
        public string Name { get; set; }
        public int Code { get; set; }
    }
    class Vule
    {
        public int Num_Right { get; set; }
        public int Num_Left { get; set; }
        private int distance;
        public int Distance { get => distance; }

        public void DistanceSet(int num_1, int num_2, int num)
        {
            distance = Math.Abs(num_1 - num_2 - num);
        }
    }
    class Users
    {
        public List<User>[] u;
        public int distance;
    }
}
