<%@ Page Title="Rules List Editor" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RulesListEditor.aspx.cs" Inherits="BingVoiceSystem.RulesListEditor" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Rules List Editor</h2>

    <h3>Pending</h3>
    <asp:GridView ID="PendingRulesGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="Question" EmptyDataText="There are no pending rules to display" 
        OnRowEditing="PendingRulesGridView_RowEditing" OnRowUpdating="PendingRulesGridView_RowUpdating" OnRowCancelingEdit="PendingRulesGridView_RowCancelingEdit" 
        OnRowDeleting="PendingRulesGridView_RowDeleting">
        <Columns>
            <asp:BoundField DataField="Question" HeaderText="Question" />
            <asp:BoundField DataField="Answer" HeaderText="Answer" />
            <asp:BoundField DataField="CreatedBy" HeaderText="Created By" ReadOnly="True" />
            <asp:BoundField DataField="EditedBy" HeaderText="Edited By" ReadOnly="True" />

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
    <asp:GridView ID="ApprovedRulesGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="Question" EmptyDataText="There are no approved rules to display" 
        OnRowEditing="ApprovedRulesGridView_RowEditing" OnRowUpdating="ApprovedRulesGridView_RowUpdating" OnRowCancelingEdit="ApprovedRulesGridView_RowCancelingEdit" 
        OnRowDeleting="ApprovedRulesGridView_RowDeleting">
        <Columns>
            <asp:BoundField DataField="Question" HeaderText="Question" />
            <asp:BoundField DataField="Answer" HeaderText="Answer" />
            <asp:BoundField DataField="ApprovedBy" HeaderText="Approved By" ReadOnly="True" />

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

    <h3>Rejected</h3>
    <asp:GridView ID="RejectedRulesGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="Question" EmptyDataText="There are no rejected rules to display" 
        OnRowEditing="RejectedRulesGridView_RowEditing" OnRowUpdating="RejectedRulesGridView_RowUpdating" OnRowCancelingEdit="RejectedRulesGridView_RowCancelingEdit" 
        OnRowDeleting="RejectedRulesGridView_RowDeleting">
        <Columns>
            <asp:BoundField DataField="Question" HeaderText="Question" />
            <asp:BoundField DataField="Answer" HeaderText="Answer" />
            <asp:BoundField DataField="RejectedBy" HeaderText="Rejected By" ReadOnly="True" />

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

</asp:Content>
