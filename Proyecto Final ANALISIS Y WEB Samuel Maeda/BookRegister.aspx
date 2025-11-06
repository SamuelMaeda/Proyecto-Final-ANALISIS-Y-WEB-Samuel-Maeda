<%@ Page Title="Registrar Libro" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="BookRegister.aspx.cs"
    Inherits="Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda.BookRegister" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow p-4">
        <h2 class="text-center mb-4">📚 Registrar Nuevo Libro</h2>

        <div class="row g-3">
            <!-- Título -->
            <div class="col-md-6">
                <label class="form-label">Título:</label>
                <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control" placeholder="Ingrese el título del libro"></asp:TextBox>
            </div>

            <!-- Autor -->
            <div class="col-md-6">
                <label class="form-label">Autor:</label>
                <asp:TextBox ID="txtAutor" runat="server" CssClass="form-control" placeholder="Ingrese el autor del libro"></asp:TextBox>
            </div>

            <!-- Categoría -->
            <div class="col-md-4">
                <label class="form-label">Categoría:</label>
                <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select"
                    AutoPostBack="true" OnSelectedIndexChanged="ddlCategoria_SelectedIndexChanged">
                </asp:DropDownList>
            </div>

            <!-- Editorial -->
            <div class="col-md-4">
                <label class="form-label">Editorial:</label>
                <asp:DropDownList ID="ddlEditorial" runat="server" CssClass="form-select"></asp:DropDownList>
            </div>

            <!-- Stock -->
            <div class="col-md-4">
                <label class="form-label">Stock (unidades):</label>
                <asp:TextBox ID="txtStock" runat="server" CssClass="form-control" TextMode="Number" min="0" placeholder="Ejemplo: 50"></asp:TextBox>
            </div>

            <!-- Precio de compra -->
            <div class="col-md-6">
                <label class="form-label">Precio de Compra (Q):</label>
                <asp:TextBox ID="txtPrecioCompra" runat="server" CssClass="form-control" TextMode="Number" step="0.01" placeholder="Ejemplo: 50.00"></asp:TextBox>
            </div>

            <!-- Precio de venta -->
            <div class="col-md-6">
                <label class="form-label">Precio de Venta (Q):</label>
                <asp:TextBox ID="txtPrecioVenta" runat="server" CssClass="form-control" TextMode="Number" step="0.01" placeholder="Ejemplo: 80.00"></asp:TextBox>
            </div>

            <!-- Botones -->
            <div class="text-center mt-4">
                <asp:Button ID="btnRegistrar" runat="server" Text="Registrar Libro" CssClass="btn btn-primary px-4"
                    OnClick="btnRegistrar_Click" />
                <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar Campos" CssClass="btn btn-secondary px-4 ms-2"
                    OnClick="Page_Load" CausesValidation="false" />
            </div>
        </div>

        <hr class="my-4" />

        <!-- Mensaje de estado -->
        <div class="text-center mb-4">
            <asp:Label ID="lblMensaje" runat="server" Text="" CssClass="fw-bold"></asp:Label>
        </div>

        <!-- Últimos 3 libros agregados -->
        <h4 class="fw-bold text-center mb-3">📘 Últimos 3 libros agregados</h4>
        <asp:GridView ID="gvUltimosLibros" runat="server"
            CssClass="table table-striped table-hover text-center"
            AutoGenerateColumns="False" ShowHeaderWhenEmpty="true">
            <Columns>
                <asp:BoundField DataField="LibroId" HeaderText="ID" />
                <asp:BoundField DataField="Titulo" HeaderText="Título" />
                <asp:BoundField DataField="Editorial" HeaderText="Editorial" />
                <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                <asp:TemplateField HeaderText="Precio compra">
                    <ItemTemplate>Q<%# Eval("PrecioCompra", "{0:N2}") %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Precio venta">
                    <ItemTemplate>Q<%# Eval("PrecioVenta", "{0:N2}") %></ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="StockUnidades" HeaderText="Stock (unidades)" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
    