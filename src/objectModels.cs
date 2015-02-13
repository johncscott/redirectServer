using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tg.redirectServer
{
    public class LookupRedirect
    {
        public Dictionary<string, string> Lookup { get; set; }

        public LookupRedirect(UrlListModel ulm)
        {
            Lookup = new Dictionary<string, string>();
            foreach (var z in ulm.Fieldsets)
            {
                string source = z.Properties.Where(a => a.Alias == "source").Select(s => s.Value).FirstOrDefault();
                string value = z.Properties.Where(a => a.Alias == "destination").Select(s => s.Value).FirstOrDefault();
                Lookup.Add(source, value);
            }
        }
        
    
    }


    public class UrlListModel
    {
        [JsonProperty("fieldsets")]
        public List<ArchetypeFieldsetModel> Fieldsets { get; set; }

    }

    public class ArchetypeFieldsetModel
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("disabled")]
        public bool Disabled { get; set; }

        [JsonProperty("properties")]
        public List<ArchetypeProperty> Properties;

        public ArchetypeFieldsetModel()
        {
            Properties = new List<ArchetypeProperty>();
        }

    }

    public class ArchetypeProperty
    {
        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

}