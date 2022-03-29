<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportFilter.ascx.cs" Inherits="Kentico.Xperience.Google.SearchConsole.Controls.ReportFilter" %>

<div class="form-horizontal form-filter">
    <div class="form-group">
        <div class="filter-form-label-cell">
            <asp:Label runat="server" Text="Show" CssClass="control-label editing-form-label" />
        </div>
        <div class="filter-form-value-cell">
            <cms:CMSDropDownList ID="drpShowErrors" runat="server" />
        </div>
    </div>
</div>