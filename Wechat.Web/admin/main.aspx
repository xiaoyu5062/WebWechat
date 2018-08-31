<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="main.aspx.cs" Inherits="Wechat.Web.admin.main" %>

<!DOCTYPE html>
<html>

<head>

    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    

    <title>爆客系统</title>

    <meta name="keywords" content="H爆客系统">
    <meta name="description" content="爆客系统">


    <link rel="shortcut icon" href="favicon.ico"> <link href="css/bootstrap.min.css?v=3.3.5" rel="stylesheet">
    <link href="css/font-awesome.min.css?v=4.4.0" rel="stylesheet">
    <link href="css/plugins/iCheck/custom.css" rel="stylesheet">
    <link href="css/animate.min.css" rel="stylesheet">
    <link href="css/style.min.css?v=4.0.0" rel="stylesheet"><base target="_blank">

</head>

<body class="gray-bg">
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-sm-3">
                <div class="widget style1 navy-bg">
                    <div class="row">
                        <div class="col-xs-4">
                            <i class="glyphicon glyphicon-envelope fa-5x"></i>
                        </div>
                        <div class="col-xs-8 text-right">
                            <h2 class="font-bold"><%=count_msg %></h2>
                            <span> 条消息 </span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-sm-3">
                <div class="widget style1 lazur-bg">
                    <div class="row">
                        <div class="col-xs-4">
                            <i class="glyphicon glyphicon-send fa-5x"></i>
                        </div>
                        <div class="col-xs-8 text-right">
                            <h2 class="font-bold"><%=count_send %></h2>
                            <span> 次发送 </span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-sm-3">
                <div class="widget style1 yellow-bg">
                    <div class="row">
                        <div class="col-xs-4">
                            <i class="glyphicon glyphicon-stats fa-5x"></i>
                        </div>
                        <div class="col-xs-8 text-right">
                            <h2 class="font-bold"><%=count_look %></h2>
                            <span> 次曝光 </span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
     
       
    </div>
    <script src="js/jquery.min.js?v=2.1.4"></script>
    <script src="js/jquery-ui-1.10.4.min.js"></script>
    <script src="js/bootstrap.min.js?v=3.3.5"></script>
    <script src="js/content.min.js?v=1.0.0"></script>
    <!--<script src="js/plugins/iCheck/icheck.min.js"></script>-->
    <script src="js/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js"></script>
    <script src="js/plugins/jvectormap/jquery-jvectormap-world-mill-en.js"></script>
    <script src="js/plugins/flot/jquery.flot.js"></script>
    <script src="js/plugins/flot/jquery.flot.tooltip.min.js"></script>
    <script src="js/plugins/flot/jquery.flot.resize.js"></script>
   
</body>

</html>
