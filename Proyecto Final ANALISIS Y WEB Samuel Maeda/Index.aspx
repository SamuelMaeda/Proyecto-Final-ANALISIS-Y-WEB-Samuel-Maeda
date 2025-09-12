<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Proyecto_Final_ANALISIS_Y_WEB_Samuel_Maeda.Index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card shadow p-4 text-center">
        <h1 class="mb-3">
            Bienvenido, <asp:Label ID="lblUsuario" runat="server" Text=""></asp:Label>
        </h1>

        <p class="mb-4">Has iniciado sesión correctamente.</p>

        <div class="d-flex justify-content-center gap-3">
            <!-- Botón para ir al registro de libros -->
            <asp:Button ID="btnLibros" runat="server" Text="Ir al Registro de Libros"
                CssClass="btn btn-primary"
                PostBackUrl="~/BookRegister.aspx" />

            <!-- Botón para cerrar sesión -->
            <asp:Button ID="btnLogout" runat="server" Text="Cerrar Sesión"
                CssClass="btn btn-danger"
                OnClick="btnLogout_Click" />
        </div>
    </div>
</asp:Content>
