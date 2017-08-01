using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Wechat.Web.admin
{
    public partial class index : System.Web.UI.Page
    {
        public Model.User user { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["a"] != null)
            {
                string action = Request["a"];
                if (action == "exit")
                {
                    btn_exit_Click(null,null);
                }
                return;
            }

            if (!IsPostBack)
            {
                var login = Context.User.Identity.Name;
                string sql = "select * from bk_user where login='" + login + "'";
                var row = FXH.DbUtility.AosyMySql.ExecuteforDataSet(sql).Tables[0].Rows[0];
                user = ZFY.DataExtensions.ToModel<Model.User>(row);
                ViewState["user"] = user;
            }
            else
            {
                if (ViewState["user"] != null)
                {
                    user = (Model.User)ViewState["user"];
                }
            }
        }

        protected void btn_exit_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            ViewState.Clear();
            Response.Redirect("~/login.aspx");
           // FormsAuthentication.RedirectToLoginPage();
        }
    }
}