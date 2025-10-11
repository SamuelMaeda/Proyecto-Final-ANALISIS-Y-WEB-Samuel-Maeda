<%@ Page Title="Registrar Venta" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Venta.aspx.cs" Inherits="Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda.Venta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow p-4">
        <h2 class="text-center mb-4">💸 Registrar Venta</h2>

        <div class="row g-3">
            <div class="col-md-6">
                <label class="form-label">Cliente:</label>
                <asp:DropDownList ID="ddlCliente" runat="server" CssClass="form-select"></asp:DropDownList>
            </div>
        </div>

        <hr />

        <div class="row g-3 align-items-end">
            <div class="col-md-4">
                <label class="form-label">Libro:</label>
                <asp:DropDownList ID="ddlLibro" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlLibro_SelectedIndexChanged"></asp:DropDownList>
            </div>

            <div class="col-md-3">
                <label class="form-label">Unidad de medida:</label>
                <asp:DropDownList ID="ddlUnidad" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlUnidad_SelectedIndexChanged"></asp:DropDownList>
            </div>

            <div class="col-md-2">
                <label class="form-label">Cantidad:</label>
                <asp:TextBox ID="txtCantidad" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
            </div>

            <div class="col-md-3">
                <label class="form-label">Stock actual:</label>
                <asp:TextBox ID="txtStockActual" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
            </div>
        </div>

        <div class="row mt-3">
            <div class="col-md-6">
                <label class="form-label">Total a pagar:</label>
                <asp:TextBox ID="txtTotal" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
            </div>
        </div>

        <div class="text-center mt-4">
            <asp:Button ID="btnRegistrarVenta" runat="server" Text="Registrar Venta" CssClass="btn btn-success" OnClick="btnRegistrarVenta_Click" />
        </div>
    </div>
</asp:Content>
