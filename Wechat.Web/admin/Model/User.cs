/********************************************************************************
** auth： 张逢阳
** date： 2017-8-1 11:30:34
** desc： 尚未编写描述
** Ver.:  V1.0.0
** Email: xiaoyu5062@qq.com
** QQ:    170515071
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wechat.Web.admin.Model
{
    [Serializable]
    public class User
    {
        public int id { get; set; }
        public string login { get; set; }
        public string username { get; set; }
        /// <summary>
        /// -1admin 0普通用户 1企业主 2员工 3代理
        /// </summary>
        public int level { get; set; }
        public int parent_id { get; set; }
        public string create_dt { get; set; }
        public string modify_dt { get; set; }
        public string exp_dt { get; set; }

        public int scan_allow { get; set; }

        public int scan_used { get; set; }
        public int scan_user { get; set; }
    }

    [Serializable]
    public class Message{

        public int id
        {
            get;
            set;
        }
        public int uid
        {
            get;
            set;
        }
        public string title
        {
            get;
            set;
        }
        public string    content
        {
            get;
            set;
        }

        public string create_dt
        {
            get;
            set;
        }
        public int state
        {
            get;
            set;
        }
    }
}