using System;
using System.Web;
using System.Web.UI;

namespace Wechat.Web
{

    public partial class share : System.Web.UI.Page
    {

        #region Fileds
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool show { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        /// <value>The title.</value>
        public string title
        {
            get;
            set;
        }
        /// <summary>
        /// 活动内容
        /// </summary>
        /// <value>The content.</value>
        public string content
        {
            get;
            set;
        }
        #endregion
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                show = true;
                if (Request["i"] == null)
                {
                    show = false;
                }
               
               
                title = "cedcdcdcdc";
                content = "这里是活动内容";
                
            }
        }
    }
}
