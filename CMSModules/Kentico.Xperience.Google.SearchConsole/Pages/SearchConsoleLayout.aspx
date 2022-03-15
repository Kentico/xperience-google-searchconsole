<%@ Page Language="C#" AutoEventWireup="true" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeBehind="SearchConsoleLayout.aspx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Pages.SearchConsoleLayout" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UILayout runat="server" ID="layoutElem">
        <Panes>
            <cms:UILayoutPane ID="uipTree" runat="server" Direction="West" RenderAs="div"
                ControlPath="~/CMSModules/Kentico.Xperience.Google.SearchConsole/Controls/SearchConsoleContentTree.ascx" SpacingOpen="8"
                SpacingClosed="8" PaneClass="" Size="304" TogglerLengthOpen="32" TogglerLengthClosed="32" UseUpdatePanel="true" MinSize="400" />
            <cms:UILayoutPane ID="UILayoutPane1" runat="server" Direction="Center" RenderAs="div"
                ControlPath="~/CMSModules/Kentico.Xperience.Google.SearchConsole/Controls/SearchConsolePageDetails.ascx" SpacingOpen="8"
                SpacingClosed="8" Size="304" TogglerLengthOpen="32" TogglerLengthClosed="32" UseUpdatePanel="true" />
        </Panes>
    </cms:UILayout>
</asp:Content>
