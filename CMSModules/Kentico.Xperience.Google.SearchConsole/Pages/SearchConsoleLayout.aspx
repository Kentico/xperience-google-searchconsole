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
            <asp:Panel ID="pnlActions" runat="server">
                <div style="margin-bottom:40px">
                    <uc:ActionPanel ID="actionPanel" runat="server" />
                </div>
            </asp:Panel>
            <div style="display:inline-flex;width:100%">
                <uc:ContentTree ID="contentTree" runat="server" />
                <div style="min-width:60%;margin-left:70px;padding-bottom:100px">
                    <uc:ConsoleReport ID="consoleReport" runat="server" StopProcessing="true" />
                    <div style="padding-top:10px">
                        <uc:ConsoleDetails ID="consoleDetails" runat="server" StopProcessing="true" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
