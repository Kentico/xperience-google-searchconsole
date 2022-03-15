<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchConsoleContentTree.ascx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Controls.SearchConsoleContentTree" %>
<%@ Register TagPrefix="uc" TagName="SiteCultureSelector" Src="~/CMSFormControls/Cultures/SiteCultureSelector.ascx" %>
<%@ Register TagPrefix="uc" TagName="ContentTree" Src="~/CMSModules/Content/Controls/ContentTree.ascx" %>

<div style="padding:30px 20px">
    <asp:Panel ID="pnlMain" runat="server">
        <uc:ContentTree ID="contentTree" runat="server" IsLiveSite="false" AllowDragAndDrop="false" AllowMarks="false" />
        <div style="margin-top:30px">
            <uc:SiteCultureSelector ID="drpCulture" runat="server" PostbackOnChange="true" />
        </div>
    </asp:Panel>
</div>