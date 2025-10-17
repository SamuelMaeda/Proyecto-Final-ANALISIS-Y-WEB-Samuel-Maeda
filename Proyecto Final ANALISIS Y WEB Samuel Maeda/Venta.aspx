<%@ Page Title="Registrar Venta" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="Ventas.aspx.cs" Inherits="LuzDelSaber.Ventas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card shadow p-4">
        <h2 class="text-center mb-4">💸 Registrar Venta</h2>

        <div class="row g-3">
            <!-- Cliente -->
            <div class="col-md-4">
                <label class="form-label">Cliente:</label>
                <asp:DropDownList ID="ddlCliente" runat="server" CssClass="form-select">
                    <asp:ListItem Text="-- Seleccione un cliente --" Value="0"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <!-- Libro -->
            <div class="col-md-4">
                <label class="form-label">Libro:</label>
                <asp:DropDownList ID="ddlLibro" runat="server" CssClass="form-select" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlLibro_SelectedIndexChanged"></asp:DropDownList>
            </div>

            <!-- Categoría del libro seleccionado (solo lectura) -->
            <div class="col-md-4">
                <label class="form-label">Categoría:</label>
                <asp:TextBox ID="txtCategoria" runat="server" CssClass="form-control bg-light" ReadOnly="true"
                    placeholder="Seleccione un libro"></asp:TextBox>
            </div>

            <!-- Unidad de medida -->
            <div class="col-md-4">
                <label class="form-label">Unidad de medida:</label>
                <asp:DropDownList ID="ddlUnidad" runat="server" CssClass="form-select"></asp:DropDownList>
            </div>

            <!-- Cantidad -->
            <div class="col-md-4">
                <label class="form-label">Cantidad:</label>
                <asp:TextBox ID="txtCantidad" runat="server" CssClass="form-control" TextMode="Number" 
                    placeholder="Ejemplo: 5" min="1"></asp:TextBox>
            </div>

            <!-- Stock actual -->
            <div class="col-md-4">
                <label class="form-label">Stock actual (unidades):</label>
                <asp:Label ID="lblStock" runat="server" Text="-" CssClass="form-control bg-light"></asp:Label>
            </div>

            <div class="text-center mt-3">
                <asp:Button ID="btnAgregarLibro" runat="server" Text="Agregar Libro" CssClass="btn btn-success"
                    OnClick="btnAgregarLibro_Click" />
                <asp:Button ID="btnRegistrarVenta" runat="server" Text="Registrar Venta" CssClass="btn btn-primary"
                    OnClick="btnRegistrarVenta_Click" />
                <asp:Button ID="btnLimpiarTodo" runat="server" Text="Limpiar Todo" CssClass="btn btn-secondary"
                    OnClick="btnLimpiarTodo_Click" />
            </div>
        </div>

        <hr class="my-4" />

        <!-- Lista de libros -->
        <h4 class="text-center mb-3 fw-bold" style="font-size: 1.4rem;">📚 Libros en la venta actual</h4>

        <asp:GridView ID="gvVentaActual" runat="server" CssClass="table table-bordered table-hover text-center"
            AutoGenerateColumns="False" ShowHeaderWhenEmpty="true">
            <Columns>
                <asp:BoundField DataField="Titulo" HeaderText="Título" />
                <asp:BoundField DataField="Editorial" HeaderText="Editorial" />
                <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                <asp:BoundField DataField="UnidadNombre" HeaderText="Unidad" />
                <asp:BoundField DataField="Cantidad" HeaderText="Cantidad (unidades)" />
                <asp:TemplateField HeaderText="Precio Unitario">
                    <ItemTemplate>Q<%# Eval("PrecioUnitario", "{0:N2}") %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Ganancia Unitaria">
                    <ItemTemplate>Q<%# Eval("GananciaUnitaria", "{0:N2}") %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Subtotal">
                    <ItemTemplate>Q<%# Eval("Subtotal", "{0:N2}") %></ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <!-- Mensaje cuando la lista está vacía -->
        <asp:Label ID="lblMensajeVacio" runat="server" 
            Text="Todavía no se ha seleccionado ningún libro en la lista de ventas." 
            CssClass="alert alert-info text-center d-block"
            Visible="false">
        </asp:Label>

        <div class="text-center mt-2">
            <asp:Button ID="btnLimpiarLista" runat="server" Text="Limpiar Lista" CssClass="btn btn-secondary"
                OnClick="btnLimpiarLista_Click" />
        </div>

        <!-- Totales -->
        <div class="text-end mt-4">
            <h5><asp:Label ID="lblTotal" runat="server" Text="Total: Q0.00"></asp:Label></h5>
            <p class="text-success fw-semibold">
                <asp:Label ID="lblGananciaTotal" runat="server" Text="Ganancia total: Q0.00"></asp:Label>
            </p>
        </div>
    </div>

    <hr class="my-4" />

    <!-- Últimas ventas -->
    <h5 class="mt-4 mb-3">🧾 Últimas 5 ventas registradas</h5>
    <asp:GridView ID="gvUltimasVentas" runat="server" CssClass="table table-striped table-hover text-center" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Fecha" HeaderText="Fecha" />
            <asp:BoundField DataField="Hora" HeaderText="Hora" />
            <asp:BoundField DataField="Cliente" HeaderText="Cliente" />
            <asp:BoundField DataField="Productos" HeaderText="Libros Vendidos" />
            <asp:BoundField DataField="Categorias" HeaderText="Categorías" />
            <asp:BoundField DataField="Editoriales" HeaderText="Editoriales" />
            <asp:TemplateField HeaderText="Total">
                <ItemTemplate>Q<%# Eval("Total", "{0:N2}") %></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Ganancia">
                <ItemTemplate>Q<%# Eval("Ganancia", "{0:N2}") %></ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

</asp:Content>