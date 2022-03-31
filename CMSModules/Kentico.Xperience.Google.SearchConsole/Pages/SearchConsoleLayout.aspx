<%@ Page Language="C#" AutoEventWireup="true" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    CodeBehind="SearchConsoleLayout.aspx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Pages.SearchConsoleLayout" %>

<%@ Register TagPrefix="uc" TagName="ContentTree" Src="~/CMSModules/Kentico.Xperience.Google.SearchConsole/Controls/SearchConsoleContentTree.ascx" %>
<%@ Register TagPrefix="uc" TagName="ConsoleReport" Src="~/CMSModules/Kentico.Xperience.Google.SearchConsole/Controls/SearchConsoleReport.ascx" %>
<%@ Register TagPrefix="uc" TagName="ConsoleDetails" Src="~/CMSModules/Kentico.Xperience.Google.SearchConsole/Controls/SearchConsolePageDetails.ascx" %>
<%@ Register TagPrefix="uc" TagName="ActionPanel" Src="~/CMSModules/Kentico.Xperience.Google.SearchConsole/Controls/ActionPanel.ascx" %>
<%@ Register TagPrefix="uc" TagName="OAuthButton" Src="~/CMSModules/Kentico.Xperience.Google.SearchConsole/Controls/OAuthButton.ascx" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <uc:OAuthButton ID="btnAuth" runat="server" Visible="false" />
    <div style="padding:30px">
        <asp:Panel ID="pnlMain" runat="server">
            <div style="max-width:16%" class="pull-left">
                <uc:ContentTree ID="contentTree" runat="server" />
            </div>
            <div style="min-width:50%;max-width:81%;margin-left:70px;padding-bottom:100px" class="pull-left">
                <div id="messageContainer" runat="server" visible="false" style="margin-bottom:15px" />
                <asp:Panel ID="pnlActions" runat="server">
                    <div style="margin-bottom:40px">
                        <uc:ActionPanel ID="actionPanel" runat="server" />
                    </div>
                </asp:Panel>
                <uc:ConsoleReport ID="consoleReport" runat="server" StopProcessing="true" Visible="false" />
                <uc:ConsoleDetails ID="consoleDetails" runat="server" StopProcessing="true" Visible="false" />
            </div>
        </asp:Panel>
    </div>
</asp:Content>
