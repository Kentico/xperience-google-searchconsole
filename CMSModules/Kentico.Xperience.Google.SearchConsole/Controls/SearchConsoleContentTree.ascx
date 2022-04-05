<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchConsoleContentTree.ascx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Controls.SearchConsoleContentTree" %>
<%@ Register TagPrefix="uc" TagName="SiteCultureSelector" Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" %>
<%@ Register TagPrefix="uc" TagName="ContentTree" Src="~/CMSModules/Content/Controls/ContentTree.ascx" %>

<asp:Panel ID="pnlMain" runat="server">
    <uc:ContentTree ID="contentTree" runat="server" IsLiveSite="false" AllowDragAndDrop="false" AllowMarks="false" />
    <div style="margin-top:30px">
        <uc:SiteCultureSelector ID="drpCulture" runat="server" PostbackOnChange="false" />
    </div>
</asp:Panel>