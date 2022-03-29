<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActionPanel.ascx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Controls.ActionPanel" %>

<asp:Button ID="btnIndexSingle" runat="server" CssClass="btn btn-primary" Text="Request page indexing" OnClick="btnIndexSingle_Click" />
<asp:Button ID="btnGetSingleStatus" runat="server" CssClass="btn btn-default" Text="Refresh page status" OnClick="btnGetSingleStatus_Click" />
<asp:Button ID="btnGetSectionStatus" runat="server" CssClass="btn btn-default" Text="Refresh section status" OnClick="btnGetSectionStatus_Click" />