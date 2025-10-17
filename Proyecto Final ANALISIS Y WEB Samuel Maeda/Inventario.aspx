<%@ Page Title="Inventario" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Inventario.aspx.cs" Inherits="LuzDelSaber.Inventario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-4">
        <h2 class="text-center mb-4">📦 Inventario de Libros</h2>

        <!-- Sección para búsqueda -->
        <div class="row mb-3">
            <div class="col-md-8">
                <asp:Label ID="lblBuscar" runat="server" Text="Buscar:" CssClass="form-label"></asp:Label>
                <div class="input-group">
                    <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" placeholder="Buscar por nombre, autor, editorial o categoría..."></asp:TextBox>
                    <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-primary" OnClick="btnBuscar_Click" />
                    <asp:Button ID="btnReiniciar" runat="server" Text="Limpiar" CssClass="btn btn-secondary" OnClick="btnReiniciar_Click" />
                </div>
            </div>
        </div>

        <div class="row mb-3">
            <div class="col-md-4">
                <asp:Label ID="lblOrdenarPor" runat="server" Text="Ordenar por:" CssClass="form-label"></asp:Label>
                <asp:DropDownList ID="ddlOrdenarPor" runat="server" CssClass="form-select">
                    <asp:ListItem Text="ID" Value="LibroId" />
                    <asp:ListItem Text="Nombre" Value="Titulo" />
                    <asp:ListItem Text="Autor" Value="Autor" />
                    <asp:ListItem Text="Editorial" Value="Editorial" />
                    <asp:ListItem Text="Categoría" Value="Categoria" />
                    <asp:ListItem Text="Precio" Value="PrecioBase" />
                    <asp:ListItem Text="Stock" Value="StockUnidades" />
                </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <asp:Label ID="lblOrden" runat="server" Text="Orden:" CssClass="form-label"></asp:Label>
                <asp:DropDownList ID="ddlOrden" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Ascendente" Value="ASC" Selected="True" />
                    <asp:ListItem Text="Descendente" Value="DESC" />
                </asp:DropDownList>
            </div>
            <div class="col-md-4 d-flex align-items-end">
                <asp:Button ID="btnOrdenar" runat="server" Text="Aplicar Orden" CssClass="btn btn-primary w-100" OnClick="btnBuscar_Click" />
            </div>
        </div>

        <asp:GridView ID="gvInventario" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover"
            AllowPaging="True" PageSize="10" OnPageIndexChanging="gvInventario_PageIndexChanging">
            <Columns>
                <asp:BoundField DataField="LibroId" HeaderText="ID" />
                <asp:BoundField DataField="Titulo" HeaderText="Nombre" />
                <asp:BoundField DataField="Autor" HeaderText="Autor" />
                <asp:BoundField DataField="Editorial" HeaderText="Editorial" />
                <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                <asp:BoundField DataField="PrecioBase" HeaderText="Precio" DataFormatString="Q{0:N2}" HtmlEncode="false" />
                <asp:BoundField DataField="StockUnidades" HeaderText="Stock" />
                <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
            </Columns>
            <PagerStyle CssClass="gridview-pager" />
        </asp:GridView>
    </div>
</asp:Content>