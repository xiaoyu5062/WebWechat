using System;
using System.Collections.Generic;

namespace Wechat.Web.admin
{
    public partial class Msg : System.Web.UI.Page
    {
        public string msg { get; set; }

        public List<Model.Message> list { get; set; }
        public Model.User self { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                self = admin.UserManager.UserSession[Context.User.Identity.Name];
            }
            catch
            {
                System.Web.Security.FormsAuthentication.RedirectToLoginPage();
                return;
            }
            msg = "ok";
            if (!IsPostBack)
            {
                GetMsg();
            }
            else
            {
                if (ViewState["list"] != null)
                {
                    list = (List<Model.Message>)ViewState["list"];
                }
            }
        }

        void GetMsg()
        {
            string sql = string.Empty;
            if (self.level == 2)//员工
            {
                sql = string.Format("select * from bk_msg where state=0 and uid={0}",
           self.parent_id);
            }
            else
            {
                sql = string.Format("select * from bk_msg where state=0 and uid={0}",
            self.id);
            }
            var dt = FXH.DbUtility.AosyMySql.ExecuteforDataSet(sql).Tables[0];
            list = ZFY.DataExtensions.ToList<Model.Message>(dt);
            ViewState["list"] = list;
        }

        protected void btn_sava_Click(object sender, EventArgs e)
        {
            var title = tb_title.Text;
            var content = tb_content.Text;

            string sql = string.Format("select id,exp_dt from bk_user where login='{0}'", Context.User.Identity.Name);
            var dt = FXH.DbUtility.AosyMySql.ExecuteforDataSet(sql).Tables[0];
            int id = int.Parse(dt.Rows[0]["id"].ToString());
            string exp_dt = dt.Rows[0]["exp_dt"].ToString();
            sql = "insert into bk_msg (uid,title,content) values (@uid,@title,@content);";
            bool r = FXH.DbUtility.AosyMySql.ExecuteforBool(sql, System.Data.CommandType.Text, new MySql.Data.MySqlClient.MySqlParameter[] {
                    new MySql.Data.MySqlClient.MySqlParameter ("@uid",id),
                    new MySql.Data.MySqlClient.MySqlParameter ("@title",title),
                new MySql.Data.MySqlClient.MySqlParameter ("@content",content)
                });
            if (r)
            {
                msg = "添加成功";
                tb_title.Text = "";
                tb_content.Text = "";
                GetMsg();
            }
            else
            {
                msg = "添加失败";
            }

        }
    }
}