<%@ Page Title="Rules Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditorReport.aspx.cs" Inherits="BingVoiceSystem.EditorReport" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Editor Report</h2>

    <h3>Your Approved Rules</h3>
    <asp:GridView ID="MyRulesGridView" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Question" />
            <asp:BoundField DataField="Value" HeaderText="Answer" />
        </Columns>
    </asp:GridView>

    <h3>Your Rule Statistics</h3>
    <asp:GridView ID="RulesStatisticsGridView" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Statistic" />
            <asp:BoundField DataField="Value" HeaderText="Value" />
        </Columns>
    </asp:GridView>
</asp:Content>

