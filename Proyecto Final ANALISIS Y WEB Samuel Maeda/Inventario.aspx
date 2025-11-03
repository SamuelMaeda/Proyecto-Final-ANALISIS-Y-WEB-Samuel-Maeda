<%@ Page Title="Inventario" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Inventario.aspx.cs" Inherits="LuzDelSaber.Inventario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card shadow p-4">
        <h2 class="text-center mb-4">📦 Inventario de Libros</h2>

    
        <div class="row align-items-center mb-3">
            <div class="col-md-6 d-flex">
                <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control me-2" placeholder="Buscar libro, editorial o categoría..." />
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-primary me-2" OnClick="btnBuscar_Click" />
                <asp:Button ID="btnReiniciar" runat="server" Text="Limpiar" CssClass="btn btn-secondary" OnClick="btnReiniciar_Click" />
            </div>

            <div class="col-md-6 text-end">
                <label for="ddlOrdenarPor" class="me-2 fw-bold">Ordenar por:</label>
                <asp:DropDownList ID="ddlOrdenarPor" runat="server" CssClass="form-select d-inline-block w-auto me-2">
                    <asp:ListItem Value="L.LibroId">ID</asp:ListItem>
                    <asp:ListItem Value="L.Titulo">Título</asp:ListItem>
                    <asp:ListItem Value="E.Nombre">Editorial</asp:ListItem>
                    <asp:ListItem Value="C.Nombre">Categoría</asp:ListItem>
                    <asp:ListItem Value="C.PrecioBase">Precio</asp:ListItem>
                    <asp:ListItem Value="L.StockUnidades">Stock</asp:ListItem>
                </asp:DropDownList>

                <label for="ddlOrden" class="me-2 fw-bold">Orden:</label>
                <asp:DropDownList ID="ddlOrden" runat="server" CssClass="form-select d-inline-block w-auto">
                    <asp:ListItem Value="ASC">Ascendente</asp:ListItem>
                    <asp:ListItem Value="DESC">Descendente</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>

      
        <asp:GridView ID="gvLibros" runat="server" CssClass="table table-bordered table-hover text-center align-middle"
            AutoGenerateColumns="False" DataKeyNames="LibroId"
            AllowPaging="True" PageSize="10"
            OnPageIndexChanging="gvLibros_PageIndexChanging"
            OnRowEditing="gvLibros_RowEditing"
            OnRowCancelingEdit="gvLibros_RowCancelingEdit"
            OnRowUpdating="gvLibros_RowUpdating"
            OnRowDeleting="gvLibros_RowDeleting"
            OnRowDataBound="gvLibros_RowDataBound">

            <Columns>
                <asp:BoundField DataField="LibroId" HeaderText="ID" ReadOnly="True" />
                <asp:BoundField DataField="Titulo" HeaderText="Título" />
                <asp:BoundField DataField="Editorial" HeaderText="Editorial" />
                <asp:BoundField DataField="Categoria" HeaderText="Categoría" />

                <asp:TemplateField HeaderText="Precio de venta">
                    <ItemTemplate>
                        <%# "Q" + String.Format("{0:N2}", Eval("PrecioVenta")) %>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtPrecio" runat="server" CssClass="form-control text-center"
                            Text='<%# Bind("PrecioVenta") %>' />
                    </EditItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Stock (unidades)">
                    <ItemTemplate>
                        <span style='<%# ObtenerColorStock(Eval("StockUnidades")) %>'>
                            <%# Eval("StockUnidades") %>
                        </span>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtStock" runat="server" CssClass="form-control text-center"
                            Text='<%# Bind("StockUnidades") %>' />
                    </EditItemTemplate>
                </asp:TemplateField>

             
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEditar" runat="server" CommandName="Edit" CssClass="btn btn-sm btn-warning">✏️ Editar</asp:LinkButton>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:LinkButton ID="lnkActualizar" runat="server" CommandName="Update" CssClass="btn btn-sm btn-success me-1">Guardar</asp:LinkButton>
                        <asp:LinkButton ID="lnkCancelar" runat="server" CommandName="Cancel" CssClass="btn btn-sm btn-secondary">Cancelar</asp:LinkButton>
                    </EditItemTemplate>
                </asp:TemplateField>
                
      
                <asp:TemplateField HeaderText="Eliminar">
                    <ItemTemplate>
                        <asp:Button ID="btnEliminar" runat="server" Text="🗑️ Eliminar"
                            CssClass="btn btn-sm btn-danger"
                            CommandName="Delete"
                            Visible="false"
                            OnClientClick="return confirm('¿Seguro que deseas eliminar este libro?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <div class="text-end mt-2">
            <asp:Label ID="lblTotalLibros" runat="server" CssClass="fw-bold"></asp:Label>
        </div>
    </div>
</asp:Content>
