using Newtonsoft.Json;
using Ruanmou.Framework.Log;
using Ruanmou.Framework.Serialize;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ruanmou.ThreadHomework.TianLongBaBu
{
    /// <summary>
    /// 1 数据动态配置，按字节输出，背景色
    /// 2 多线程顺序控制
    /// 3 监控线程
    /// 4 其他线程封装
    /// 
    /// </summary>
    class Program
    {
        private static readonly object lockObj = new object();
        static void Main(string[] args)
        {
            try
            {
                //Console.WriteLine("*********请输入字符串**********");
                //string txt = Console.ReadLine();
                //ReverseStr(txt, 3);

                ManualResetEvent mre = new ManualResetEvent(false);//使用ManualResetEvent等待某个线程完成后
                Console.WriteLine($"任务开始");
                //ThreadPool.QueueUserWorkItem((o) =>
                //{
                //    Thread.Sleep(1000);
                //    Console.WriteLine($"this is ThreadPool {Thread.CurrentThread.ManagedThreadId}");

                //    mre.Set();//动作完成后set(),需要等待拿结果时WaitOne()能马上拿到信号
                //});
                List<int> result = new List<int>();
                for (int j = 0; j < 10; j++)
                {
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        result.Add(j);
                        //Console.WriteLine($"this is ThreadPool {Thread.CurrentThread.ManagedThreadId}");
                    });
                }
                Console.WriteLine($"Do something else");
                Console.WriteLine($"Do something else");
                Console.WriteLine($"Do something else");
                Console.WriteLine($"Do something else");
                Console.WriteLine($"Do something else");
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    //
                    while (true)
                    {

                        if (result.Count == 10)
                        {
                            for (int j = 0; j < result.Count; j++)
                            {
                                Console.WriteLine($"result= {result[j]}");
                            }
                            mre.Set();
                            break;
                        }
                        //else
                        //    mre.WaitOne();
                    }
                });
                mre.WaitOne();// mre.Set();后，waitOne就能收到信号
                Console.WriteLine($"任务完成");


                CancellationTokenSource cts = new CancellationTokenSource();

                //TaskScheduler.UnobservedTaskException+=(s,e)=>{

                //};

                //var obj = new { id = 1, name = "掌声" };
                //Task.Factory.StartNew(o =>
                //{
                //    try
                //    {
                //        Console.WriteLine($"Task.Factory.StartNew开始了:{JsonConvert.SerializeObject(o)}");
                //        throw new Exception();
                //        //int iresult = Convert.ToInt32("rewr");
                //    }
                //    catch (Exception ex)
                //    {
                //        throw;
                //    }
                //}, obj, cts.Token).ContinueWith(t =>
                //{
                //    if (t.Exception != null)
                //        Console.WriteLine($"Task.Factory.StartNew异常了：{t.Exception.InnerException}");
                //    else
                //    {
                //        Console.WriteLine($"Task.Factory.ContinueWith继续开始了");
                //    }
                //});//失败时执行
                //Task.Run(() =>
                //{ Console.WriteLine("Task.Run又开始了"); });
                //Task d = new Task(() => { Console.WriteLine("Task.Start 又开始了"); });
                //d.Start();
                //Thread thread = new Thread(() => { Console.WriteLine("thread 又开始了"); });
                //thread.Start();

                //Action action = () => { Console.WriteLine("Action.BeginInvoke 又开始了"); int iresult = Convert.ToInt32("rewr"); };
                //AsyncCallback asyncCallback = ar => {
                //    Console.WriteLine($"asyncCallback又开始了:{JsonConvert.SerializeObject(ar.AsyncState)}");
                //};
                //action.BeginInvoke(asyncCallback, obj);
                Console.ReadKey();
                return;
                bool isMonitor = true;

                List<StoryCharacter> scList = JsonHelper.JsonFileToObject<List<StoryCharacter>>("StoryCharacter.json");
                //List<Task<StoryCharacter>> taskList = new List<Task<StoryCharacter>>();
                Stopwatch watch = new Stopwatch();
                watch.Start();
                int i = 0;
                List<Task> taskList = new List<Task>();
                foreach (var sc in scList)
                {
                    taskList.Add(Task.Factory.StartNew(o =>
                    {
                        foreach (var exp in sc.Experience)
                        {
                            if (i == 0)
                            {
                                lock (lockObj)
                                {
                                    LogHelper.LogConsole($"人物：{ sc.Name} ，事件：{ exp}", sc.Color);
                                    if (i == 0)
                                    {
                                        LogHelper.LogConsole("天龙八部就此拉开序幕。。。。", ConsoleColor.DarkBlue);
                                        i++;
                                    }
                                }
                            }
                            else
                            {
                                LogHelper.LogConsole($"人物：{ sc.Name} ，事件：{ exp}", sc.Color);
                            }
                            if (cts.IsCancellationRequested)
                                break;
                        }
                    }, sc.Name, cts.Token));
                }
                Task.Run(() =>
                {
                    while (isMonitor && new Random().Next(2017, 20301) != DateTime.Now.Year)
                    {
                        Thread.Sleep(10);
                    }
                    if (isMonitor)
                    {
                        LogHelper.LogConsole("天降雷霆灭世，天龙八部的故事就此结束....", ConsoleColor.White);
                        cts.Cancel();
                    }
                    else
                    {
                        LogHelper.LogConsole("故事都结束了，监控线程也不需要了。。。", ConsoleColor.White);
                    }
                });

                Task.Factory.ContinueWhenAny(taskList.ToArray(), t =>
                {
                    if (!cts.IsCancellationRequested)
                        LogHelper.LogConsole($"{t.AsyncState}已经做好准备啦。。。。", ConsoleColor.White);
                });
                //也可以waitall
                Task.Factory.ContinueWhenAll(taskList.ToArray(), tArray =>
                {
                    if (!cts.IsCancellationRequested)
                    {
                        LogHelper.LogConsole("中原群雄大战辽兵，忠义两难一死谢天", ConsoleColor.White);
                        watch.Stop();
                        LogHelper.LogConsole($"天龙八部一共耗时{watch.ElapsedMilliseconds}ms", ConsoleColor.White);
                        isMonitor = false;
                    }
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogConsole(ex.Message, ConsoleColor.White);
            }
            Console.ReadKey();
        }

        /// <summary>
        /// 给定一个字符串 s 和一个整数 k，从字符串开头算起，每 2k 个字符反转前 k 个字符。
        ///如果剩余字符少于 k 个，则将剩余字符全部反转。
        ////如果剩余字符小于 2k 但大于或等于 k 个，则反转前 k 个字符，其余字符保持原样。
        /// </summary>
        /// 如：输入：s = "abcdefg", k = 2 输出："bacdfeg"
        /// 如  ：输入：s = "abcd", k = 2 输出："bacd"
        /// <param name="s"></param>
        /// <param name="k"></param>
        public static void ReverseStr(string s, int k)
        {
            //if (string.IsNullOrEmpty(s) || k == 0) return "";
            string result = string.Empty; string tempstr = string.Empty;
            s = s.Replace(" ", "");
            int index = 0; int currentCnts = (k * 2);
            for (int i = 0; i < s.Length; i+= (k * 2))
            {
                if (i % (k * 2) == 0 && s.Length >= currentCnts)
                {
                    tempstr = string.Join("", s.ToArray().Skip(index * (k * 2)).Take((k * 2) - k).Reverse()) + string.Join("", s.ToArray().Skip(index * (k * 2) + k).Take((k * 2) - k));
                    result += tempstr;
                    var leaveStr = s.Substring(currentCnts, s.Length - currentCnts);
                    if (leaveStr.Length <= k)
                    {
                        leaveStr = string.Join("", leaveStr.ToArray().Reverse());
                        result += leaveStr;
                    }
                    else if (leaveStr.Length > k && leaveStr.Length < k * 2)
                    {
                        var rd = string.Join("", leaveStr.Substring(0, k).ToArray().Reverse());
                        result += rd + leaveStr.Substring(k);
                    }
                    index++;
                    currentCnts = (index + 1) * (k * 2);
                }

            }
            Console.WriteLine(string.Join(" ", result.ToArray()));
            string txt = Console.ReadLine();
            ReverseStr(txt, 3);
        }

    }
}
