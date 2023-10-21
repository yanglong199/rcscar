using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataService.carclass
{
    class station
    {
        public short stationid { get; set; }              //站id
        public short stationtype { get; set; }             //站类型
        public Int32 stationpoisition { get; set; }        //站位置
        public Int32 downheight { get; set; }             //下降高度
        public Int32 errorvalue { get; set; }              //定位误差

    }
}
