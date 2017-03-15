using LUISLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace BoBot
{
    public class VendingLuisHelper
    {
        public const string HELLO_ANSWERE = "您好，我是贩卖机管理小助手，请您先登录系统，输入验证码：";
        public const string A_VERIFY = "验证成功，请问有什么可以帮您 ？ ";
        public const string A_MACHINE_PRODUCT_STATUS = "号贩卖机";
        public const string A_MACHINE_PRODUCT_STATUS1 = "瓶可乐，";
        public const string A_MACHINE_PRODUCT_STATUS2 = "瓶雪碧。";
        public const string A_SUPPLY_PRODUCT = "已经安排了。请问还有什么可以帮您 ？";
        public const string MACHINE_STATUS = "号贩卖机目前工作正常，但是，根据我们的预测，周六需要补充";
        public const string MACHINE_STATUS2 = "瓶可乐。";
        public const string A_MACHINE_LOCATION = "号贩卖机的具体位置请参考地图链接 http://j.map.baidu.com/TDG0F";
        public const string A_MACHINE_POWERPI = "贩卖机整体状态PowerBi链接请参考 https://msit.powerbi.com/view?r=eyJrIjoiMTY4YTIwMDgtY2I4NS00OWY5LWIwYzMtOWE0ZDljNjQ3M2I4IiwidCI6IjcyZjk4OGJmLTg2ZjEtNDFhZi05MWFiLTJkN2NkMDExZGI0NyIsImMiOjV9";
        public const string ENTITY_MACHINENO = "";
        public const string RE_CONFIRM = "不客气，请问还有什么可以帮您?";
        public const string STATUS_QUERY_URL = "http://smartapi.chinacloudsites.cn/api/v1/querygoodsstatus?deviceid=98011609002D";
        public const string TEMPER_QUERY_URL = "http://smartapi.chinacloudsites.cn/api/v1/querymachinestatus?deviceid=98011609002D";
        private string machineNo = "1234";
        public static string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        public class QueryGoodsStatus
        {
            public Goodsdata[] goodsdata { get; set; }
            public bool success { get; set; }
            public string message { get; set; }
            public int code { get; set; }
        }
        public class Goodsdata
        {
            public int goodsno { get; set; }
            public int goodsstock { get; set; }
            public long svmtime { get; set; }
            public string goodsname { get; set; }
            public int goodsstatus { get; set; }
        }

        public class MachineTemper
        {
            public Areadata[] areadatas { get; set; }
            public bool success { get; set; }
            public string message { get; set; }
            public int code { get; set; }
        }

        public class Areadata
        {
            public string warmareaname { get; set; }
            public int temperature { get; set; }
            public int temperaturemode { get; set; }
        }

        public static string GetAnswer(string Question, LUISResult LuisResult)
        {
            string message = null;
            switch (Question)
            {
                case "":
                    message = "对不起，目前我不支持这项服务";
                    break;

                case "验证码":
                    message = A_VERIFY;
                    break;
                case "打招呼":
                    message = HELLO_ANSWERE;
                    break;
                case "贩卖机状态":
                    Random rd = new Random();
                    int number1 = rd.Next(1, 50);
                    int number2 = rd.Next(1, 15);
                    int numMachine = rd.Next(1000, 2000);
                    string num1 = Convert.ToString(number1);
                    string num2 = Convert.ToString(number2);
                    string machineNo = Convert.ToString(numMachine);

                    string machineStatus = HttpGet(STATUS_QUERY_URL, "");

                    QueryGoodsStatus goodsData = (QueryGoodsStatus)JsonConvert.DeserializeObject(machineStatus, typeof(QueryGoodsStatus));
                    if (true == goodsData.success)
                    {
                        StringBuilder MyStringBuilder = new StringBuilder();
                        string tempResult = "";
                        for (int i = 0; i < goodsData.goodsdata.Length; i++)
                        {
                            if (goodsData.goodsdata[i].goodsstatus == 1)
                            {
                                tempResult = goodsData.goodsdata[i].goodsno.ToString() + "货道" + "故障" +
                                    "商品名称：" + goodsData.goodsdata[i].goodsname;
                                MyStringBuilder.Append(tempResult);

                                MyStringBuilder.AppendLine("");
                            }
                        }
                        string finalStatusResult = MyStringBuilder.ToString();

                        if (finalStatusResult.Length > 1)
                        {
                            message = "0127" + A_MACHINE_PRODUCT_STATUS + finalStatusResult;
                        }
                        else
                        {
                            message = "0127" + A_MACHINE_PRODUCT_STATUS + "工作正常";
                        }
                    }
                    else
                    {
                        message = "无法查询到贩卖机状态，请稍后重试";
                    }
                    break;

                case "有无货物":

                    message = goodsStatus();


                    break;

                case "货道状态":
                    message = goodsLineStatus(LuisResult);
                    break;

                case "货道商品的名称":
                    message = goodsLineName(LuisResult);
                    break;

                case "补货请求":
                    message = A_SUPPLY_PRODUCT;
                    break;

                case "特定贩售机状态":
                    Random rd2 = new Random();
                    int number3 = rd2.Next(1, 100);
                    string num3 = Convert.ToString(number3);
                    machineNo = LuisResult.LUISEntity.entity;
                    message = machineNo + MACHINE_STATUS + num3 + MACHINE_STATUS2;
                    machineNo = "1234";
                    break;

                case "贩售机位置":
                    message = "0127号" + A_MACHINE_LOCATION;
                    break;

                case "整体状态":
                    message = "0127号" + A_MACHINE_POWERPI;
                    break;

                case "感谢":
                    message = RE_CONFIRM;
                    break;

                case "贩卖机的温度":
                    message = tempuratureQuery();
                    break;

                case "目前温度模式":
                    message = tempuratureModeQuery();
                    break;

                    //case "":
                    //    message = null;
                    //    break;

                    //case "":
                    //    message = null;
                    //    break;

                    //case "":
                    //    message = null;
                    //    break;

                    //case "":
                    //    message = null;
                    //    break;
            }

            return message;
        }

        public static string tempuratureModeQuery()
        {
            string machineTemperStatus = HttpGet(TEMPER_QUERY_URL, "");
            MachineTemper machineTemper = (MachineTemper)JsonConvert.DeserializeObject(machineTemperStatus, typeof(MachineTemper));
            if (true == machineTemper.success)
            {
                //List<Goodsdata> shortageGoods = new List<Goodsdata>();
                List<string> resultMachineTmp = new List<string>();
                StringBuilder MyStringBuilder = new StringBuilder();
                string tempResult = "";
                string temperMode = "关闭";
                for (int i = 0; i < machineTemper.areadatas.Length; i++)
                {
                    switch (machineTemper.areadatas[i].temperaturemode)
                    {
                        case 0:
                            temperMode = "关闭";
                            break;
                        case 1:
                            temperMode = "加热";
                            break;
                        case 2:
                            temperMode = "制冷";
                            break;
                        default:
                            break;
                    }
                    tempResult = machineTemper.areadatas[i].warmareaname + "模式:" + temperMode;
                    MyStringBuilder.Append(tempResult);
                    MyStringBuilder.AppendLine("");
                }
                string finalStatusResult = MyStringBuilder.ToString();
                return ("0127" + A_MACHINE_PRODUCT_STATUS + "温度模式如下: \r\n" + finalStatusResult);
            }
            else
            {
                return ("无法查询到贩卖机状态，请稍后重试");
            }
        }

        public static string tempuratureQuery()
        {
            string machineTemperStatus = HttpGet(TEMPER_QUERY_URL, "");
            MachineTemper machineTemper = (MachineTemper)JsonConvert.DeserializeObject(machineTemperStatus, typeof(MachineTemper));
            if (true == machineTemper.success)
            {
                //List<Goodsdata> shortageGoods = new List<Goodsdata>();
                List<string> resultMachineTmp = new List<string>();
                StringBuilder MyStringBuilder = new StringBuilder();
                string tempResult = "";
                string temperMode = "关闭";
                for (int i = 0; i < machineTemper.areadatas.Length; i++)
                {
                    switch (machineTemper.areadatas[i].temperaturemode)
                    {
                        case 0:
                            temperMode = "关闭";
                            break;
                        case 1:
                            temperMode = "加热";
                            break;
                        case 2:
                            temperMode = "制冷";
                            break;
                        default:
                            break;
                    }
                    tempResult = machineTemper.areadatas[i].warmareaname + "温度:" + machineTemper.areadatas[i].temperature.ToString() +
                        " 温度模式:" + temperMode;
                    MyStringBuilder.Append(tempResult);
                    MyStringBuilder.AppendLine("");
                }
                string finalStatusResult = MyStringBuilder.ToString();
                return ("0127" + A_MACHINE_PRODUCT_STATUS + "温度状态如下: \r\n" + finalStatusResult);
            }
            else
            {
                return ("无法查询到贩卖机状态，请稍后重试");
            }

        }

        public static string goodsLineName(LUISResult LuisResult)
        {
            string machineLineNo = LuisResult.LUISEntity.entity;
            string machineStatus = HttpGet(STATUS_QUERY_URL, "");
            QueryGoodsStatus goodsData = (QueryGoodsStatus)JsonConvert.DeserializeObject(machineStatus, typeof(QueryGoodsStatus));
            if (true == goodsData.success)
            {
                StringBuilder MyStringBuilder = new StringBuilder();
                string tempResult = "";
                string goodsStockStatus = "";
                //int goodsStockNo;
                for (int i = 0; i < goodsData.goodsdata.Length; i++)
                {
                    if (goodsData.goodsdata[i].goodsno.ToString() == machineLineNo)
                    {
                        //shortageGoods.Add(goodsData.goodsdata[i]);
                        //goodsStockNo = goodsData.goodsdata[i].goodsno;
                        if (0 == goodsData.goodsdata[i].goodsstock)
                        {
                            goodsStockStatus = "有货";
                        }
                        else
                        {
                            goodsStockStatus = "缺货";
                        }
                        tempResult = goodsData.goodsdata[i].goodsno.ToString() + "货道" +
                            "商品名称：" + goodsData.goodsdata[i].goodsname + goodsStockStatus;
                        MyStringBuilder.Append(tempResult);
                        MyStringBuilder.AppendLine("");
                        break;
                    }
                }
                if (goodsStockStatus.Length > 1)
                {
                    string finalStatusResult = MyStringBuilder.ToString();
                    return ("0127" + A_MACHINE_PRODUCT_STATUS + finalStatusResult);
                }
                else
                {
                    return ("0127" + A_MACHINE_PRODUCT_STATUS + "货道" + machineLineNo
                        + "不存在");
                }
            }
            else
            {
                return ("无法查询到贩卖机状态，请稍后重试");
            }
        }

        public static string goodsLineStatus(LUISResult LuisResult)
        {
            string machineLineNo = LuisResult.LUISEntity.entity;
            string machineStatus = HttpGet(STATUS_QUERY_URL, "");
            QueryGoodsStatus goodsData = (QueryGoodsStatus)JsonConvert.DeserializeObject(machineStatus, typeof(QueryGoodsStatus));
            if (true == goodsData.success)
            {
                StringBuilder MyStringBuilder = new StringBuilder();
                string tempResult = "";
                string goodsStockStatus = "";
                string goodsStatus;
                //int goodsStockNo;
                for (int i = 0; i < goodsData.goodsdata.Length; i++)
                {
                    if (goodsData.goodsdata[i].goodsno.ToString() == machineLineNo)
                    {
                        //shortageGoods.Add(goodsData.goodsdata[i]);
                        //goodsStockNo = goodsData.goodsdata[i].goodsno;
                        if (0 == goodsData.goodsdata[i].goodsstock)
                        {
                            goodsStockStatus = "有货";
                        }
                        else
                        {
                            goodsStockStatus = "缺货";
                        }
                        if (0 == goodsData.goodsdata[i].goodsstatus)
                        {
                            goodsStatus = "正常,";
                        }
                        else
                        {
                            goodsStatus = "故障,";
                        }
                        tempResult = goodsData.goodsdata[i].goodsno.ToString() + "号货道" + goodsStatus +
                            "商品名称：" + goodsData.goodsdata[i].goodsname + goodsStockStatus;
                        MyStringBuilder.Append(tempResult);
                        MyStringBuilder.AppendLine("");
                        break;
                    }
                }
                if (goodsStockStatus.Length > 1)
                {
                    string finalStatusResult = MyStringBuilder.ToString();
                    return ("0127" + A_MACHINE_PRODUCT_STATUS + finalStatusResult);
                }
                else
                {
                    return ("0127" + A_MACHINE_PRODUCT_STATUS + "货道" + machineLineNo
                        + "不存在");
                }
            }
            else
            {
                return ("无法查询到贩卖机状态，请稍后重试");
            }
        }

        public static string goodsStatus()
        {
            string machineStatus = HttpGet(STATUS_QUERY_URL, "");
            QueryGoodsStatus goodsData = (QueryGoodsStatus)JsonConvert.DeserializeObject(machineStatus, typeof(QueryGoodsStatus));
            if (true == goodsData.success)
            {
                List<Goodsdata> shortageGoods = new List<Goodsdata>();
                List<string> resultGoods = new List<string>();
                StringBuilder MyStringBuilder = new StringBuilder();
                string tempResult = "";
                int goodsStockNo;
                for (int i = 0; i < goodsData.goodsdata.Length; i++)
                {
                    if (goodsData.goodsdata[i].goodsstock == 1)
                    {
                        shortageGoods.Add(goodsData.goodsdata[i]);
                        goodsStockNo = goodsData.goodsdata[i].goodsno;
                        tempResult = goodsData.goodsdata[i].goodsno.ToString() + "货道" +
                            goodsData.goodsdata[i].goodsname + "缺货";
                        MyStringBuilder.Append(tempResult);
                        MyStringBuilder.AppendLine("");
                    }
                }
                string finalStatusResult = MyStringBuilder.ToString();
                if (finalStatusResult.Length > 1)
                {
                    return ("0127" + A_MACHINE_PRODUCT_STATUS + finalStatusResult);
                }
                else
                {
                    return ("0127" + A_MACHINE_PRODUCT_STATUS + "货物充足");
                }

            }
            else
            {
                return ("无法查询到贩卖机状态，请稍后重试");
            }
        }
    }
}