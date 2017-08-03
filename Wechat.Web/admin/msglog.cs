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

        public Model.User user { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                user = UserManager.UserSession[Context.User.Identity.Name];
            }
            catch
            {
                System.Web.Security.FormsAuthentication.RedirectToLoginPage();
                return;
            }
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

        void GetUsers()
        {
            string sql = string.Empty;
            if (user.level == 2||user.level==0)
            {
                sql = string.Format("select l.id,m.title ,l.wx_nickname nickname,l.wx_send_count send_count ,l.create_dt from bk_msg_log l left join bk_msg m on m.id=l.msg_id WHERE l.uid={0} order by l.id desc",user.id);
            }
            else {
                
                // sql = string.Format("select l.id,m.title,l.wx_nickname nickname,l.wx_send_count send_count ,l.create_dt from bk_msg_log l left join bk_msg m on m.id=l.msg_id WHERE l.uid={0}) or l.uid_parent={1} order by l.id desc", user.id,user.parent_id);
                sql = string.Format(@"select l.id,m.title,l.wx_nickname nickname,l.wx_send_count send_count ,l.create_dt,u.username from bk_msg_log l 
left join bk_msg m on m.id = l.msg_id
left join bk_user u on u.id = l.uid
WHERE l.uid = {0} or l.uid_parent = {1} order by l.id desc", user.id, user.parent_id);
            }
            var dt = FXH.DbUtility.AosyMySql.ExecuteforDataSet(sql).Tables[0];
            list = ZFY.DataExtensions.ToList<Model.MsgLog>(dt);
            ViewState["list"] = list;
        }
    }
}