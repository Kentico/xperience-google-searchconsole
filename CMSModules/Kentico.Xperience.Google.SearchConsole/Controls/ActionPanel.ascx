<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActionPanel.ascx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Controls.ActionPanel" %>

<asp:Literal ID="ltlMessage" runat="server" EnableViewState="false" Visible="false" />
<div style="margin-top:10px">
    <asp:Button ID="btnIndexSingle" runat="server" CssClass="btn btn-primary" Text="Request page indexing" OnClick="btnIndexSingle_Click" UseSubmitBehavior="false" />
    <asp:Button ID="btnGetSingleStatus" runat="server" CssClass="btn btn-default" Text="Refresh page status" OnClick="btnGetSingleStatus_Click" UseSubmitBehavior="false" />
    <asp:Button ID="btnGetSectionStatus" runat="server" CssClass="btn btn-default" Text="Refresh section status" OnClick="btnGetSectionStatus_Click" UseSubmitBehavior="false" />
</div>