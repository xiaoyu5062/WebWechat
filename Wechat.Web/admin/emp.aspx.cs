using System;
using System.Collections.Generic;

namespace Wechat.Web.admin
{
    public partial class emp : System.Web.UI.Page
    {
        public string msg { get; set; }

        public List<Model.User> list { get; set; }

        public Model.User self { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                self = UserManager.UserSession[Context.User.Identity.Name];
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
                    list = (List<Model.User>)ViewState["list"];
                }
            }
        }

        void GetUsers()
        {
            string sql = string.Empty;
            if (self.level == -1)
            {
                sql = string.Format("select * from bk_user ");
            }
            else if (self.level == 1 || self.level == 3)
            {
                sql = string.Format("select * from bk_user where parent_id={0}", self.id);
            }
            var dt = FXH.DbUtility.AosyMySql.ExecuteforDataSet(sql).Tables[0];
            list = ZFY.DataExtensions.ToList<Model.User>(dt);
            ViewState["list"] = list;
        }

        protected void btn_sava_Click(object sender, EventArgs e)
        {

            var company = tb_company.Text;
            var login = tb_login.Text;
            var pwd = tb_pwd1.Text;
            var pwd2 = tb_pwd2.Text;
            if (pwd != pwd2)
            {
                msg = "两次密码不一致";
            }
            else
            {
                string checkuser = "select count(1) from bk_user where login='" + login + "'";
                if (int.Parse(FXH.DbUtility.AosyMySql.ExecuteScalar(checkuser).ToString()) > 0)
                {
                    msg = "账号已被注册，请使用其它账号";
                    return;
                }

                pwd = ZFY.FYCommon.GetMD5(pwd);
                string sql = string.Format("select id,exp_dt from bk_user where login='{0}'", Context.User.Identity.Name);
                var dt = FXH.DbUtility.AosyMySql.ExecuteforDataSet(sql).Tables[0];
                int id = int.Parse(dt.Rows[0]["id"].ToString());
                string exp_dt = string.Empty;
                if (self.level == 1 || self.level == 3)
                {
                    exp_dt = dt.Rows[0]["exp_dt"].ToString();
                }
                else if (self.level == -1)
                {
                    exp_dt = DateTime.Now.AddYears(1).ToString();
                }
                sql = string.Format("insert into bk_user (login,pwd,username,level,parent_id,exp_dt) values (@login,@pwd,@username,{0},@parent_id,@exp_dt)", self.level == -1 ? 0 : 2);
                bool r = FXH.DbUtility.AosyMySql.ExecuteforBool(sql, System.Data.CommandType.Text, new MySql.Data.MySqlClient.MySqlParameter[] {
                    new MySql.Data.MySqlClient.MySqlParameter ("@login",login),
                    new MySql.Data.MySqlClient.MySqlParameter ("@pwd",pwd),
                    new MySql.Data.MySqlClient.MySqlParameter ("@username",company),
                    new MySql.Data.MySqlClient.MySqlParameter ("@parent_id",id),
                    new MySql.Data.MySqlClient.MySqlParameter ("@exp_dt",exp_dt)
                });
                if (r)
                {
                    msg = "添加成功";
                    tb_company.Text = "";
                    tb_login.Text = "";
                    tb_pwd1.Text = "";
                    tb_pwd2.Text = "";
                    GetUsers();
                }
                else
                {
                    msg = "账号已存在";
                }
            }
        }
    }
}