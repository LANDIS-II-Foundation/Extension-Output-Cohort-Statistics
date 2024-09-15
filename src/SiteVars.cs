//  Authors:  Brendan C. Ward, Robert M. Scheller

using Landis.SpatialModeling;
using Landis.Library.UniversalCohorts;

namespace Landis.Extension.Output.CohortStats
{
    public static class SiteVars
    {
        private static ISiteVar<SiteCohorts> cohorts;
        private static ISiteVar<int> siteAgeRichness;

        //---------------------------------------------------------------------

        public static void Initialize()
        {
            cohorts = PlugIn.ModelCore.GetSiteVar<SiteCohorts>("Succession.UniversalCohorts");

            siteAgeRichness = PlugIn.ModelCore.Landscape.NewSiteVar<int>();

            PlugIn.ModelCore.RegisterSiteVar(siteAgeRichness, "Output.AgeRichness");

            if(SiteVars.Cohorts == null)
                throw new System.ApplicationException("Site Cohorts NOT Initialized.");

        }

        //---------------------------------------------------------------------
        public static ISiteVar<SiteCohorts> Cohorts
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
