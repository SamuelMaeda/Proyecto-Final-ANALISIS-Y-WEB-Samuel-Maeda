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
                    
    <!-- Logo -->
    <asp:Image ID="Image1" runat="server" 
               ImageUrl="~/Images/LuzDelSaberLOGO.jpg"
               AlternateText="LuzDelSaber" Style="height:80px;" CssClass="mb-3 mx-auto d-block" />

    <h3 class="mb-4">Bienvenido al sistema de Luz del Saber</h3>

    <!-- Usuario -->
    <div class="mb-3 text-start">
        <asp:Label ID="lblUsuario" runat="server" Text="Usuario:" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="tbUsuario" runat="server" CssClass="form-control"></asp:TextBox>
    </div>

    <!-- Contraseña -->
    <div class="mb-3 text-start">
        <asp:Label ID="lblPassword" runat="server" Text="Contraseña:" CssClass="form-label"></asp:Label>
        <asp:TextBox ID="tbPassword" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
    </div>

    <!-- Botón ingresar -->
    <asp:Button ID="BtnIngresar" runat="server" Text="Ingresar"
                CssClass="btn btn-primary w-100" OnClick="BtnIngresar_Click" />

    <!-- Mensaje de error -->
    <asp:Label ID="lblError" runat="server" CssClass="text-danger d-block mt-3"></asp:Label>
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
