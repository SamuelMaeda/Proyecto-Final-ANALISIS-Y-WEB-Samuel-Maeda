<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestionEmpleados.aspx.cs"
    Inherits="Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda.GestionEmpleados" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow p-4">
        <h2 class="text-center mb-4">👔 Gestión de Empleados</h2>

        <!-- ==================== AGREGAR EMPLEADO ==================== -->
        <div class="border rounded p-3 mb-4 bg-light">
            <h4 class="mb-3 text-primary">➕ Agregar nuevo empleado</h4>

            <div class="row g-3 align-items-end">
                <div class="col-md-3">
                    <label class="form-label fw-bold">Usuario</label>
                    <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" placeholder="Nombre de usuario"></asp:TextBox>
                </div>

                <div class="col-md-3">
                    <label class="form-label fw-bold">Correo</label>
                    <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" placeholder="Correo electrónico"></asp:TextBox>
                </div>

                <div class="col-md-2">
                    <label class="form-label fw-bold">Rol</label>
                    <asp:DropDownList ID="ddlRol" runat="server" CssClass="form-select">
                        <asp:ListItem Text="-- Rol --" Value="" />
                        <asp:ListItem Text="Gerente" Value="Gerente" />
                        <asp:ListItem Text="Cajero" Value="Cajero" />
                    </asp:DropDownList>
                </div>

                <div class="col-md-2">
                    <label class="form-label fw-bold">Contraseña</label>
                    <asp:TextBox ID="txtContrasenia" runat="server" CssClass="form-control" TextMode="Password" placeholder="Contraseña temporal"></asp:TextBox>
                </div>

                <div class="col-md-2">
                    <asp:Button ID="btnAgregar" runat="server" Text="Agregar empleado" CssClass="btn btn-primary w-100" OnClick="btnAgregar_Click" />
                </div>
            </div>
        </div>

        <!-- ==================== BUSCADOR ==================== -->
        <div class="border rounded p-3 mb-4">
            <h4 class="mb-3 text-success">🔍 Buscar empleados</h4>

            <div class="row g-3 align-items-end">
                <div class="col-md-4">
                    <label class="form-label fw-bold">Buscar por usuario o correo</label>
                    <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control" placeholder="Ej. Juan o juan@email.com"></asp:TextBox>
                </div>

                <div class="col-md-3">
                    <label class="form-label fw-bold">Filtrar por rol</label>
                    <asp:DropDownList ID="ddlFiltroRol" runat="server" CssClass="form-select">
                        <asp:ListItem Text="-- Todos --" Value="" />
                        <asp:ListItem Text="Gerente" Value="Gerente" />
                        <asp:ListItem Text="Cajero" Value="Cajero" />
                    </asp:DropDownList>
                </div>

                <div class="col-md-2">
                    <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-success w-100" OnClick="btnBuscar_Click" />
                </div>

                <div class="col-md-2">
                    <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar filtros" CssClass="btn btn-outline-secondary w-100" OnClick="btnLimpiar_Click" />
                </div>
            </div>
        </div>

        <!-- ==================== LISTA DE EMPLEADOS ==================== -->
        <div class="border rounded p-3">
            <h4 class="mb-3 text-secondary">📋 Empleados registrados</h4>

            <asp:GridView ID="gvEmpleados" runat="server" CssClass="table table-striped table-hover text-center align-middle"
                AutoGenerateColumns="False" DataKeyNames="id" AllowPaging="true" PageSize="10"
                OnPageIndexChanging="gvEmpleados_PageIndexChanging" OnRowCommand="gvEmpleados_RowCommand">
                <Columns>
                    <asp:BoundField DataField="id" HeaderText="ID" ReadOnly="True" />
                    <asp:BoundField DataField="Usuario" HeaderText="Usuario" />
                    <asp:BoundField DataField="Correo" HeaderText="Correo" />
                    <asp:BoundField DataField="Rol" HeaderText="Rol" />
                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <asp:Button ID="btnEliminar" runat="server" Text="🗑️ Eliminar"
                                CssClass="btn btn-sm btn-danger"
                                CommandName="Eliminar"
                                CommandArgument='<%# Eval("id") %>'
                                OnClientClick="return confirm('¿Seguro que deseas eliminar este usuario?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
