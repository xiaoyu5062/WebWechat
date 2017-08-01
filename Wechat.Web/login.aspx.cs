using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Wechat.Web
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected void btn_login_Click(object sender, EventArgs e)
        {
            var username = txt_username.Text;
            var pwd = txt_pwd.Text;
            int r=int.Parse(  FXH.DbUtility.AosyMySql.ExecuteScalar(@"select count(1) from bk_user where login=@login and pwd=@pwd
", System.Data.CommandType.Text,new MySqlParameter[] {

            new MySqlParameter ("@login",username),
            new MySqlParameter ("@pwd",ZFY.FYCommon.GetMD5(pwd))
            }).ToString());
            if (r>0)
            {
                FormsAuthentication.SetAuthCookie(username, false);
                Response.Redirect("admin/index.aspx");
               // FormsAuthentication.RedirectFromLoginPage(username, false);
            }
            else
            {
                FormsAuthentication.RedirectToLoginPage();
            }
           
        }
    }
}