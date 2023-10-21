using DataService.carclass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DataService
{
    public class valueset
    {
        public short Id { get; set; }

        public object Value { get; set; }

    }
    public class WalkArea
    {
        public short Id { get; set; }
        public Int32 StartPoisiton { get; set; }
        public Int32 EndPoisiton { get; set; }
        public Int32 Decstance { get; set; }
        public Int32 Stopstance { get; set; }
        public float DecSpeed { get; set; }
        public float StopSpeed { get; set; }
    }
    public class Protectarea
    {
        public short Id { get; set; }
        public Int32 Dec_Startarea { get; set; }
        public Int32 dec_endarea { get; set; }
        public Int32 stop_startarea { get; set; }
        public Int32 stop_endarea { get; set; }
        public float DecSpeed { get; set; }
        public float StopSpeed { get; set; }
    }
    public class Backstage
    {


        List<ITag> _heartlist;
        
        Dictionary<int, IGroup> _usegroup;
        Dictionary<short,short> Carnumber = new Dictionary<short, short>();                     //正在使用的小车ID集合
        //通过id得到对应的Tag名字
        public Dictionary<string, ITag> _maping { get; set; }
        public List<TagMetaData> _list { get; set; }
        public ITag this[short id]
        {
            get
            {
                int index = GetItemProperties(id);                      //通过id号得到在集合中的序号
                if (index >= 0)
                {
                    return this[_list[index].Name];                    //返回集合中的对用序号的标签名
                }
                return null;
            }
        }
        //通过名字得到对应的Tag
        public ITag this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name)) return null;
                ITag dataItem;
                _maping.TryGetValue(name.ToUpper(), out dataItem);
                return dataItem;
            }
        }

       
        public IList<TagMetaData> MetaDataList
        {
            get
            {
                return _list;
            }
        }
        // public Dictionary<short, short> Carnumber { get; set; }
        Dictionary<short, Int32> _liststation_poisition = new Dictionary<short, Int32>();
        List<Task_Data> _listask = new List<Task_Data>();
        List<WalkArea> Space_dataarry = new List<WalkArea>();          //配置的调度减速、停止距离的区域
        Protectarea[] Protect_Area = new Protectarea[60];
       // List<WalkArea> Protect_Area = new List<WalkArea>();            //根据各车的实时位置得到锁定的区域
        List<valueset> Car_Poisition = new List<valueset>();           //小车实时位置集合
        Int32 Maxpoisition = new Int32();                             //最大位置值
        

        #region 注册小车id到集合中
        public void Setonline(short id,short nember)
        {
            if(!Carnumber.ContainsKey(id))
            {
                Carnumber.Add(id, nember);
            }


        }
        #endregion

        #region 通过id号得到在集合中的序号
        /// <summary>
        /// 通过id号得到在集合中的序号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetItemProperties(short id)
        {
            return _list.BinarySearch(new TagMetaData { ID = id });
        }
        #endregion
        #region 通过心跳集合，设置true给心跳给小车
        /// <summary>
        /// 通过心跳集合，设置true给心跳给小车
        /// </summary>
        /// <param name="tg"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<valueset> Getheartprotect(List<ITag> tg,bool value)
        {    
            Dictionary<int, bool> data =new Dictionary<int, bool>();
            List<valueset> data1 = new List<valueset>();
            valueset set = new valueset();
            foreach (ITag WriteTag in tg )
            {
                set.Id = WriteTag.ID;
                bool i = true;
                set.Value = i;
                data1.Add(set);
            }
            return data1;

        }
        #endregion
        #region 通过读各个驱动的连接情况，导出对应的通讯失败标志
        /// <summary>
        /// 通过读各个驱动的连接情况，导出对应的通讯失败标志
        /// </summary                                                                                                                                                                                                                          
        /// <param name="dr"></param>
        /// <returns></returns>
        public List<valueset> connectfos(SortedList<short, IDriver> dr)
        {
            List<valueset> data = new List<valueset>();
            bool i=new bool() ;
            valueset set = new valueset();
            foreach (IDriver dd in dr.Values)
            {
                if (dd.IsClosed)
                {
                    i = true;


                }
                else
                {
                    i = false;

                }
                set.Id = dd.ID;
                set.Value = i;
                data.Add(set);

            }
            return data;
            }
        #endregion
        #region 根据实时的小车位置，生成保护锁定区域
        /// <summary>
        /// 根据实时的小车位置，生成保护锁定区域
        /// </summary>
        /// <param name="walk"></param>
        /// <returns></returns>
        public void LockArea(List<WalkArea> walk, List<valueset> car)
        {

            List<WalkArea> protect =new List<WalkArea>();
            int i = 0;
            int j=0;
            foreach (valueset carp in car )
            {
                if ((Int32) carp.Value != 0)
                { 
                  foreach (WalkArea area in walk)
                  {
                    if (((Int32)carp.Value>area.StartPoisiton) && ((Int32)carp.Value < area.EndPoisiton))
                    {

                        Protect_Area[i].Dec_Startarea = (Int32)carp.Value - area.Decstance;
                        Protect_Area[i].dec_endarea = (Int32)carp.Value + area.Decstance;
                        Protect_Area[i].stop_startarea = (Int32)carp.Value - area.Stopstance;
                        Protect_Area[i].stop_endarea = (Int32)carp.Value + area.Stopstance;
                        if (Protect_Area[i].dec_endarea>Maxpoisition && Protect_Area[i].dec_endarea < Maxpoisition)                       
                            Protect_Area[i].dec_endarea =Protect_Area[i].dec_endarea - Maxpoisition + 1000;
                       
                         if (Protect_Area[i].stop_endarea > Maxpoisition && Protect_Area[i].stop_endarea < Maxpoisition)                      
                            Protect_Area[i].stop_endarea = Protect_Area[i].stop_endarea - Maxpoisition + 1000;

                        if (Protect_Area[i].Dec_Startarea < 1000) 
                            Protect_Area[i].Dec_Startarea = Maxpoisition + Protect_Area[i].Dec_Startarea - 1000;
                        if (Protect_Area[i].stop_startarea < 1000)
                            Protect_Area[i].stop_startarea = Maxpoisition + Protect_Area[i].stop_startarea - 1000;

                    }
                    j++;
                  }                 
                }
                else
                {
                    Protect_Area[i].Dec_Startarea = 0;
                    Protect_Area[i].dec_endarea = 0;
                    Protect_Area[i].stop_startarea = 0;
                    Protect_Area[i].stop_endarea = 0;
                }
                i++;
            }               
        }
        #endregion
        #region 判断任务是否重复，添加新的任务到队列中
        public void Task_Creart(short stationid , short tasktype,short priority)
        {
            Int32 stationpois=new Int32();
            bool repetition=false;
            if (_liststation_poisition.ContainsKey(stationid))                                      //判断给定的站id是否有对应的站位置
            {
               _liststation_poisition.TryGetValue(stationid, out stationpois);                      
                int num = _listask.Count();
                foreach(Task_Data task in _listask )                                               
                {

                    if (!(stationid==task.Station_ID && tasktype==task.Task_Type))                   //判断新加的任务与任务队列的任务是否重复
                    {

                        repetition = true;
                        break;

                    }
                }
                if(!repetition) 
                {
                     Task_Data addtask = new Task_Data();
                    addtask.Task_Type = tasktype;
                    addtask.Priority = priority;
                    addtask.Station_ID = stationid;
                    addtask.Station_Position = stationpois;
                    addtask.End_ID = 1;
                    addtask.End_Poisition = 67325;
                    addtask.Targe_ID = 0;
                    addtask.Targe_Position = 0;
                    addtask.sumcheck = addtask.Task_Type + addtask.Task_Type + addtask.Station_ID+priority
                        + addtask.Station_Position + addtask.End_ID + addtask.End_Poisition + addtask.Targe_ID + addtask.Targe_Position;
                    _listask.Add(addtask);
                }
            }
            


        }
        #endregion
        #region 下发第一组任务数据
        public Task_Data Tasktosend()
        {
            Task_Data tas = new Task_Data();

            tas=_listask[0];

   

            return tas;
        }
        #endregion
        public void Poisition_inter()
        {




        }
        #region 根据地址读取tag
        public List<ITag> Getctaglist(string Address,Storage type)
        { List<ITag> m = new List<ITag>();
            List<TagMetaData> mm = _list.FindAll(d => d.Address == Address);
            valueset vaa = new valueset();
            foreach (TagMetaData j in mm)
            {
               
                ITag tt = this[j.ID];
                m.Add(tt);
              

            }
            return m;
        }
        #endregion
        #region 根据注册小车号筛选tag
        public List<ITag> chosetaglist(List<ITag>tt)
        {
            List<ITag> mk = new List<ITag>();
   
            foreach (ITag j in tt)
            {
                foreach(var car in Carnumber)
                {
                    if(car.Value==j.Parent.ID)
                    {
                        mk.Add(j);
                        break;

                    }


                }

            }
            return mk;
        }
        #endregion


        #region 向XML文件中添加节点属性信息
        /// <summary>
        /// 添加节点属性信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="head"></param>
        public static void XMLAttributeAppend(XmlDocument rootxml, string name, string value, XmlElement head)
        {
            XmlAttribute att = rootxml.CreateAttribute(name);
            att.Value = value;
            head.Attributes.Append(att);
        }
        #endregion

        #region 根据节点及节点名称获取相应的Value
        /// <summary>
        /// 节点属性信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="head"></param>
        public static string XMLAttributeGetValue(XmlNode rootxml, string name)
        {
            string resvalue = "";
            if (rootxml != null && rootxml.Attributes != null && rootxml.Attributes[name] != null)
            {
                resvalue = rootxml.Attributes[name].Value;
            }
            return resvalue;
        }
        #endregion

      //  #region 装载XML文件：根据路径返回VariableAlarm_Modbus  LIST集合
        /// <summary>
        /// 装载XML文件：根据路径返回LIST集合
        /// </summary>
        /// <param name="xmlpath"></param>   //第四步  报警变量集合
       // CommMethods.VarAlarmS7List = CommMethods.LoadAlarmXML(FilePath + "VarAlarm.xml");     //定义文件路径
      //  private string FilePath = System.Windows.Forms.Application.StartupPath + "\\ConfigFile\\";
        /// <returns></returns>    
        //public static List<VarAlarm> LoadAlarmXML(string xmlAlarmpath)
        //{
        //    List<VarAlarm> VarAlarmList2 = null;

        //    if (!File.Exists(xmlAlarmpath))
        //    {1
        //        MessageBox.Show("IO配置报警变量的XML文件不存在！");
        //    }
        //    else
        //    {
        //        VarAlarmList2 = new List<VarAlarm>();
        //        XmlDocument xdoc = new XmlDocument();
        //        xdoc.Load(xmlAlarmpath);
        //        foreach (XmlNode noodroot in xdoc.ChildNodes)
        //        {
        //            if (noodroot.Name == "Root")
        //            {
        //                foreach (XmlNode noodtool in noodroot.ChildNodes)
        //                    if (noodtool.Name == "VariableAlarm")
        //                    {
        //                        VarAlarm objVar = new VarAlarm();
        //                        objVar.VarName = XMLAttributeGetValue(noodtool, "VarName");
        //                        objVar.Priority = Convert.ToInt32(XMLAttributeGetValue(noodtool, "Priority"));
        //                        objVar.AlarmType = XMLAttributeGetValue(noodtool, "AlarmType");
        //                        objVar.AlarmValue = float.Parse(XMLAttributeGetValue(noodtool, "AlarmValue"));
        //                        objVar.Note = XMLAttributeGetValue(noodtool, "Note");
        //                        VarAlarmList2.Add(objVar);
        //                    }
        //            }
        //        }
        //    }
        //    return VarAlarmList2;
        //}
        //#endregion





