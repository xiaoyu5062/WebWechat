using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Wechat.Web.admin
{
    public partial class company : System.Web.UI.Page
    {
        public string msg { get; set; }

        public List<Model.User> list { get; set; }
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
                    list = (List<Model.User>)ViewState["list"];
                }
            }
        }

        void GetUsers() {
            string sql = string.Format("select * from bk_user where parent_id=(select id from bk_user where login='{0}')", Context.User.Identity.Name);
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
                pwd = ZFY.FYCommon.GetMD5(pwd);
                string sql = string.Format("select id from bk_user where login='{0}'", Context.User.Identity.Name);
                var id = int.Parse(FXH.DbUtility.AosyMySql.ExecuteScalar(sql).ToString());
                sql = "insert into bk_user (login,pwd,username,level,parent_id,exp_dt) values (@login,@pwd,@username,1,@parent_id,@exp_dt)";
                bool r = FXH.DbUtility.AosyMySql.ExecuteforBool(sql, System.Data.CommandType.Text, new MySql.Data.MySqlClient.MySqlParameter[] {
                    new MySql.Data.MySqlClient.MySqlParameter ("@login",login),
                    new MySql.Data.MySqlClient.MySqlParameter ("@pwd",pwd),
                    new MySql.Data.MySqlClient.MySqlParameter ("@username",company),
                    new MySql.Data.MySqlClient.MySqlParameter ("@parent_id",id),
                    new MySql.Data.MySqlClient.MySqlParameter ("@exp_dt",DateTime.Now.AddYears(1).ToShortDateString())
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