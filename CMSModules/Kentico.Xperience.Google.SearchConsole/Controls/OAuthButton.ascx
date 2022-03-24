<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OAuthButton.ascx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Controls.OAuthButton" %>

<asp:Panel ID="pnlMain" runat="server">
    <div class="form-form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Literal Text="Google access token not found. Please click here to re-authorize." runat="server" />
            </div>
            <div class="editing-form-value-cell">
                <asp:Button ID="btnAuth" runat="server" EnableViewState="false" CssClass="btn btn-default" Text="Authorize" />
            </div>
        </div>
    </div>
</asp:Panel>