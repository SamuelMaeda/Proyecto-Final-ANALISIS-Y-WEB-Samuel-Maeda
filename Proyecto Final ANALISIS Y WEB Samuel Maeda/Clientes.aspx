<%@ Page Title="Registro de Clientes" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Clientes.aspx.cs"
    Inherits="Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda.Clientes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow p-4">
        <h2 class="text-center mb-4">👥 Gestión de Clientes</h2>

        <!-- Botón de volver a ventas -->
        <asp:Panel ID="pnlVolverVentas" runat="server" Visible="false" CssClass="mb-3 text-end">
            <asp:Button ID="btnVolverVentas" runat="server" Text="⬅ Volver a Ventas" 
                CssClass="btn btn-outline-secondary btn-sm"
                OnClick="btnVolverVentas_Click" />
        </asp:Panel>

        <!-- ==================== REGISTRAR CLIENTE ==================== -->
        <div class="border rounded p-3 mb-4 bg-light">
            <h4 class="mb-3 text-primary">➕ Registrar nuevo cliente</h4>

            <div class="row g-3 align-items-end">
                <div class="col-md-3">
                    <label class="form-label fw-bold">Nombre completo</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control"
                        placeholder="Ej. Juan Pérez"></asp:TextBox>
                </div>

                <div class="col-md-2">
                    <label class="form-label fw-bold">NIT</label>
                    <asp:TextBox ID="txtNIT" runat="server" CssClass="form-control"
                        placeholder="1234567-8"></asp:TextBox>
                </div>

                <div class="col-md-2">
                    <label class="form-label fw-bold">Teléfono</label>
                    <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control"
                        placeholder="5555-5555"></asp:TextBox>
                </div>

                <div class="col-md-3">
                    <label class="form-label fw-bold">Correo electrónico</label>
                    <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control"
                        TextMode="Email" placeholder="cliente@gmail.com"></asp:TextBox>
                </div>

                <div class="col-md-2 d-flex align-items-end">
                    <asp:Button ID="btnRegistrarCliente" runat="server" Text="Registrar cliente"
                        CssClass="btn btn-primary w-100" OnClick="btnRegistrarCliente_Click" />
                </div>
            </div>
        </div>

        <!-- ==================== BUSCADOR ==================== -->
        <div class="border rounded p-3 mb-4">
            <h4 class="mb-3 text-success">🔍 Buscar clientes</h4>

            <div class="row g-3 align-items-end">
                <div class="col-md-6">
                    <label class="form-label fw-bold">Buscar por nombre, NIT o correo</label>
                    <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control"
                        placeholder="Ej. Pérez, 1234567-8 o cliente@gmail.com"></asp:TextBox>
                </div>

                <div class="col-md-2">
                    <asp:Button ID="btnBuscar" runat="server" Text="Buscar"
                        CssClass="btn btn-success w-100" OnClick="btnBuscar_Click" />
                </div>

                <div class="col-md-2">
                    <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar"
                        CssClass="btn btn-outline-secondary w-100" OnClick="btnLimpiar_Click" />
                </div>
            </div>
        </div>

        <!-- ==================== LISTA DE CLIENTES ==================== -->
        <div class="border rounded p-3">
            <h4 class="mb-3 text-secondary">📋 Clientes registrados</h4>

            <asp:GridView ID="gvClientes" runat="server"
                CssClass="table table-striped table-hover text-center align-middle"
                AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
                OnPageIndexChanging="gvClientes_PageIndexChanging"
                OnRowCommand="gvClientes_RowCommand"
                DataKeyNames="ClienteId">

                <Columns>
                    <asp:BoundField DataField="ClienteId" HeaderText="ID" ReadOnly="True" />
                    <asp:BoundField DataField="Nombre" HeaderText="Nombre completo" />
                    <asp:BoundField DataField="NIT" HeaderText="NIT" />
                    <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                    <asp:BoundField DataField="CorreoElectronico" HeaderText="Correo electrónico" />

                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <asp:Button ID="btnEliminar" runat="server" Text="🗑️ Eliminar"
                                CssClass="btn btn-sm btn-danger"
                                CommandName="Eliminar"
                                CommandArgument='<%# Eval("ClienteId") %>'
                                OnClientClick="return confirm('¿Seguro que deseas eliminar este cliente?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <asp:Label ID="lblMensajeVacio" runat="server"
                Text="No hay clientes registrados aún."
                CssClass="alert alert-info text-center d-block" Visible="false" />
        </div>
    </div>
</asp:Content>
