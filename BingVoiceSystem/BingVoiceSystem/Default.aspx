<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BingVoiceSystem._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-4">
            <h2>Q&A</h2>
            <p>
                Enter a question and get your answer below!
            </p>
                <div runat="server" ID="chat" style="overflow-y:auto; height:200px; width:500px; border: 1px solid black" />
                <asp:TextBox CssClass="question" ID="questionTb" runat="server" style="width:500px"></asp:TextBox>
                <br/>
                <br/>
                <asp:Button ID="submitBtn" runat="server" Text="Submit" OnClick="SubmitBtn_Click" Height="38px" Width="155px" />
                <br/>
                <br/>
        </div>
    </div>

    <%--Scrolls the chat to the bottom at each new entry--%>
    <%--Found at: https://stackoverflow.com/questions/270612/scroll-to-bottom-of-div--%>
    <script type="text/javascript">
        window.onload = function () {
            document.getElementById('<%=chat.ClientID%>').scrollTop = document.getElementById('<%=chat.ClientID%>').scrollHeight;
        };
    </script>
</asp:Content>
