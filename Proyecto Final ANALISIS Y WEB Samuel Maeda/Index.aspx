<%@ Page Title="Menú Principal" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="LuzDelSaber.Index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow p-5 text-center">
        <h1 class="mb-3">Bienvenido, 
            <asp:Label ID="lblUsuario" runat="server" Text=""></asp:Label>
        </h1>
        <h5 class="text-muted">
            Rol: <asp:Label ID="lblRol" runat="server" Text=""></asp:Label>
        </h5>

        <p class="mb-4">Selecciona una de las siguientes opciones:</p>

        <div class="row justify-content-center g-3">
            <asp:Panel ID="panelLibros" runat="server" CssClass="col-md-3 mb-3">
                <asp:Button ID="btnLibros" runat="server" Text="📚 Registro de Libros"
                    CssClass="btn btn-primary w-100" PostBackUrl="~/BookRegister.aspx" />
            </asp:Panel>

            <asp:Panel ID="panelCompras" runat="server" CssClass="col-md-3 mb-3">
                <asp:Button ID="btnCompras" runat="server" Text="🛒 Registrar Compra"
                    CssClass="btn btn-info w-100" PostBackUrl="~/Compra.aspx" />
            </asp:Panel>

            <asp:Panel ID="panelVentas" runat="server" CssClass="col-md-3 mb-3">
                <asp:Button ID="btnVentas" runat="server" Text="💸 Registrar Venta"
                    CssClass="btn btn-warning w-100" PostBackUrl="~/Venta.aspx" />
            </asp:Panel>

            <asp:Panel ID="panelReportes" runat="server" CssClass="col-md-3 mb-3">
                <asp:Button ID="btnReportes" runat="server" Text="📊 Reportes"
                    CssClass="btn btn-secondary w-100" PostBackUrl="~/Reportes.aspx" />
            </asp:Panel>

            <asp:Panel ID="panelInventario" runat="server" CssClass="col-md-3 mb-3">
                <asp:Button ID="btnInventario" runat="server" Text="📦 Inventario"
                    CssClass="btn btn-dark w-100" PostBackUrl="~/Inventario.aspx" />
            </asp:Panel>

            <asp:Panel ID="panelClientes" runat="server" CssClass="col-md-3 mb-3">
                <asp:Button ID="btnClientes" runat="server" Text="👥 Registrar Clientes"
                    CssClass="btn btn-success w-100" PostBackUrl="~/Clientes.aspx" />
            </asp:Panel>
        </div>

        <hr class="my-4" />
    </div>
</asp:Content>
