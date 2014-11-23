using System;
using System.Linq;
using ServiceStack;
using ServiceStack.Host.Handlers;
using ServiceStack.IO;
using ServiceStack.Web;

namespace Common.Services
{
    /// <summary>
    ///     This wrapper class attempts to alter the 'SwaggerFeature' class and replace its plugin registration with a custom
    ///     one.
    /// </summary>
    /// <remarks>
    ///     By default, the ServiceStack.Api.SwaggerFeature presumes that swagger-ui components are located in the
    ///     '/swagger-ui' directory of the service.
    ///     In our case we have relocated them into the 'docs' folder to make more sense to a person visiting the site.
    ///     This wrapper redirects the feature to that directory.
    /// </remarks>
    public class SwaggerFeatureWrapper : IPlugin
    {
        private readonly string directory;
        private readonly IPlugin plugIn;

        /// <summary>
        ///     Creates a new instance of the <see cref="SwaggerFeatureWrapper" /> class.
        /// </summary>
        public SwaggerFeatureWrapper(IPlugin plugIn, string directory)
        {
            Guard.NotNull(() => plugIn, plugIn);
            Guard.NotNullOrEmpty(() => directory, directory);

            this.plugIn = plugIn;
            this.directory = directory;
        }

        /// <summary>
        ///     Registers the <see cref="IPlugin" />
        /// </summary>
        public void Register(IAppHost appHost)
        {
            plugIn.Register(appHost);

            // Replace previous plugin link
            appHost.GetPlugin<MetadataFeature>().PluginLinks.Remove("swagger-ui/");
            string newPluginPath = "{0}/".FormatWith(directory);
            appHost.GetPlugin<MetadataFeature>().AddPluginLink(newPluginPath, "Swagger UI");
            appHost.CatchAllHandlers.Add(delegate(string httpMethod, string pathInfo, string filePath)
            {
                var supportedPaths = new[]
                {
                    "/{0}".FormatWith(directory),
                    "/{0}/".FormatWith(directory),
                    "/{0}/default.html".FormatWith(directory)
                };
                string newPath = "/{0}/index.html".FormatWith(directory);

                if (supportedPaths.Contains(pathInfo, StringComparer.OrdinalIgnoreCase))
                {
                    IVirtualFile file = appHost.VirtualPathProvider.GetFile(newPath);
                    if (file != null)
                    {
                        string html = file.ReadAllText();
                        return new CustomResponseHandler(delegate(IRequest req, IResponse res)
                        {
                            res.ContentType = MimeTypes.Html;
                            string newValue = req.ResolveAbsoluteUrl(@"~/resources");
                            html = html.Replace("http://petstore.swagger.wordnik.com/api/api-docs", newValue);
                            return html;
                        }, null);
                    }
                }
                return null;
            });
        }
    }
}