using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Common;
using tradelr.Library;

namespace tradelr.Models.google.gears
{
    public class Manifest
    {
        public int betaManifestVersion { get; set; }
        public string version { get; set; }
        public List<ManifestEntry> entries { get; set; }

        public Manifest(string version)
        {
            this.version = version;
            betaManifestVersion = 1;
            entries = new List<ManifestEntry>();
        }

        public void LoadManifestEntries(IEnumerable<string> e, string subdomain)
        {
            foreach (var entry in e)
            {
                var me = new ManifestEntry() {url = subdomain.ToDomainUrl(entry)};
                this.entries.Add(me);
            }
        }
    }
}