using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Trace_Invader
{
    class Place
    {
        public string display_name;
        public string name;
        public string nelat;
        public string nelng;
        public string swlat;
        public string swlng;
        public string place_type_name;
        public Place()
        {

        }
        public Place(string display_name, string name, string nelat, string nelng, string swlat, string swlng, string place_type_name)
        {
            this.display_name = display_name;
            this.name = name;
            this.nelat = nelat;
            this.nelng = nelng;
            this.swlat = swlat;
            this.swlng = swlng;
            this.place_type_name = place_type_name;
        }
    }
}
