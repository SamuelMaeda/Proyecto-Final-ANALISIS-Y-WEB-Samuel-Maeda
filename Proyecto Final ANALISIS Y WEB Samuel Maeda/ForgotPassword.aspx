<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda.ForgotPassword" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Recuperar Contraseña</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.7/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="bg-light">
    <form id="form1" runat="server" class="container mt-5">
        <div class="card p-4 shadow">
            <h3 class="mb-3">Recuperar Contraseña</h3>
            <div class="mb-3">
                <asp:Label ID="lblCorreo" runat="server" Text="Correo registrado:" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="tbCorreo" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="mb-3">
                <asp:Button ID="btnEnviar" runat="server" Text="Enviar enlace" CssClass="btn btn-primary" OnClick="btnEnviar_Click" />
            </div>
            <asp:Label ID="lblMensaje" runat="server" CssClass="text-danger fw-bold"></asp:Label>
        </div>
    </form>
</body>
</html>
