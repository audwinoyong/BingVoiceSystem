<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BingVoiceSystem._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-4">
            <h2>Q&A</h2>
            <p>
                Enter a question and get your answer below!
            </p>
            <p>
                <asp:TextBox ID="questionTb" runat="server" Width="400px"></asp:TextBox>
                <br/>
                <br/>
                <asp:Button ID="submitBtn" runat="server" Text="Submit" OnClick="SubmitBtn_Click" Height="38px" Width="155px" />
                <br/>
                <br/>
                <asp:Label ID="answerLbl" runat="server" Font-Size="Large" Visible="false"></asp:Label>

            </p>
        </div>
    </div>

</asp:Content>
