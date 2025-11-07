<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reportes.aspx.cs"
    Inherits="Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda.Reportes" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card shadow p-4">

        <h2 class="mb-4 text-center">📊 Reportes del Sistema</h2>

        <!-- Nav tabs -->
        <ul class="nav nav-tabs mb-3" id="tabReportes" role="tablist">
            <li class="nav-item" role="presentation">
                <button class="nav-link active" id="tabComprasBtn" data-bs-toggle="tab"
                    data-bs-target="#tabCompras" type="button" role="tab">🧾 Historial de Compras</button>
            </li>
            <li class="nav-item" role="presentation">
                <button class="nav-link" id="tabVentasBtn" data-bs-toggle="tab"
                    data-bs-target="#tabVentas" type="button" role="tab">💸 Historial de Ventas</button>
            </li>
            <li class="nav-item" role="presentation">
                <button class="nav-link" id="tabEstadisticasBtn" data-bs-toggle="tab"
                    data-bs-target="#tabEstadisticas" type="button" role="tab">📈 Estadísticas</button>
            </li>
        </ul>

        <div class="tab-content">

            <!-- ==================== COMPRAS ==================== -->
            <div class="tab-pane fade show active" id="tabCompras" role="tabpanel">
                <h4 class="fw-bold text-center mb-3">🧾 Historial de Compras</h4>

                <div class="row gx-2 align-items-end mb-3 justify-content-center">
                    <div class="col-auto">
                        <label class="form-label">Desde</label>
                        <asp:TextBox ID="txtDesdeCompras" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-auto">
                        <label class="form-label">Hasta</label>
                        <asp:TextBox ID="txtHastaCompras" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-auto">
                        <asp:Button ID="btnFiltrarCompras" runat="server" Text="Filtrar" CssClass="btn btn-primary"
                            OnClick="btnFiltrarCompras_Click" />
                    </div>
                    <div class="col-auto">
                        <asp:Button ID="btnLimpiarCompras" runat="server" Text="Limpiar" CssClass="btn btn-outline-secondary"
                            OnClick="btnLimpiarCompras_Click" />
                    </div>
                </div>

                <asp:GridView ID="gvHistorialCompras" runat="server"
                    CssClass="table table-striped table-hover text-center"
                    AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" AllowPaging="true" PageSize="10"
                    OnPageIndexChanging="gvHistorialCompras_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="FechaHora" HeaderText="Fecha y Hora" />
                        <asp:BoundField DataField="Proveedor" HeaderText="Proveedor" />
                        <asp:BoundField DataField="LibrosComprados" HeaderText="Libros Comprados" />
                        <asp:BoundField DataField="Categorias" HeaderText="Categorías" />
                        <asp:BoundField DataField="Editoriales" HeaderText="Editoriales" />
                        <asp:BoundField DataField="Descuentos" HeaderText="Descuentos" />
                        <asp:TemplateField HeaderText="Total (con IVA)">
                            <ItemTemplate>Q<%# Eval("TotalConIVA", "{0:N2}") %></ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <!-- ==================== VENTAS ==================== -->
            <div class="tab-pane fade" id="tabVentas" role="tabpanel">
                <h4 class="fw-bold text-center mb-3">💸 Historial de Ventas</h4>

                <div class="row gx-2 align-items-end mb-3 justify-content-center">
                    <div class="col-auto">
                        <label class="form-label">Desde</label>
                        <asp:TextBox ID="txtDesdeVentas" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-auto">
                        <label class="form-label">Hasta</label>
                        <asp:TextBox ID="txtHastaVentas" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-auto">
                        <asp:Button ID="btnFiltrarVentas" runat="server" Text="Filtrar" CssClass="btn btn-primary"
                            OnClick="btnFiltrarVentas_Click" />
                    </div>
                    <div class="col-auto">
                        <asp:Button ID="btnLimpiarVentas" runat="server" Text="Limpiar" CssClass="btn btn-outline-secondary"
                            OnClick="btnLimpiarVentas_Click" />
                    </div>
                </div>

                <asp:GridView ID="gvHistorialVentas" runat="server"
                    CssClass="table table-striped table-hover text-center"
                    AutoGenerateColumns="False" ShowHeaderWhenEmpty="true" AllowPaging="true" PageSize="10"
                    OnPageIndexChanging="gvHistorialVentas_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="Fecha" HeaderText="Fecha y Hora" />
                        <asp:BoundField DataField="Cliente" HeaderText="Cliente" />
                        <asp:BoundField DataField="LibrosVendidos" HeaderText="Detalles de la Venta" />
                        <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                        <asp:BoundField DataField="Editorial" HeaderText="Editoriales" />
                        <asp:TemplateField HeaderText="Descuento total">
                            <ItemTemplate><%# "Q" + string.Format("{0:N2}", Eval("DescuentoTotal")) %></ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total (con IVA)">
                            <ItemTemplate>Q<%# Eval("TotalConIVA", "{0:N2}") %></ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <!-- ==================== ESTADÍSTICAS ==================== -->
            <div class="tab-pane fade" id="tabEstadisticas" role="tabpanel">
                <h4 class="fw-bold text-center mb-4">📈 Estadísticas Generales</h4>

                <asp:GridView ID="gvEstadisticas" runat="server"
                    CssClass="table table-bordered text-center"
                    AutoGenerateColumns="False" ShowHeaderWhenEmpty="true">
                    <Columns>
                        <asp:BoundField DataField="Indicador" HeaderText="Indicador" />
                        <asp:BoundField DataField="Valor" HeaderText="Valor" />
                    </Columns>
                </asp:GridView>
            </div>

        </div>

    </div>

    <script type="text/javascript">
        window.addEventListener('load', function () {
            ['<%= txtDesdeCompras.ClientID %>', '<%= txtHastaCompras.ClientID %>',
             '<%= txtDesdeVentas.ClientID %>', '<%= txtHastaVentas.ClientID %>']
                .forEach(id => { var el = document.getElementById(id); if (el) el.type = 'date'; });
        });
    </script>

</asp:Content>
