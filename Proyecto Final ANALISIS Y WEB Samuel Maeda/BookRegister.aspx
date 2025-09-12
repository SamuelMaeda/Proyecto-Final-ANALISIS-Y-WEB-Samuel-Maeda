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

        <!-- GridView para mostrar y editar/eliminar libros -->
        <h4 class="mt-4">Últimos Libros Registrados</h4>
        <asp:GridView ID="gvLibros" runat="server" CssClass="table table-striped mt-3"
            AutoGenerateColumns="False" DataKeyNames="Id"
            OnRowEditing="gvLibros_RowEditing"
            OnRowCancelingEdit="gvLibros_RowCancelingEdit"
            OnRowUpdating="gvLibros_RowUpdating"
            OnRowDeleting="gvLibros_RowDeleting">

            <Columns>
                <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="true" />

                <asp:TemplateField HeaderText="Título">
                    <ItemTemplate><%# Eval("Titulo") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEditTitulo" runat="server" Text='<%# Bind("Titulo") %>' CssClass="form-control"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Autor">
                    <ItemTemplate><%# Eval("Autor") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEditAutor" runat="server" Text='<%# Bind("Autor") %>' CssClass="form-control"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Categoría">
                    <ItemTemplate><%# Eval("Categoria") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlEditCategoria" runat="server" CssClass="form-select" SelectedValue='<%# Bind("Categoria") %>'>
                            <asp:ListItem Text="Novela" Value="Novela"></asp:ListItem>
                            <asp:ListItem Text="Infantil" Value="Infantil"></asp:ListItem>
                            <asp:ListItem Text="Educativo" Value="Educativo"></asp:ListItem>
                            <asp:ListItem Text="Ciencia" Value="Ciencia"></asp:ListItem>
                            <asp:ListItem Text="Otros" Value="Otros"></asp:ListItem>
                        </asp:DropDownList>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Precio (Q)">
                    <ItemTemplate><%# Eval("Precio", "{0:F2}") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEditPrecio" runat="server" Text='<%# Bind("Precio") %>' CssClass="form-control"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Cantidad">
                    <ItemTemplate><%# Eval("Cantidad") %></ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtEditCantidad" runat="server" Text='<%# Bind("Cantidad") %>' CssClass="form-control"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="FechaCreacion" HeaderText="Fecha de Registro" DataFormatString="{0:dd/MM/yyyy}" ReadOnly="true" />

                <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" />
            </Columns>
        </asp:GridView>

        <!-- Mensaje de estado -->
        <asp:Label ID="lblMensaje" runat="server" CssClass="text-success mt-3 d-block"></asp:Label>
    </div>

</asp:Content>
