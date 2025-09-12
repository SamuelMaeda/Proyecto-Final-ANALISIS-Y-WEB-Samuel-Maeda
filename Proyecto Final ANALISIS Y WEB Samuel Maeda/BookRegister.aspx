<%@ Page Title="Registro de Libros" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BookRegister.aspx.cs" Inherits="Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda.BookRegister" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card shadow p-4">
        <h2 class="mb-4 text-center">Registro de Libros</h2>

        <!-- Formulario para agregar libros -->
        <div class="mb-3">
            <asp:Label ID="lblTitulo" runat="server" Text="Título:" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control"></asp:TextBox>
        </div>

        <div class="mb-3">
            <asp:Label ID="lblAutor" runat="server" Text="Autor:" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="txtAutor" runat="server" CssClass="form-control"></asp:TextBox>
        </div>

        <div class="mb-3">
            <asp:Label ID="lblCategoria" runat="server" Text="Categoría:" CssClass="form-label"></asp:Label>
            <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select">
                <asp:ListItem Text="-- Seleccione --" Value=""></asp:ListItem>
                <asp:ListItem Text="Novela" Value="Novela"></asp:ListItem>
                <asp:ListItem Text="Infantil" Value="Infantil"></asp:ListItem>
                <asp:ListItem Text="Educativo" Value="Educativo"></asp:ListItem>
                <asp:ListItem Text="Ciencia" Value="Ciencia"></asp:ListItem>
                <asp:ListItem Text="Otros" Value="Otros"></asp:ListItem>
            </asp:DropDownList>
        </div>

        <div class="mb-3">
            <asp:Label ID="lblPrecio" runat="server" Text="Precio (Q):" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="txtPrecio" runat="server" CssClass="form-control"></asp:TextBox>
        </div>

        <div class="mb-3">
            <asp:Label ID="lblCantidad" runat="server" Text="Cantidad:" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="txtCantidad" runat="server" CssClass="form-control"></asp:TextBox>
        </div>

        <div class="mb-3">
            <asp:Label ID="lblDescripcion" runat="server" Text="Descripción:" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="txtDescripcion" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
        </div>

        <div class="text-center">
            <asp:Button ID="btnGuardar" runat="server" Text="Guardar Libro" CssClass="btn btn-success" OnClick="btnGuardar_Click" />
        </div>

        <hr />

        <!-- GridView para mostrar los libros registrados -->
        <h4 class="mt-4">Últimos Libros Registrados</h4>
        <asp:GridView ID="gvLibros" runat="server" CssClass="table table-striped mt-3" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="Titulo" HeaderText="Título" />
                <asp:BoundField DataField="Autor" HeaderText="Autor" />
                <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                <asp:BoundField DataField="Precio" HeaderText="Precio (Q)" DataFormatString="{0:F2}" />
                <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" />
                <asp:BoundField DataField="FechaCreacion" HeaderText="Fecha de Registro" DataFormatString="{0:dd/MM/yyyy}" />
            </Columns>
        </asp:GridView>

        <!-- Mensaje de estado -->
        <asp:Label ID="lblMensaje" runat="server" CssClass="text-success mt-3 d-block"></asp:Label>
    </div>

</asp:Content>
