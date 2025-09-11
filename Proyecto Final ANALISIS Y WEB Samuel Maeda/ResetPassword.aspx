<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda.ResetPassword" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Restablecer Contraseña</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.7/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="bg-light">
    <form id="form1" runat="server" class="container mt-5">
        <div class="card p-4 shadow">
            <h3 class="mb-3">Restablecer Contraseña</h3>
            <div class="mb-3">
                <asp:Label ID="lblNueva" runat="server" Text="Nueva contraseña:" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="tbNueva" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="mb-3">
                <asp:Button ID="btnReset" runat="server" Text="Cambiar contraseña" CssClass="btn btn-success" OnClick="btnReset_Click" />
            </div>
            <asp:Label ID="lblMensaje" runat="server" CssClass="fw-bold"></asp:Label>
        </div>
    </form>
</body>
</html>