//               try {
//                int lenght = 0;
//        //保存分3步
//        //第一步：保存s7变量到XML文件
//        XmlDocument rootxml = new XmlDocument();
//        XmlElement rootnode1 = rootxml.CreateElement("Root");
//            foreach (Variable_S7 item in this.VarList)
//            { 
//                XmlElement xmle = rootxml.CreateElement("Variable");
//        CommMethods.XMLAttributeAppend(rootxml, "VarName", item.VarName, xmle);
//                    CommMethods.XMLAttributeAppend(rootxml, "Address", item.Address, xmle);
//                    CommMethods.XMLAttributeAppend(rootxml, "DataType", item.DataType, xmle);
//                    CommMethods.XMLAttributeAppend(rootxml, "StoreArea", item.StoreArea, xmle);
//                    CommMethods.XMLAttributeAppend(rootxml, "Note", item.Note, xmle);
//                    CommMethods.XMLAttributeAppend(rootxml, "IsFiling", item.IsFiling, xmle);
//                    CommMethods.XMLAttributeAppend(rootxml, "IsAlarm", item.IsAlarm, xmle);
//                    CommMethods.XMLAttributeAppend(rootxml, "IsReport", item.IsReport, xmle);
//                    CommMethods.XMLAttributeAppend(rootxml, "ReadWrite", item.ReadWrite, xmle);
//                    rootnode1.AppendChild(xmle);
//            }
//    rootxml.AppendChild(rootnode1);
//            if(File.Exists(VarPath))
//            {
//                File.Delete(VarPath);
//            }
//rootxml.Save(VarPath);









    }
}
