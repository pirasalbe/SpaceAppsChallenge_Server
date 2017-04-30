using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Trace_Invader
{
    class Taxon
    {
        public string id;
        public string name;
        public string observations_count;
        public string wikipedia_summary;
        public TaxonPhoto[] taxon_photos;

        public Taxon(string id, string name, string observations_count, string wikipedia_summary, TaxonPhoto[] taxon_photos)
        {
            this.id = id;
            this.name = name;
            this.observations_count = observations_count;
            this.wikipedia_summary = wikipedia_summary;
            this.taxon_photos = taxon_photos;
        }
    }
}
