<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    CodeBehind="SearchConsoleLayout.aspx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Pages.SearchConsoleLayout" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UILayout runat="server" ID="layoutElem">
        <Panes>
            <cms:UILayoutPane ID="uipTree" runat="server" Direction="West" RenderAs="div"
                ControlPath="~/CMSModules/Kentico.Xperience.Google.SearchConsole/Controls/SearchConsoleContentTree.ascx" SpacingOpen="8"
                SpacingClosed="8" Size="304" TogglerLengthOpen="32" TogglerLengthClosed="32" UseUpdatePanel="true" MinSize="330" />
            <cms:UILayoutPane ID="uipDetails" runat="server" Direction="Center" RenderAs="div" SpacingOpen="8" SpacingClosed="8"
                TogglerLengthOpen="32" TogglerLengthClosed="32" UseUpdatePanel="false" />
        </Panes>
    </cms:UILayout>
</asp:Content>
