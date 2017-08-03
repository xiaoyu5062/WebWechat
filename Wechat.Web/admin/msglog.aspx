<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="company.aspx.cs" Inherits="Wechat.Web.admin.msglog" %>

<!DOCTYPE html>
<html>

<head>

    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">


    <title>爆客系统</title>
    <link rel="shortcut icon" href="favicon.ico">
    <link href="css/bootstrap.min.css?v=3.3.5" rel="stylesheet">
    <link href="css/font-awesome.min.css?v=4.4.0" rel="stylesheet">
    <link href="css/plugins/footable/footable.core.css" rel="stylesheet">

    <link href="css/animate.min.css" rel="stylesheet">
    <link href="css/style.min.css?v=4.0.0" rel="stylesheet">
</head>

<body class="gray-bg">
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="tabs-container">
                <ul class="nav nav-tabs">
                    <li class="active">
                        <a data-toggle="tab" href="#tab-1" aria-expanded="true">发送记录</a>
                    </li>
                </ul>
                <div class="tab-content">
                    <div id="tab-1" class="tab-pane active">
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-sm-12">
                                    <div class="ibox float-e-margins">
                                        <div class="ibox-content">
                                            <input type="text" class="form-control input-sm m-b-xs" id="filter"
                                                placeholder="搜索表格...">

                                            <table class="footable table table-stripped" data-page-size="8" data-filter="#filter">
                                                <thead>
                                                    <tr>
                                                        <th>编号</th>
                                                        <th>消息标题</th>
                                                         <%if (user.level != 0 && user.level != 2)
                                                            { %>
                                                          <th>引导员</th>
                                                        <%} %>
                                                        <th>发送者</th>
                                                        <th>爆光量</th>
													    <th>发送时间</th>
                                                       
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <%
                                                        System.Text.StringBuilder sb = new StringBuilder();
                                                        foreach (var item in list)
                                                        {
                                                            sb.Append(" <tr class=\"gradeX\">");
                                                            sb.Append("<td>" + item.id + "</td>");
                                                            sb.Append("<td>" + item.title + "</td>");
                                                            if (user.level != 0 && user.level != 2)
                                                            {
                                                                sb.Append("<td class=\"center\">" + item.username + "</td>");
                                                            }
                                                            sb.Append(" <td>" + item.nickname + "</td>");
                                                            sb.Append(" <td class=\"center\">" + item.send_count + "</td>");
                                                            sb.Append(" <td class=\"center\">" + item.create_dt+ "</td>");
                                                        }
                                                    %>
                                                    <%=sb %>
                                                </tbody>
                                                <tfoot>
                                                    <tr>
                                                        <td colspan="5">
                                                            <ul class="pagination pull-right"></ul>
                                                        </td>
                                                    </tr>
                                                </tfoot>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                  
                </div>


            </div>

        </div>

    </div>
    <script src="js/jquery.min.js?v=2.1.4"></script>
    <script src="js/bootstrap.min.js?v=3.3.5"></script>
    <script src="js/plugins/footable/footable.all.min.js"></script>
    <script src="js/content.min.js?v=1.0.0"></script>
    <script>
        $(document).ready(function () {

            $(".footable").footable();
            var msg = "<%=msg%>";
            if (msg != "ok") {
                alert(msg);
            }
        });
    </script>
</body>

</html>
