<%@ Page Language="C#" AutoEventWireup="true" Title="Google authentication" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeBehind="OAuthAuthorizationFinished.aspx.cs"
    Inherits="Kentico.Xperience.Google.SearchConsole.Pages.OAuthAuthorizationFinished" Theme="Default" %>

<asp:Content ID="cntMain" ContentPlaceHolderID="plcContent" runat="Server" EnableViewState="false">
    <asp:Panel ID="pnlSuccess" runat="server">
        <cms:LocalizedHeading runat="server" Level="3" Text="Authentication successful" />
        <p>Google authentication was successful. Please close this window and refresh the previous page.</p>
    </asp:Panel>
    <asp:Panel ID="pnlError" runat="server" Visible="false">
        <cms:LocalizedHeading runat="server" Level="3" Text="Authentication failed" />
        <p><asp:Literal ID="ltlErrorMessage" runat="server" /></p>
    </asp:Panel>
</asp:Content>
