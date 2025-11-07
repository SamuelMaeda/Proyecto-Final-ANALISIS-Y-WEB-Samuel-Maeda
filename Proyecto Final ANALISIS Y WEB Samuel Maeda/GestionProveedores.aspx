<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestionProveedores.aspx.cs"
    Inherits="Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda.GestionProveedores" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow p-4">
        <h2 class="text-center mb-4">🏢 Gestión de Proveedores</h2>

        <!-- ==================== REGISTRAR PROVEEDOR ==================== -->
        <div class="border rounded p-3 mb-4 bg-light">
            <h4 class="mb-3 text-primary">➕ Registrar nuevo proveedor</h4>

            <div class="row g-3 align-items-end">
                <div class="col-md-5">
                    <label class="form-label fw-bold">Nombre del proveedor</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" placeholder="Ej. Distribuidora Universal"></asp:TextBox>
                </div>

                <div class="col-md-5">
                    <label class="form-label fw-bold">Contacto (teléfono o persona)</label>
                    <asp:TextBox ID="txtContacto" runat="server" CssClass="form-control" placeholder="Ej. 5555-5555 o Juan Pérez"></asp:TextBox>
                </div>

                <div class="col-md-2 d-flex align-items-end">
                    <asp:Button ID="btnGuardar" runat="server" Text="Registrar proveedor"
                        CssClass="btn btn-success w-100" OnClick="btnGuardar_Click" />
                </div>
            </div>
        </div>

        <!-- ==================== BUSCADOR ==================== -->
        <div class="border rounded p-3 mb-4">
            <h4 class="mb-3 text-success">🔍 Buscar proveedores</h4>

            <div class="row g-3 align-items-end">
                <div class="col-md-6">
                    <label class="form-label fw-bold">Buscar por nombre o contacto</label>
                    <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" placeholder="Ej. Universal o Juan"></asp:TextBox>
                </div>

                <div class="col-md-2">
                    <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-success w-100" OnClick="btnBuscar_Click" />
                </div>

                <div class="col-md-2">
                    <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="btn btn-outline-secondary w-100" OnClick="btnLimpiar_Click" />
                </div>
            </div>
        </div>

        <!-- ==================== LISTA DE PROVEEDORES ==================== -->
        <div class="border rounded p-3">
            <h4 class="mb-3 text-secondary">📋 Proveedores registrados</h4>

            <asp:GridView ID="gvProveedores" runat="server"
                CssClass="table table-striped table-hover text-center align-middle"
                AutoGenerateColumns="False" DataKeyNames="ProveedorId"
                AllowPaging="true" PageSize="10"
                OnPageIndexChanging="gvProveedores_PageIndexChanging"
                OnRowCommand="gvProveedores_RowCommand">

                <Columns>
                    <asp:BoundField DataField="ProveedorId" HeaderText="ID" ReadOnly="True" />
                    <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                    <asp:BoundField DataField="Contacto" HeaderText="Contacto" />

                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <asp:Button ID="btnEliminar" runat="server" Text="🗑️ Eliminar"
                                CssClass="btn btn-sm btn-danger"
                                CommandName="Eliminar"
                                CommandArgument='<%# Eval("ProveedorId") %>'
                                OnClientClick="return confirm('¿Seguro que deseas eliminar este proveedor?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
