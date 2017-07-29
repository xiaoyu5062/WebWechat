<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Wechat.Web.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script src="http://resource.ledianduo.com/js/jquery.min.js" type="text/javascript"></script>
</head>
<body>
    <div>
        <img id="QRCode" />
        <input type="button" id="btn_getQRCode" onclick="getQRCode();" value="生成二维码" />
    </div>
    <script>
        function getQRCode() {
            $.post("api.asmx/GetQRCode", {}, function (data) {
                if (data.code == 200) {
                    $('#QRCode').attr('src', data.data);
			        console.log(data.data);
                }
            });
        }
    </script>
</body>
</html>
