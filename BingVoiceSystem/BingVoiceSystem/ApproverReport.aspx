<%@ Page Title="Approvers Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ApproverReport.aspx.cs" Inherits="BingVoiceSystem.ApproverReport" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Approvers Report</h2>

    <h3>Editor Statistics</h3>
    <asp:GridView ID="EditorStatisticsGridView" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Editor" HeaderText="Editor" />
            <asp:BoundField DataField="Approved Count" HeaderText="Approved Count" />
            <asp:BoundField DataField="Rejected Count" HeaderText="Rejected Count" />
            <asp:BoundField DataField="Pending Count" HeaderText="Pending Count" />
            <asp:BoundField DataField="Success Rate" HeaderText="Success Rate" />
            <asp:BoundField DataField="Average Success Rate" HeaderText="Average Success Rate" />
        </Columns>
    </asp:GridView>
</asp:Content>

