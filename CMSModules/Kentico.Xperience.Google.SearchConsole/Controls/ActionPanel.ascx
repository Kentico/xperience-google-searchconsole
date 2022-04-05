<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActionPanel.ascx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Controls.ActionPanel" %>

<div class="form-inline">
    <div class="form-group" style="margin-right:10px">
        <div style="margin-bottom:10px">
            <asp:Button ID="btnIndexSingle" runat="server" CssClass="btn btn-primary" Text="Request page indexing" ToolTip="Send a request for Google to index this page" OnClick="btnIndexSingle_Click" />
        </div>
        <div>
            <asp:Button ID="btnIndexSection" runat="server" CssClass="btn btn-primary" Text="Request section indexing" ToolTip="Send a request for Google to index the children of this page" OnClick="btnIndexSection_Click" />
        </div>
    </div>
    <div class="form-group">
        <div style="margin-bottom:10px">
            <asp:Button ID="btnGetSingleStatus" runat="server" CssClass="btn btn-default" Text="Refresh page status" ToolTip="Get the page's indexed status from Google Search Console" OnClick="btnGetSingleStatus_Click" />
        </div>
        <div>
            <asp:Button ID="btnGetSectionStatus" runat="server" CssClass="btn btn-default" Text="Refresh section status" ToolTip="Get the indexed status of the children of this page from Google Search Console" OnClick="btnGetSectionStatus_Click" />
        </div>
    </div>
</div>