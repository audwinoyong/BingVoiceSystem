<%@ Page Title="Rules List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DataList.aspx.cs" Inherits="BingVoiceSystem.DataList" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Data List</h2>

    <asp:GridView ID="DataGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="MovieTitle" EmptyDataText="There are no approved rules to display" 
        OnRowEditing="DataGridView_RowEditing" OnRowUpdating="DataGridView_RowUpdating" OnRowCancelingEdit="DataGridView_RowCancelingEdit" 
        OnRowDeleting="DataGridView_RowDeleting">
        <Columns>
            <asp:BoundField DataField="MovieTitle" HeaderText="Movie Title" />
            <asp:BoundField DataField="Genre" HeaderText="Genre" />
            <asp:BoundField DataField="LastEditedBy" HeaderText="Last Edited By" ReadOnly="True" />

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
        <asp:Button ID="AddData" runat="server" Text="Add New Data" OnClick="AddDataButton_Click" />
    </div>
</asp:Content>
