<%@ Page Title="Rules List Approver" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RulesListApprover.aspx.cs" Inherits="BingVoiceSystem.RulesListApprover" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Rules List Approver</h2>

    <h3>Pending</h3>
    <asp:GridView ID="PendingRulesGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="Key" EmptyDataText="There are no pending rules to display" 
        OnRowCommand="PendingRulesGridView_RowCommand">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Question" />
            <asp:BoundField DataField="Value" HeaderText="Answer" />

            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Button ID="ApproveButton" runat="server" Text="Approve" CommandName="Approve" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                    <asp:Button ID="RejectButton" runat="server" Text="Reject" CommandName="Reject" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </asp:GridView>

    <br />

    <h3>Approved</h3>
    <asp:GridView ID="ApprovedRulesGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="Key" EmptyDataText="There are no approved rules to display">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Question" />
            <asp:BoundField DataField="Value" HeaderText="Answer" />

        </Columns>
    </asp:GridView>

    <h3>Rejected</h3>
    <asp:GridView ID="RejectedRulesGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="Key" EmptyDataText="There are no rejected rules to display">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Question" />
            <asp:BoundField DataField="Value" HeaderText="Answer" />

        </Columns>
    </asp:GridView>

</asp:Content>