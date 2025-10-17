<%@ Page Title="Menú Principal" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="LuzDelSaber.Index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow p-5 text-center">
        <h1 class="mb-3">Bienvenido, 
            <asp:Label ID="lblUsuario" runat="server" Text=""></asp:Label>
        </h1>

        <p class="mb-4">Selecciona una de las siguientes opciones:</p>

        <div class="row justify-content-center g-3">
            <div class="col-md-3">
                <asp:Button ID="btnLibros" runat="server" Text="📚 Registro de Libros"
                    CssClass="btn btn-primary w-100"
                    PostBackUrl="~/BookRegister.aspx" />
            </div>
            <div class="col-md-3">
                <asp:Button ID="btnCompras" runat="server" Text="🛒 Registrar Compra"
                    CssClass="btn btn-info w-100"
                    PostBackUrl="~/Compra.aspx" />
            </div>
            <div class="col-md-3">
                <asp:Button ID="btnVentas" runat="server" Text="💸 Registrar Venta"
                    CssClass="btn btn-warning w-100"
                    PostBackUrl="~/Venta.aspx" />
            </div>
            <div class="col-md-3">
                <asp:Button ID="btnReportes" runat="server" Text="📊 Reportes"
                    CssClass="btn btn-secondary w-100"
                    PostBackUrl="~/Reportes.aspx" />
            </div>
        </div>

        <div class="row justify-content-center g-3 mt-3">
            <div class="col-md-3">
                <asp:Button ID="btnInventario" runat="server" Text="📦 Inventario"
                    CssClass="btn btn-dark w-100"
                    PostBackUrl="~/Inventario.aspx" />
            </div>
        </div>

        <hr class="my-4" />
    </div>
</asp:Content>
