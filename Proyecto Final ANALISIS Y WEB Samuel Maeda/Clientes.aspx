<%@ Page Title="Registro de Clientes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Clientes.aspx.cs" Inherits="Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda.Clientes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow p-4">
        <h2 class="text-center mb-4">👥 Registro de Clientes</h2>

        <div class="row g-3">
           
            <asp:Panel ID="pnlVolverVentas" runat="server" Visible="false" CssClass="mb-3 text-end">
    <asp:Button ID="btnVolverVentas" runat="server" Text="⬅ Volver a Ventas" 
        CssClass="btn btn-outline-secondary btn-sm"
        OnClick="btnVolverVentas_Click" />
</asp:Panel>

            
            <!-- Nombre -->
            <div class="col-md-6">
                <label class="form-label">Nombre completo:</label>
                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" placeholder="Ingrese el nombre completo"></asp:TextBox>
            </div>

            <!-- NIT -->
            <div class="col-md-6">
                <label class="form-label">NIT:</label>
                <asp:TextBox ID="txtNIT" runat="server" CssClass="form-control" placeholder="Ejemplo: 1234567-8"></asp:TextBox>
            </div>

            <!-- Teléfono -->
            <div class="col-md-6">
                <label class="form-label">Teléfono:</label>
                <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" placeholder="Ejemplo: 5555-5555"></asp:TextBox>
            </div>

            <!-- Correo -->
            <div class="col-md-6">
                <label class="form-label">Correo electrónico:</label>
                <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" placeholder="Ejemplo: cliente@gmail.com" TextMode="Email"></asp:TextBox>
            </div>

            <div class="text-center mt-3">
                <asp:Button ID="btnRegistrarCliente" runat="server" Text="Registrar Cliente" CssClass="btn btn-primary"
                    OnClick="btnRegistrarCliente_Click" />
                <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-secondary ms-2"
                    OnClick="btnLimpiar_Click" />
            </div>
        </div>

        <hr class="my-4" />

        <!-- Listado de clientes -->
        <h4 class="text-center mb-3 fw-bold">📋 Lista de Clientes Registrados</h4>

        <asp:GridView ID="gvClientes" runat="server" CssClass="table table-bordered table-hover text-center"
            AutoGenerateColumns="False" ShowHeaderWhenEmpty="true">
            <Columns>
                <asp:BoundField DataField="Nombre" HeaderText="Nombre completo" />
                <asp:BoundField DataField="NIT" HeaderText="NIT" />
                <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                <asp:BoundField DataField="CorreoElectronico" HeaderText="Correo electrónico" />
            </Columns>
        </asp:GridView>

        <asp:Label ID="lblMensajeVacio" runat="server"
            Text="No hay clientes registrados aún."
            CssClass="alert alert-info text-center d-block"
            Visible="false">
        </asp:Label>
    </div>
</asp:Content>
