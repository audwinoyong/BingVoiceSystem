<%@ Page Title="Add Movie Data" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RulesDataAdd.aspx.cs" Inherits="BingVoiceSystem.RulesDataAdd" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Add New Movie Data</h2>
    <table>
        <tr>
            <td>Movie:</td>
            <td>
                <asp:TextBox ID="QuestionTextBox" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="QuestionTextBox" ErrorMessage="Question is required" Text="[Required]" Font-Bold="true" ForeColor="Red" />
            </td>
        </tr>
        <tr>
            <td>Genre:</td>
            <td>
                <asp:TextBox ID="AnswerTextBox" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="AnswerTextBox" ErrorMessage="Answer is required" Text="[Required]" Font-Bold="true" ForeColor="Red" />
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