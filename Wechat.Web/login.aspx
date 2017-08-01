<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="Wechat.Web.login" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>欢迎使用爆客系统</title>
    <link rel="shortcut icon" href="favicon.ico">
    <link href="admin/css/bootstrap.min.css?v=3.3.5" rel="stylesheet">
    <link href="admin/css/font-awesome.min.css?v=4.4.0" rel="stylesheet">

    <link href="admin/css/animate.min.css" rel="stylesheet">
    <link href="admin/css/style.min.css?v=4.0.0" rel="stylesheet">
    
    <!--[if lt IE 8]>
    <meta http-equiv="refresh" content="0;ie.html" />
    <![endif]-->
    <script>if (window.top !== window.self) { window.top.location = window.location; }</script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="middle-box text-center loginscreen  animated fadeInDown">
            <div>
                <div>
                    <h1 class="logo-name">B</h1>
                </div>
                <h3>欢迎使用 爆客系统</h3>
                    <div class="form-group">
                          <asp:TextBox ID="txt_username" TextMode="SingleLine" runat="server" class="form-control" placeholder="用户名" required=""></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:TextBox ID="txt_pwd" TextMode="Password" runat="server" class="form-control" placeholder="密码" required=""></asp:TextBox>
                    </div>
                    <asp:Button ID="btn_login" class="btn btn-red block full-width m-b" runat="server" OnClick="btn_login_Click" Text="登 录" />
            </div>
        </div>
        <script src="admin/js/jquery.min.js?v=2.1.4"></script>
        <script src="admin/js/bootstrap.min.js?v=3.3.5"></script>
    </form>
</body>
</html>
