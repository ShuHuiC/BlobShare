<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<UserViewModel>" %>

<asp:Content ContentPlaceHolderID="TitleContent" runat="server">
    Blob Share | Edit Profile
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <p>Please update your profile:</p>

    <% using (Html.BeginForm())
    { %>
        <div class="form">
            <div class="left-box">
                <div class="editor-label">Name</div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.Name) %>
                    <%: Html.ValidationMessageFor(m => m.Name) %>
                    <%: Html.HiddenFor(m => m.OriginalName) %>
                </div>
            </div>
            <div class="submit clear">
                <input type="submit" value="Update" />
            </div>
        </div>
    <% } %>
</asp:Content>