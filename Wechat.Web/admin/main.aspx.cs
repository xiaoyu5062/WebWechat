using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Wechat.Web.admin
{
    public partial class main : System.Web.UI.Page
    {
        public int count_msg { get; set; }
        public int count_send { get; set; }
        public int count_look { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Model.User self = UserManager.UserSession[Context.User.Identity.Name];
                string sql = string.Empty;
                if (self.level == 0)
                {
                    sql = string.Format("select (select count(1) from bk_msg WHERE uid={0}) msg_count, count(1) send_count,SUM(wx_send_count) look from bk_msg_log WHERE uid={0}", self.id);
                }
                else if (self.level == 2)
                {
                    sql = string.Format("select (select count(1) from bk_msg WHERE uid={0}) msg_count, count(1) send_count,SUM(wx_send_count) look from bk_msg_log WHERE uid in (select id from bk_user where uid={1} or parent_id={2})", self.parent_id, self.id, self.parent_id);
                }
                else if (self.level == 1 || self.level == 3)
                {
                    sql = string.Format("select (select count(1) from bk_msg WHERE uid={0}) msg_count, count(1) send_count,SUM(wx_send_count) look from bk_msg_log WHERE uid in (select id from bk_user where uid={1} or parent_id={1})", self.parent_id, self.id);
                }
                else
                {
                    sql = string.Format("select (select count(1) from bk_msg ) msg_count, count(1) send_count,SUM(wx_send_count) look from bk_msg_log ");
                }
                var dt = FXH.DbUtility.AosyMySql.ExecuteforDataSet(sql).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    var dr = dt.Rows[0];
                    count_msg = int.Parse(dr[0].ToString());
                    count_send = int.Parse(dr[1].ToString());
                    if (dr[2].ToString() != "")
                    {
                        count_look = int.Parse(dr[2].ToString());
                    }
                    else
                    {
                        count_look = 0;
                    }
                }
            }
            catch
            {
                System.Web.Security.FormsAuthentication.RedirectToLoginPage();
            }

        }
    }
}