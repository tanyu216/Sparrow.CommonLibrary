using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.CommonLibrary.Test.Query
{
    public class HotelSimpleInfoModel
    {
        /// <summary>
        /// 是否显示微官网
        /// </summary>
        public virtual int HasWeixinSite { get; set; }
        /// <summary>
        /// 酒店编号
        /// </summary>
        public virtual int Hid { get; set; }
        /// <summary>
        /// 酒店中文名称
        /// </summary>
        public virtual string HotelName { get; set; }
        /// <summary>
        /// 酒店星级 5二星及以下，10三星/舒适，15四星/高档，20五星级/豪华
        /// </summary>
        public virtual string HotelStar { get; set; }
        /// <summary>
        /// 主图Url
        /// </summary>
        public virtual string DefaultImgUrl { get; set; }
        /// <summary>
        /// 酒店简短一句话
        /// </summary>
        public virtual string HotelIntroduction { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public virtual string Lat { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public virtual string Lng { get; set; }
        /// <summary>
        /// 酒店最低房价
        /// </summary>
        public virtual decimal LowFee { get; set; }
        /// <summary>
        /// 酒店最低房价原价
        /// </summary>
        public virtual decimal ListPrice { get; set; }
        /// <summary>
        /// 酒店最低房价返现
        /// </summary>
        public virtual decimal ReturnAmount { get; set; }
        /// <summary>
        /// 城市ID
        /// </summary>
        public virtual int CityID { get; set; }
        /// <summary>
        /// 是否有wifi
        /// </summary>
        public virtual bool IsWifi { get; set; }
        /// <summary>
        /// 是否有carstop
        /// </summary>
        public virtual bool IsCarstop { get; set; }
        /// <summary>
        /// 是否有swim
        /// </summary>
        public virtual bool IsSwim { get; set; }
        /// <summary>
        /// 是否有cook
        /// </summary>
        public virtual bool IsCook { get; set; }

        /// <summary>
        /// 是否有免费停车场
        /// </summary>
        public virtual bool IsFreePark { get; set; }
        /// <summary>
        /// 是否有收费停车场
        /// </summary>
        public virtual bool IsPark { get; set; }
        /// <summary>
        /// 是否有fitness
        /// </summary>
        public virtual bool IsFitness { get; set; }
        /// <summary>
        /// 是否有wakeup
        /// </summary>
        public virtual bool IsWakeup { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public virtual string CityName { get; set; }
        /// <summary>
        /// 酒店地址
        /// </summary>
        public virtual string HotelAddress { get; set; }

        /// <summary>
        /// 酒店周边商圈
        /// </summary>
        public virtual string RegionalInfo { get; set; }
        /// <summary>
        /// 酒店周边商圈
        /// </summary>
        public virtual string[] Regional { get; set; }
        /// <summary>
        /// 配套设施
        /// </summary>
        public virtual string HotelFacilitie { get; set; }
        /// <summary>
        /// 距离信息
        /// </summary>  
        public virtual string Distance { get; set; }
        /// <summary>
        /// 酒店评论信息
        /// </summary>
        public virtual HotelCommentModel HotelComment { get; set; }
    }

    public class HotelCommentModel
    {
        /// <summary>
        /// 好评分
        /// </summary>
        public virtual string Desc { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        public virtual int TotalCount { get; set; }
    }
}
