<%@ Page Language="C#" Theme="Default" AutoEventWireup="true" CodeBehind="SearchConsolePropertyPage.aspx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Pages.SearchConsolePropertyPage" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>
<%@ Register TagPrefix="uc" TagName="ContentTree" Src="~/CMSModules/Content/Controls/ContentTree.ascx" %>
<%@ Register TagPrefix="uc" TagName="OAuthButton" Src="~/CMSModules/Kentico.Xperience.Google.SearchConsole/Controls/OAuthButton.ascx" %>
<%@ Register TagPrefix="uc" TagName="RequestUpdateButton" Src="~/CMSModules/Kentico.Xperience.Google.SearchConsole/Controls/RequestUpdateButton.ascx" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:HiddenField ID="hidSelectedNode" runat="server" Value="<%# NodeID %>" />
    <uc:OAuthButton ID="btnAuth" runat="server" />
    <asp:Panel ID="pnlMain" runat="server">
        <div class="form-horizontal" style="padding:20px 10px">
            <div class="form-group">
                <uc:RequestUpdateButton ID="btnRequestNode" runat="server" IsSingleNode="true" />
                <uc:RequestUpdateButton ID="btnRequestSection" runat="server" IsSection="true" />
                <uc:RequestUpdateButton ID="btnRequestTree" runat="server" IsContentTree="true" />
            </div>
            <div class="form-group" style="margin-top:30px;">
                <uc:ContentTree ID="contentTree" runat="server" IsLiveSite="false" AllowDragAndDrop="false" AllowMarks="false" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>