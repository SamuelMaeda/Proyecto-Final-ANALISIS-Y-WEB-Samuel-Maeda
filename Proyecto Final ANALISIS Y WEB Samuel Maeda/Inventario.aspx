<%@ Page Title="Inventario" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Inventario.aspx.cs" Inherits="LuzDelSaber.Inventario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card shadow p-4">
        <h2 class="text-center mb-4">📦 Inventario de Libros</h2>

        <div class="d-flex justify-content-between mb-3">

            <div class="d-flex gap-2">
                <asp:Button ID="btnVerBaja" runat="server" Text="📕 Ver libros dados de baja"
                    CssClass="btn btn-outline-danger" OnClick="btnVerBaja_Click" />

                <!-- 🔹 Nuevo botón para ajustes de inventario -->
                <asp:Button ID="btnAjustesInventario" runat="server"
                    Text="⚙️ Ajustes de inventario"
                    CssClass="btn btn-outline-primary"
                    OnClick="btnAjustesInventario_Click" />
            </div>

            <div class="text-end">
                <label class="me-2 fw-bold">Ordenar por:</label>
                <asp:DropDownList ID="ddlOrdenarPor" runat="server"
                    CssClass="form-select d-inline-block w-auto me-2"
                    AutoPostBack="true" OnSelectedIndexChanged="ddlOrdenarPor_SelectedIndexChanged">
                    <asp:ListItem Value="L.LibroId">ID</asp:ListItem>
                    <asp:ListItem Value="L.Titulo">Título</asp:ListItem>
                    <asp:ListItem Value="E.Nombre">Editorial</asp:ListItem>
                    <asp:ListItem Value="C.Nombre">Categoría</asp:ListItem>
                    <asp:ListItem Value="PrecioCompra">Precio de compra</asp:ListItem>
                    <asp:ListItem Value="PrecioUnitario">Precio de venta</asp:ListItem>
                    <asp:ListItem Value="L.StockUnidades">Stock</asp:ListItem>
                </asp:DropDownList>

                <label class="me-2 fw-bold">Orden:</label>
                <asp:DropDownList ID="ddlOrden" runat="server"
                    CssClass="form-select d-inline-block w-auto"
                    AutoPostBack="true" OnSelectedIndexChanged="ddlOrden_SelectedIndexChanged">
                    <asp:ListItem Value="ASC">Ascendente</asp:ListItem>
                    <asp:ListItem Value="DESC">Descendente</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>

        <div class="row align-items-center mb-3">
            <div class="col-md-8 d-flex">
                <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control me-2" placeholder="Buscar libro, editorial o categoría..." />
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-primary me-2" OnClick="btnBuscar_Click" />
                <asp:Button ID="btnReiniciar" runat="server" Text="Limpiar" CssClass="btn btn-secondary" OnClick="btnReiniciar_Click" />
            </div>
        </div>

        <asp:GridView ID="gvLibros" runat="server"
            CssClass="table table-bordered table-hover text-center align-middle"
            AutoGenerateColumns="False" DataKeyNames="LibroId"
            AllowPaging="True" PageSize="10"
            OnPageIndexChanging="gvLibros_PageIndexChanging"
            OnRowDataBound="gvLibros_RowDataBound"
            OnRowCommand="gvLibros_RowCommand">

            <Columns>
                <asp:BoundField DataField="LibroId" HeaderText="ID" ReadOnly="True" />
                <asp:BoundField DataField="Titulo" HeaderText="Título" />
                <asp:BoundField DataField="Editorial" HeaderText="Editorial" />
                <asp:BoundField DataField="Categoria" HeaderText="Categoría" />

                <asp:BoundField DataField="PrecioCompra" HeaderText="Precio compra" DataFormatString="Q{0:N2}" HtmlEncode="false" />
                <asp:BoundField DataField="PrecioUnitario" HeaderText="Precio venta" DataFormatString="Q{0:N2}" HtmlEncode="false" />

                <asp:TemplateField HeaderText="Stock (unidades)">
                    <ItemTemplate>
                        <span style='<%# ObtenerColorStock(Eval("StockUnidades")) %>'>
                            <%# Eval("StockUnidades") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Dar de baja">
                    <ItemTemplate>
                        <asp:Button ID="btnDarDeBaja" runat="server" Text="🚫 Dar de baja"
                            CssClass="btn btn-sm btn-danger"
                            CommandName="DarDeBaja"
                            CommandArgument='<%# Eval("LibroId") %>'
                            OnClientClick="return confirm('¿Deseas dar de baja este libro?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <div class="text-end mt-3">
            <asp:Label ID="lblTotalLibros" runat="server" CssClass="fw-bold"></asp:Label>
        </div>
    </div>

</asp:Content>
