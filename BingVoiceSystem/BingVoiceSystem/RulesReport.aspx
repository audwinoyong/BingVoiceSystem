<%@ Page Title="Rules Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RulesReport.aspx.cs" Inherits="BingVoiceSystem.RulesReport" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Rules Report</h2>

    <h3>Approved Rules</h3>
    <asp:GridView ID="ApprovedRulesGridView" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Question" />
            <asp:BoundField DataField="Value" HeaderText="Answer" />
        </Columns>
    </asp:GridView>

    <h3>Rule Statistics</h3>
    <asp:GridView ID="RulesStatisticsGridView" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Statistic" />
            <asp:BoundField DataField="Value" HeaderText="Value" />
        </Columns>
    </asp:GridView>
</asp:Content>

