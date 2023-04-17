//  Authors:  Brendan C. Ward, Robert M. Scheller

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.AgeOnlyCohorts;

namespace Landis.Extension.Output.CohortStats
{
    public static class SiteVars
    {
        private static ISiteVar<ISiteCohorts> cohorts;
        private static ISiteVar<int> siteAgeRichness;

        //---------------------------------------------------------------------

        public static void Initialize()
        {
            cohorts = PlugIn.ModelCore.GetSiteVar<ISiteCohorts>("Succession.AgeCohorts");

            PlugIn.ModelCore.RegisterSiteVar(siteAgeRichness, "Output.AgeRichness");

            siteAgeRichness = PlugIn.ModelCore.Landscape.NewSiteVar<int>();

        }

        //---------------------------------------------------------------------
        public static ISiteVar<ISiteCohorts> Cohorts
        {
            get
            {
                return cohorts;
            }
        }
        //---------------------------------------------------------------------
        public static ISiteVar<int> SiteAgeRichness
        {
            get
            {
                return siteAgeRichness;
            }
        }
    }
}
