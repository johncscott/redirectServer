using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using umbraco;
using Umbraco;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Archetype.Models;
using Newtonsoft.Json;

namespace tg.redirectServer
{
    public class FindRedirect : IContentFinder
    {
        public bool TryFindContent(PublishedContentRequest contentRequest)
        {
            try
            {
                if (contentRequest != null)
                {
                    var rootNodes = contentRequest.RoutingContext.UmbracoContext.ContentCache.GetAtRoot();
                    var host = contentRequest.Uri.Host.ToLower();
                    var rootnodeList = rootNodes.Select(r => new { r.Name, r.Id }).ToList();
                    int? nid = rootnodeList.Where(w=> w.Name.ToLower().Contains(host)).Select(i => i.Id).FirstOrDefault();
                    //if no results for host request return false and continue to next PublishedContentRequest operator in the pipeline
                    if (nid == null) { return false; }
                    else
                    {
                        IContent c = ApplicationContext.Current.Services.ContentService.GetById((int)nid);
                        if (c.HasProperty("redirectURL"))
                        {
                            var urlList = JsonConvert.DeserializeObject<UrlListModel>(c.GetValue<string>("redirectURL"));
                            var k = new LookupRedirect(urlList);
                            string dest;
                            if (k.Lookup.TryGetValue(contentRequest.Uri.AbsoluteUri, out dest))
                            {
                                contentRequest.SetRedirectPermanent(dest);
                            }
                        }
                        
                    }

                               }
            }
            catch (Exception ex)
            {
            }

            return contentRequest.PublishedContent != null;
        }
    }

        public class UmbracoStartup : ApplicationEventHandler
        {
            protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
            {
                //With the content finder we can match nodes to urls.
                ContentFinderResolver.Current.InsertTypeBefore<ContentFinderByNiceUrl, FindRedirect>();

                //Remove the ContentFinderByNiceUrl because our DomainContentFinder should find all the content.
                //ContentFinderResolver.Current.RemoveType<ContentFinderByNiceUrl>();
            }
        }
 
}