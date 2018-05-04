<%@ Page Title="Create Rule" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MovieRulesList.aspx.cs" Inherits="BingVoiceSystem.MovieRulesList" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Add New Rule</h2>
    <table>
        <tr>
            <td>Question:</td>
            <td>
                <asp:TextBox ID="QuestionTextBox" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="QuestionTextBox" ErrorMessage="Question is required" Text="[Required]" Font-Bold="true" ForeColor="Red" />
            </td>
        </tr>
        <tr>
            <td>Find:</td>
            <td>
                <asp:DropDownList ID="LookupDropDown" runat="server" />
                <asp:DropdownList ID="AnswerDropDown" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="LookupDropDown" ErrorMessage="Answer is required" Text="[Required]" Font-Bold="true" ForeColor="Red" />
            </td>
        </tr>
        <tr>
            <td>Response:</td>
            <td>
                <asp:TextBox ID="ResponseTextBox" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="QuestionTextBox" ErrorMessage="Question is required" Text="[Required]" Font-Bold="true" ForeColor="Red" />
            </td>
        </tr>
    </table>

    <br />
    <asp:Label ID="RuleAdded" runat="server" ForeColor="Red"></asp:Label>
    <br />

    <br />
    <asp:ValidationSummary runat="server" />
    <div>
        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
        <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
    </div>
</asp:Content>

