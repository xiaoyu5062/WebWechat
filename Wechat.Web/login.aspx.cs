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
          var dt=  FXH.DbUtility.AosyMySql.ExecuteforDataSet(@"select * from bk_user where login=@login and pwd=@pwd
", System.Data.CommandType.Text,new MySqlParameter[] {

            new MySqlParameter ("@login",username),
            new MySqlParameter ("@pwd",ZFY.FYCommon.GetMD5(pwd))
            }).Tables[0];
            if (dt.Rows.Count==1)
            {
                Wechat.Web.admin.Model.User user = ZFY.DataExtensions.ToModel<admin.Model.User>(dt.Rows[0]);
                if (admin.UserManager.UserSession.ContainsKey(username))
                {
                    admin.UserManager.UserSession[username] = user;
                }
                else
                {
                    admin.UserManager.UserSession.Add(username, user);
                }
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