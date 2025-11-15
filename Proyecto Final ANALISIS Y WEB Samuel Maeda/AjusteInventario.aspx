<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AjusteInventario.aspx.cs"
    Inherits="Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda.AjusteInventario" 
    MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card shadow p-4">
        <h2 class="text-center mb-4">⚙️ Ajustes de Inventario</h2>

        <div class="row g-3 mb-4 justify-content-center">
            <div class="col-md-4">
                <label class="form-label fw-bold">Libro</label>
                <asp:DropDownList ID="ddlLibro" runat="server" CssClass="form-select" AppendDataBoundItems="true">
                    <asp:ListItem Text="-- Seleccione un libro --" Value="" />
                </asp:DropDownList>
            </div>

            <div class="col-md-2">
                <label class="form-label fw-bold">Cantidad</label>
                <asp:TextBox ID="txtCantidad" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
            </div>

            <div class="col-md-3">
                <label class="form-label fw-bold">Tipo de ajuste</label>
                <asp:DropDownList ID="ddlTipo" runat="server" CssClass="form-select">
                    <asp:ListItem Text="-- Seleccione --" Value="" />
                    <asp:ListItem Text="Robo" Value="Robo" />
                    <asp:ListItem Text="Extravio" Value="Extravio" />
                    <asp:ListItem Text="Deterioro" Value="Deterioro" />
                    <asp:ListItem Text="Error de conteo" Value="Error de conteo" />
                    <asp:ListItem Text="Ajuste positivo" Value="Ajuste positivo" />
                </asp:DropDownList>
            </div>

            <div class="col-md-8">
                <label class="form-label fw-bold">Motivo (obligatorio)</label>
                <asp:TextBox ID="txtMotivo" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2"></asp:TextBox>
            </div>

            <div class="col-md-2 d-flex align-items-end">
                <asp:Button ID="btnAplicar" runat="server" Text="Aplicar ajuste" CssClass="btn btn-primary w-100"
                    OnClick="btnAplicar_Click" />
            </div>
        </div>

        <hr />

        <div class="row gx-2 align-items-end mb-3 justify-content-center">
            <div class="col-auto">
                <label class="form-label">Desde</label>
                <asp:TextBox ID="txtDesde" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-auto">
                <label class="form-label">Hasta</label>
                <asp:TextBox ID="txtHasta" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-auto">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-primary"
                    OnClick="btnFiltrar_Click" />
            </div>
            <div class="col-auto">
                <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-outline-secondary"
                    OnClick="btnLimpiar_Click" />
            </div>
        </div>

        <asp:GridView ID="gvAjustes" runat="server"
            CssClass="table table-striped table-hover text-center"
            AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
            OnPageIndexChanging="gvAjustes_PageIndexChanging">

            <Columns>
                <asp:BoundField DataField="FechaAjuste" HeaderText="Fecha y Hora"
                    DataFormatString="{0:dd/MM/yyyy HH:mm}" />

                <asp:BoundField DataField="Libro" HeaderText="Libro" />

                <asp:BoundField DataField="TipoAjuste" HeaderText="Tipo de ajuste" />

                <asp:BoundField DataField="CantidadAjustada" HeaderText="Cantidad" />

                <asp:BoundField DataField="CantidadAnterior" HeaderText="Antes" />
                <asp:BoundField DataField="CantidadNueva" HeaderText="Después" />

                <asp:BoundField DataField="Usuario" HeaderText="Usuario" />
                <asp:BoundField DataField="Motivo" HeaderText="Motivo" />
            </Columns>

        </asp:GridView>

    </div>

    <script type="text/javascript">
        window.addEventListener('load', function () {
            ['<%= txtDesde.ClientID %>', '<%= txtHasta.ClientID %>']
                .forEach(id => { var el = document.getElementById(id); if (el) el.type = 'date'; });
        });
    </script>

</asp:Content>
