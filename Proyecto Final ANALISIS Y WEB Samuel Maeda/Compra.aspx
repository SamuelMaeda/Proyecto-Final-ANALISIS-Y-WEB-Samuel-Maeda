<%@ Page Title="Registrar Compra" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Compra.aspx.cs" Inherits="Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda.Compra" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow p-4">
        <h2 class="text-center mb-4">🛒 Registrar Compra</h2>

        <div class="row g-3">
            <div class="col-md-6">
                <label class="form-label">Proveedor:</label>
                <asp:DropDownList ID="ddlProveedor" runat="server" CssClass="form-select"></asp:DropDownList>
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
                <label class="form-label">Precio total:</label>
                <asp:TextBox ID="txtTotal" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
            </div>
        </div>

        <div class="text-center mt-4">
            <asp:Button ID="btnAgregarCompra" runat="server" Text="Registrar Compra" CssClass="btn btn-success" OnClick="btnAgregarCompra_Click" />
        </div>
    </div>
</asp:Content>
