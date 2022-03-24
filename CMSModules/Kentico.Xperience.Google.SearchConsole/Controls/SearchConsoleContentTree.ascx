<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchConsoleContentTree.ascx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Controls.SearchConsoleContentTree" %>
<%@ Register TagPrefix="uc" TagName="SiteCultureSelector" Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" %>
<%@ Register TagPrefix="uc" TagName="ContentTree" Src="~/CMSModules/Content/Controls/ContentTree.ascx" %>

<div style="padding:30px 20px">
    <asp:Panel ID="pnlMain" runat="server">
        <div class="btn-group" style="margin-bottom:30px">
            <asp:Button ID="btnModeOverview" runat="server" OnClientClick="modeSelected(0);" Text="Overview" CssClass="btn btn-default" />
            <asp:Button ID="btnModeReport" runat="server" OnClientClick="modeSelected(1);" Text="Report" CssClass="btn btn-default" />
        </div>
        <uc:ContentTree ID="contentTree" runat="server" IsLiveSite="false" AllowDragAndDrop="false" AllowMarks="false" />
        <div style="margin-top:30px">
            <uc:SiteCultureSelector ID="drpCulture" runat="server" PostbackOnChange="false" />
        </div>
    </asp:Panel>
</div>