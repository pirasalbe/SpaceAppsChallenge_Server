using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Trace_Invader
{
    class ObservationDetail : Observation
    {
        public string quality_grade;
        public string identifications_count;
        public Taxon taxon;
        public ObservationDetail(string id, string latitude, string longitude, string time_observed_at_utc, string place_guess, string species_guess, string quality_grade, string identifications_count, Taxon taxon) : base(id, latitude, longitude, time_observed_at_utc, place_guess, species_guess)
        {
            this.quality_grade = quality_grade;
            this.identifications_count = identifications_count;
            this.taxon = taxon;
        }
    }
}
