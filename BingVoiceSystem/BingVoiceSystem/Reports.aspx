<%@ Page Title="Reports" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="BingVoiceSystem.Reports" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Generate Reports:</h2>
    <asp:Button ID="rulesReportBtn" runat="server" Text="Rules Report" OnClick="RulesReportBtn_Click" Height="38px" Width="155px" Visible="True" />
    </br>
    </br>
    <asp:Button ID="editorReportBtn" runat="server" Text="Editor Report" OnClick="EditorReportBtn_Click" Height="38px" Width="155px" Visible="True" />
    </br>
    </br>
    <asp:Button ID="approverReportBtn" runat="server" Text="Rules Report" OnClick="ApproverReportBtn_Click" Height="38px" Width="155px" Visible="True" />
    </br>
    </br>
    <asp:Label ID="permissionsLbl" runat="server" Text="No reports to generate" Visible="False" />
</asp:Content>

