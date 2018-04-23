<%@ Page Title="Approvers Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ApproverReport.aspx.cs" Inherits="BingVoiceSystem.ApproverReport" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Approvers Report</h2>

    <h3>Editor Statistics</h3>
    <asp:GridView ID="EditorStatisticsGridView" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Editor" />
            <asp:BoundField DataField="Value" HeaderText="Value" />
        </Columns>
    </asp:GridView>
</asp:Content>

