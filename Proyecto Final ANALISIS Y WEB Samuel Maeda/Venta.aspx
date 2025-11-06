<%@ Page Title="Registrar Venta" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Venta.aspx.cs"
    Inherits="Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda.Ventas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card shadow p-4">
        <h2 class="text-center mb-4">💸 Registrar Venta</h2>

        <div class="row g-3">
            <!-- Cliente -->
            <div class="col-md-4">
                <label class="form-label">Cliente:</label>
                <!-- Clase select2 para inicializar -->
                <asp:DropDownList ID="ddlCliente" runat="server" CssClass="form-select select2"
                    AutoPostBack="true" OnSelectedIndexChanged="ddlCliente_SelectedIndexChanged" 
                    data-placeholder="Buscar cliente..."></asp:DropDownList>
                <div class="mt-2">
                    <asp:Button ID="btnAgregarCliente" runat="server" Text="➕ Agregar Cliente" CssClass="btn btn-outline-primary btn-sm w-100" OnClick="btnAgregarCliente_Click" />
                </div>
            </div>

            <!-- Libro -->
            <div class="col-md-4">
                <label class="form-label">Libro:</label>
                <asp:DropDownList ID="ddlLibro" runat="server" CssClass="form-select select2"
                    AutoPostBack="true" OnSelectedIndexChanged="ddlLibro_SelectedIndexChanged"
                    data-placeholder="Buscar libro..."></asp:DropDownList>
            </div>

            <!-- Categoría -->
            <div class="col-md-4">
                <label class="form-label">Categoría:</label>
                <asp:TextBox ID="txtCategoria" runat="server" CssClass="form-control bg-light" ReadOnly="true"
                    placeholder="Seleccione un libro"></asp:TextBox>
            </div>

            <!-- Unidad -->
            <div class="col-md-4">
                <label class="form-label">Unidad de medida:</label>
                <asp:DropDownList ID="ddlUnidad" runat="server" CssClass="form-select select2" data-placeholder="Seleccione unidad..."></asp:DropDownList>
            </div>

            <!-- Cantidad -->
            <div class="col-md-4">
                <label class="form-label">Cantidad:</label>
                <asp:TextBox ID="txtCantidad" runat="server" CssClass="form-control"
                    TextMode="Number" placeholder="Ejemplo: 5" min="1"></asp:TextBox>
            </div>

            <!-- Stock -->
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

        <asp:Label ID="lblMensaje" runat="server" CssClass="mb-3 d-block"></asp:Label>

        <h4 class="text-center mb-3 fw-bold" style="font-size: 1.4rem;">📚 Libros en la venta actual</h4>

        <asp:GridView ID="gvVentaActual" runat="server"
            CssClass="table table-bordered table-hover text-center"
            AutoGenerateColumns="False" ShowHeaderWhenEmpty="true">
            <Columns>
                <asp:BoundField DataField="Titulo" HeaderText="Título" />
                <asp:BoundField DataField="Editorial" HeaderText="Editorial" />
                <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                <asp:BoundField DataField="UnidadNombre" HeaderText="Unidad de medida" />
                <asp:BoundField DataField="Cantidad" HeaderText="Cantidad (unidades)" />
                <asp:TemplateField HeaderText="Precio Unitario">
                    <ItemTemplate>Q<%# Eval("PrecioUnitario", "{0:N2}") %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Subtotal sin descuento">
                    <ItemTemplate>Q<%# Eval("Subtotal", "{0:N2}") %></ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Descuento aplicado por unidad de medida">
                    <ItemTemplate>
                        <%# Convert.ToDecimal(Eval("DescuentoAplicado")) > 0
                            ? "Q" + string.Format("{0:N2}", Eval("DescuentoAplicado"))
                            : "—" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Subtotal con descuento">
                    <ItemTemplate>Q<%# Eval("SubtotalConDescuento", "{0:N2}") %></ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <asp:Label ID="lblMensajeVacio" runat="server"
            Text="Todavía no se ha seleccionado ningún libro en la lista de ventas."
            CssClass="alert alert-info text-center d-block"
            Visible="false">
        </asp:Label>

        <div class="text-center mt-2">
            <asp:Button ID="btnLimpiarLista" runat="server" Text="Limpiar Lista"
                CssClass="btn btn-secondary"
                OnClick="btnLimpiarLista_Click" />
        </div>

        <div class="text-end mt-4">
            <h5><asp:Label ID="lblTotal" runat="server" Text="Subtotal sin descuento: Q0.00"></asp:Label></h5>
            <h5><asp:Label ID="lblDescuento" runat="server" Text="Descuento total aplicado: Q0.00"></asp:Label></h5>
            <h5><asp:Label ID="lblSubtotalConDesc" runat="server" Text="Subtotal con descuento: Q0.00"></asp:Label></h5>
            <h5><asp:Label ID="lblIVA" runat="server" Text="IVA (12%): Q0.00"></asp:Label></h5>
            <h4 class="text-primary fw-bold">
                <asp:Label ID="lblTotalFinal" runat="server" Text="TOTAL FINAL (con IVA): Q0.00"></asp:Label>
            </h4>
            <p class="text-success fw-semibold">
                <asp:Label ID="lblGananciaTotal" runat="server" Text="Ganancia total: Q0.00"></asp:Label>
            </p>
        </div>
    </div>

    <hr class="my-4" />

    <!-- SECCIÓN FACTURA (botón de descarga) -->
    <div class="card shadow p-3 bg-light text-center mb-4">
        <h5 class="fw-bold mb-3">🧾 Última factura generada</h5>
        <asp:Panel ID="pnlFactura" runat="server" Visible="false">
            <div class="mt-3">
                <asp:HyperLink ID="lnkDescargarFactura" runat="server" 
                    CssClass="btn btn-primary fw-bold" 
                    Target="_blank" Visible="false">
                    📄 Abrir Factura PDF
                </asp:HyperLink>
            </div>
        </asp:Panel>
        <asp:Label ID="lblNoFactura" runat="server" 
            Text="No hay facturas generadas aún."
            CssClass="text-muted" Visible="true">
        </asp:Label>
    </div>

    <!-- ÚLTIMAS VENTAS -->
    <h5 class="mt-4 mb-3 fw-bold">🧾 Últimas ventas registradas</h5>
    <asp:GridView ID="gvUltimasVentas" runat="server"
        CssClass="table table-striped table-hover text-center"
        AutoGenerateColumns="False" ShowHeaderWhenEmpty="true">
        <Columns>
            <asp:BoundField DataField="Fecha" HeaderText="Fecha y Hora" />
            <asp:BoundField DataField="Cliente" HeaderText="Cliente" />
            <asp:BoundField DataField="LibrosVendidos" HeaderText="Detalles de la Venta" />
            <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
            <asp:BoundField DataField="Editorial" HeaderText="Editoriales" />
            <asp:TemplateField HeaderText="Descuento total">
                <ItemTemplate><%# Eval("DescuentoAplicado") is DBNull ? "Q0.00" : "Q" + string.Format("{0:N2}", Eval("DescuentoAplicado")) %></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Total (con IVA)">
                <ItemTemplate>Q<%# Eval("TotalConIVA", "{0:N2}") %></ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

</asp:Content>
