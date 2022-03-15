<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchConsoleContentTree.ascx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Controls.SearchConsoleContentTree" %>

<%@ Register TagPrefix="uc" TagName="ContentTree" Src="~/CMSModules/Content/Controls/ContentTree.ascx" %>

<div style="padding:30px 15px">
    <asp:Panel ID="pnlMain" runat="server">
        <uc:ContentTree ID="contentTree" runat="server" IsLiveSite="false" AllowDragAndDrop="false" AllowMarks="false" />
    </asp:Panel>
</div>