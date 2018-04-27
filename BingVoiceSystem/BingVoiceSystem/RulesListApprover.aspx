<%@ Page Title="Rules List Approver" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RulesListApprover.aspx.cs" Inherits="BingVoiceSystem.RulesListApprover" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Rules List Approver</h2>

    <h3>Pending</h3>
    <asp:GridView ID="PendingRulesGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="Question" EmptyDataText="There are no pending rules to display" 
        OnRowCommand="PendingRulesGridView_RowCommand">
        <Columns>
            <asp:BoundField DataField="Question" HeaderText="Question" />
            <asp:BoundField DataField="Answer" HeaderText="Answer" />
            <asp:BoundField DataField="CreatedBy" HeaderText="Created By" ReadOnly="True" />
            <asp:BoundField DataField="EditedBy" HeaderText="Edited By" ReadOnly="True" />

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
    <asp:GridView ID="ApprovedRulesGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="Question" EmptyDataText="There are no approved rules to display">
        <Columns>
            <asp:BoundField DataField="Question" HeaderText="Question" />
            <asp:BoundField DataField="Answer" HeaderText="Answer" />
            <asp:BoundField DataField="ApprovedBy" HeaderText="Approved By" ReadOnly="True" />
            <asp:BoundField DataField="EditedBy" HeaderText="Edited By" ReadOnly="True" />

        </Columns>
    </asp:GridView>

    <h3>Rejected</h3>
    <asp:GridView ID="RejectedRulesGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="Question" EmptyDataText="There are no rejected rules to display">
        <Columns>
            <asp:BoundField DataField="Question" HeaderText="Question" />
            <asp:BoundField DataField="Answer" HeaderText="Answer" />
            <asp:BoundField DataField="RejectedBy" HeaderText="Rejected By" ReadOnly="True" />
            <asp:BoundField DataField="EditedBy" HeaderText="Edited By" ReadOnly="True" />

        </Columns>
    </asp:GridView>

</asp:Content>