<%@ Page Language="C#" CodeFile="default.aspx.cs" Inherits="IisDefault" %>
<html>
<head>
<title></title>
<meta name="GENERATOR" content="The Mighty Hand of Bob">
<style>
.panel-title>.badge { float: right; }
.panel-title-btn-open { color: white; }
.form-control[disabled], .form-control[readonly], fieldset[disabled] .form-control { background-color: inherit !important; }
</style>
<!-- jQuery -->
<script src="https://code.jquery.com/jquery-2.2.4.min.js" integrity="sha256-BbhdlvQf/xTY9gja0Dq3HiwQF8LaCRTXxZKRutelT44=" crossorigin="anonymous"></script>
<!-- Bootstrap -->
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css" integrity="sha384-1q8mTJOASx8j1Au+a5WDVnPi2lkFfwwEAa8hDDdjZlpLegxhjVME1fgjWPGmkzs7" crossorigin="anonymous">
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap-theme.min.css" integrity="sha384-fLW2N01lMqjakBkx3l/M9EahuwpSfeNvV63J5ezn3uZzapT0u7EYsXMjQV+0En5r" crossorigin="anonymous">
<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js" integrity="sha384-0mSbJDEHialfmuBBQP6A4Qrprq5OVfW37PRR3j5ELqxss1yVqOtnepnHVP9aJ7xS" crossorigin="anonymous"></script>
<!-- Font-Awesome -->
<script src="https://use.fontawesome.com/ccc7183fac.js"></script>
</head>
<body>

<h1 align="center"><%=ServerName%><br></h1>

<div class="container">
    <div class="row">
        <div class='col-sm-4'>
            <h3>Dev</h3>
            <%=RenderedDevDirectories%>
        </div>
        <div class='col-sm-4'>
            <h3>Stable</h3>
            <%=RenderedStableDirectories%>
        </div>
        <div class='col-sm-4'>
            <h3>Old</h3>
            <%=RenderedOldDirectories%>
        </div>
    </div>
<%--    <div class="row">
        <div class='col-sm-12'>
            <h3>Unknown</h3>
            <div class="row">
                <%=RenderedUnknownDirectories%>
            </div>
        </div>
    </div>--%>
    <div class="row">
        <div class='col-sm-12'>
            <h3>Projects</h3>
            <div class="row">
                <%=RenderedProjectDirectories%>
            </div>
        </div>
    </div>
</div>

</body>
</html>