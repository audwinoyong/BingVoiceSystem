<%@ Page Title="Rules List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RulesList.aspx.cs" Inherits="BingVoiceSystem.RulesList" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Rules List</h2>

    <h3>Pending Rules</h3>
    <asp:GridView ID="PendingRulesGridView" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Question" />
            <asp:BoundField DataField="Value" HeaderText="Answer" />
        </Columns>
    </asp:GridView>

    <h3>Approved Rules</h3>
    <asp:GridView ID="ApprovedRulesGridView" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Question" />
            <asp:BoundField DataField="Value" HeaderText="Answer" />
        </Columns>
    </asp:GridView>

    <h3>Rejected Rules</h3>
    <asp:GridView ID="RejectedRulesGridView" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Question" />
            <asp:BoundField DataField="Value" HeaderText="Answer" />
        </Columns>
    </asp:GridView>

</asp:Content>
