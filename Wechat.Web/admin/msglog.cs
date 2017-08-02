using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Wechat.Web.admin
{
    public partial class msglog : System.Web.UI.Page
    {
        public string msg { get; set; }

        public List<Model.MsgLog> list { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            msg = "ok";
            if (!IsPostBack)
            {
                GetUsers();
            }
            else
            {
                if (ViewState["list"] != null)
                {
                    list = (List<Model.MsgLog>)ViewState["list"];
                }
            }
        }

        void GetUsers() {
            string sql = string.Format("select l.id,m.title ,l.wx_nickname nickname,l.wx_send_count send_count ,l.create_dt from bk_msg_log l left join bk_msg m on m.id=l.msg_id\nWHERE l.uid=(select id from bk_user where login='{0}')", Context.User.Identity.Name);
            var dt = FXH.DbUtility.AosyMySql.ExecuteforDataSet(sql).Tables[0];
            list = ZFY.DataExtensions.ToList<Model.MsgLog>(dt);
            ViewState["list"] = list;
        }
    }
}