#pragma checksum "C:\Users\Kalictong\source\repos\Projet DNS\Views\ProjetDNSLog\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "df8e9fb81e8f3c15fd4349b189730de92e378268"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_ProjetDNSLog_Index), @"mvc.1.0.view", @"/Views/ProjetDNSLog/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\Kalictong\source\repos\Projet DNS\Views\_ViewImports.cshtml"
using Projet_DNS;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\Kalictong\source\repos\Projet DNS\Views\_ViewImports.cshtml"
using Projet_DNS.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"df8e9fb81e8f3c15fd4349b189730de92e378268", @"/Views/ProjetDNSLog/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"13657e64ac81e078d5590d34121781bcaf2df352", @"/Views/_ViewImports.cshtml")]
    public class Views_ProjetDNSLog_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 4 "C:\Users\Kalictong\source\repos\Projet DNS\Views\ProjetDNSLog\Index.cshtml"
  
    ViewData["Title"] = "Log";
    ViewData["Active"] = "projetDnsLog";
    ViewData["Menu"] = "outilsmenu";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<!-- Content Header (Page header) -->
<div class=""content-header"">
    <div class=""container-fluid"">
        <div class=""row my-2"">
            <div class=""col-sm-6"">
                <h1 class=""m-0 text-dark"">Visualisation des dernières lignes du fichier log : projetdns.log</h1>
            </div><!-- /.col -->
        </div><!-- /.row -->
    </div><!-- /.container-fluid -->
</div>
<!-- /.content-header -->
<!-- Main content -->
<section class=""content"">
    <div class=""container-fluid"">
        <div class=""row"">
            <div class=""col-sm-12 mb-2"">
                <p>Ici vous pourrez voir tous les logs qui on été enregistrer depuis le lancement du serveur</p>
            </div>
            <textarea class=""form-control"" style=""min-height: 70vh"" disabled>");
#nullable restore
#line 28 "C:\Users\Kalictong\source\repos\Projet DNS\Views\ProjetDNSLog\Index.cshtml"
                                                                        Write(ViewBag.QueryLogFull);

#line default
#line hidden
#nullable disable
            WriteLiteral("</textarea>\r\n        </div>\r\n    </div>\r\n\r\n</section>\r\n<!-- /.content -->");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591