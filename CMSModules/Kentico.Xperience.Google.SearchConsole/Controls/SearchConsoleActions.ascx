<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchConsoleActions.ascx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Controls.SearchConsoleActions" %>

<div class="mass-action">
    <div class="mass-action-value-cell">
        <cms:CMSDropDownList ID="drpMode" runat="server" />
        <cms:CMSDropDownList ID="drpAction" runat="server" />
        <asp:Button ID="btnSubmit" runat="server" Text="Submit" EnableViewState="false" ButtonStyle="Default" class="btn btn-default" OnClick="Submit" />
    </div>
</div>
