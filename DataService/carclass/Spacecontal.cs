using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataService.carclass
{
    class Spacecontal
    {
        public short SpaceID { get; set; }                     //调度id
        public Int32 Start_Poisition { get; set; }             //开始位置
        public Int32      End_Poisition { get; set; }          //结束位置
        public Int32     Dec_Distance { get; set; }             //减速距离
        public Int32    Stop_Distance { get; set; }            //停止距离
    }
    class Navi_Data
    {
        public   short SubMapID { get; set; }           //导航ID
        public short Map_Type { get; set; }            //导航类型
        public Int32 Start_Position { get; set; }       //开始位置
        public Int32 End_Poisition { get; set; }         //结束位置
        public float Speed { get; set; }               //速度

    }
   public class Task_Data
    {
        public short TaskID { get; set; }                   //任务ID
        public short Task_Type { get; set; }                  //任务类型
        public short Targe_ID { get; set; }                 //装菜点ID
        public short Station_ID { get; set; }                // 站ID
        public short End_ID { get; set; }                    // 回盘放置点ID
        public short Priority { get; set; }                 //任务优先级
        public Int32 Targe_Position { get; set; }           //装菜点位置
        public Int32 Station_Position { get; set; }           //站位置
        public Int32 End_Poisition { get; set; }               //回盘点位置

        public Int32 sumcheck { get; set; }                   //和校验值



    }


}
