<%@ Page Title="Create Rule" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DataListAdd.aspx.cs" Inherits="BingVoiceSystem.DataListAdd" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Add Data</h2>
    <table>
        <tr>
            <td>Movie Title:</td>
            <td>
                <asp:TextBox ID="MovieTextBox" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="MovieTextBox" ErrorMessage="Movie Title is required" Text="[Required]" Font-Bold="true" ForeColor="Red" />
            </td>
        </tr>
        <tr>
            <td>Genre:</td>
            <td>
                <asp:TextBox ID="GenreTextBox" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="GenreTextBox" ErrorMessage="Genre is required" Text="[Required]" Font-Bold="true" ForeColor="Red" />
            </td>
        </tr>
    </table>

    <br />
    <asp:Label ID="DataAdded" runat="server" ForeColor="Red"></asp:Label>
    <br />

    <br />
    <asp:ValidationSummary runat="server" />
    <div>
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
        <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
    </div>
</asp:Content>
