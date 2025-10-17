<%@ Page Title="Registro de Libros" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="BookRegister.aspx.cs" Inherits="Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda.BookRegister" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow p-4">
        <h2 class="text-center mb-4">📚 Registro de Libros</h2>

        <!-- Formulario de registro -->
        <div class="row g-3">
            <div class="col-md-6">
                <label class="form-label">Título:</label>
                <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control" placeholder="Ingrese el título del libro"></asp:TextBox>
            </div>

            <div class="col-md-6">
                <label class="form-label">Autor:</label>
                <asp:TextBox ID="txtAutor" runat="server" CssClass="form-control" placeholder="Ingrese el autor"></asp:TextBox>
            </div>
            
            <div class="col-md-6">
                <label class="form-label">Editorial:</label>
                <asp:DropDownList ID="ddlEditorial" runat="server" CssClass="form-select">
                </asp:DropDownList>
            </div>


            <div class="col-md-4">
                <label class="form-label">Categoría:</label>
                <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCategoria_SelectedIndexChanged"></asp:DropDownList>
            </div>

            <div class="col-md-4">
                <label class="form-label">Precio (según categoría):</label>
                <asp:TextBox ID="txtPrecio" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
            </div>

            <div class="col-md-4">
                <label class="form-label">Stock Inicial (Unidades):</label>
                <asp:TextBox ID="txtStock" runat="server" CssClass="form-control" TextMode="Number" placeholder="Ejemplo: 10"></asp:TextBox>
            </div>

            <div class="col-md-12">
                <label class="form-label">Descripción:</label>
                <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Descripción del libro"></asp:TextBox>
            </div>

            <div class="text-center mt-3">
                <asp:Button ID="btnAgregar" runat="server" Text="Agregar Libro" CssClass="btn btn-success" OnClick="btnAgregar_Click" />
                <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-secondary" OnClick="btnLimpiar_Click" />
            </div>
        </div>

        <hr class="my-4" />

        <!-- Tabla de libros -->
        <h4 class="text-center mb-3">📋 Lista de Libros Registrados</h4>
        <asp:GridView ID="gvLibros" runat="server" CssClass="table table-striped table-hover"
            AutoGenerateColumns="False" DataKeyNames="LibroId" OnRowDeleting="gvLibros_RowDeleting">
            <Columns>
                <asp:BoundField DataField="LibroId" HeaderText="ID" />
                <asp:BoundField DataField="Titulo" HeaderText="Título" />
                <asp:BoundField DataField="Autor" HeaderText="Autor" />
                <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                <asp:BoundField DataField="PrecioBase" HeaderText="Precio (Q)" />
                <asp:BoundField DataField="StockUnidades" HeaderText="Stock" />
                <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                <asp:CommandField ShowDeleteButton="True" DeleteText="Eliminar" />
            </Columns>
        </asp:GridView>

    </div>
</asp:Content>
