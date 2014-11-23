<%@ Page Language="C#" %>
<%@ Import Namespace="Common" %>
<script runat="server">

    protected override void OnLoad(EventArgs e)
    {
        string redirect = ConfigurationManager.AppSettings[@"DefaultHomeRedirect"];
        if (redirect.HasValue())
        {
            Response.Redirect(redirect);
            base.OnLoad(e);
        }
    }

</script>