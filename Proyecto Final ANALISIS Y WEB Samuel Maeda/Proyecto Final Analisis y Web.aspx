<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Proyecto Final Analisis y Web.aspx.cs" Inherits="Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda.Proyecto_Final_Analisis_y_Web" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.7/dist/css/bootstrap.min.css" rel="stylesheet">
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.7/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://code.jquery.com/jquery-3.7.1.min.js" ></script>
    <link href="Recursos/CSS/Estilos.css" rel="stylesheet" />


    <title>Luz del Saber</title>
</head>
<body class="bg-light">

    <div class="wrapper">
        <div class="formcontent">

            <form id="formulario_login" runat="server">
                <div class="form-control">
                    <div class="col-md-6 text-center mb-5">
                        <asp:label class="h3" ID="lblBienvenida" runat="server" text="Bienvenido al Sistema de Luz del Saber"></asp:label>
                    </div>
                    <div>
                        <asp:Label ID="lblUsuario" runat="server" Text="Usuario: "></asp:Label>
                        <asp:TextBox ID="tbUsuario" CssClass="form-control" runat="server" placeholder="Nombre de Usuario"></asp:TextBox>
                    </div>
                    <div>
                        <asp:Label ID="lblPassword" runat="server" Text="Password"></asp:Label>
                        <asp:TextBox ID="tbPassword" CssClass="form-control" TextMode="Password" runat="server" placeholder="Password"></asp:TextBox>
                    </div>
                    <hr />
                    <div class="row">
                        <asp:label runat="server" ID="lblError"></asp:label>
                    </div>

                    <br />

                    <div class="row">
                        <asp:Button ID="BtnIngresar" CssClass="btn btn-primary btn-dark" runat="server" Text="Ingresar" OnClick="BtnIngresar_Click"/>
                    </div>

                    <div class="row mt-3">
    <asp:HyperLink ID="lnkForgot" runat="server" NavigateUrl="ForgotPassword.aspx">¿Olvidaste tu contraseña?</asp:HyperLink>
</div>


                </div>
            </form>
        </div>

    </div>

</body>
</html>
