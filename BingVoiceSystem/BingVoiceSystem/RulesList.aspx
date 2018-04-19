<%@ Page Title="Rules List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RulesList.aspx.cs" Inherits="BingVoiceSystem.RulesList" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Rules List</h2>

    <h3>Pending</h3>
    <asp:GridView ID="PendingRulesGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="Key"
        OnRowEditing="PendingRulesGridView_RowEditing" OnRowUpdating="PendingRulesGridView_RowUpdating" OnRowCancelingEdit="PendingRulesGridView_RowCancelingEdit" OnRowDeleting="PendingRulesGridView_RowDeleting">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Question" />
            <asp:BoundField DataField="Value" HeaderText="Answer" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Button ID="EditButton" runat="server" Text="Edit" CommandName="Edit" />
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:Button ID="UpdateButton" runat="server" Text="Update" CommandName="Update" />
                    <asp:Button ID="CancelButton" runat="server" Text="Cancel" CommandName="Cancel" />
                </EditItemTemplate>
            </asp:TemplateField>

            <asp:ButtonField ButtonType="Button" Text="Delete" CommandName="Delete" />
        </Columns>
    </asp:GridView>

    <br />
    <div>
        <asp:Button ID="AddRule" runat="server" Text="Add New Rule" OnClick="AddRuleButton_Click" />
    </div>

    <h3>Approved</h3>
    <asp:GridView ID="ApprovedRulesGridView" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Question" />
            <asp:BoundField DataField="Value" HeaderText="Answer" />
        </Columns>
    </asp:GridView>

    <h3>Rejected</h3>
    <asp:GridView ID="RejectedRulesGridView" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Key" HeaderText="Question" />
            <asp:BoundField DataField="Value" HeaderText="Answer" />
        </Columns>
    </asp:GridView>

</asp:Content>
