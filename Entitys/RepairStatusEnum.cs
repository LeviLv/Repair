using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Repair.Entitys
{
    public enum RepairStatusEnum
    {
        /// <summary>
        /// 派单
        /// </summary>
        [Display(Name = "派单")]
        Init = 1,

        /// <summary>
        /// 后台接单
        /// </summary>
        [Display(Name = "接单")]
        Sure = 2,

        /// <summary>
        /// 处理中
        /// </summary>
        [Display(Name = "处理中")]
        Doing = 3,

        /// <summary>
        /// 已完成、待评价
        /// </summary>
        [Display(Name = "已完成、待评价")]
        Down = 4,

        /// <summary>
        /// 完成
        /// </summary>
        [Display(Name = "已评价")]
        Success = 5
    }


}
