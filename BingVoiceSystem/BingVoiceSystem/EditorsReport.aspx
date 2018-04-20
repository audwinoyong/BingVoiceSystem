<%@ Page Title="Rules Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditorsReport.aspx.cs" Inherits="BingVoiceSystem.EditorsReport" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Rules Report</h2>

    <h3>My Rules</h3>
    <asp:GridView ID="MyRulesGridView" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Question" />
            <asp:BoundField DataField="Value" HeaderText="Answer" />
        </Columns>
    </asp:GridView>
</asp:Content>

