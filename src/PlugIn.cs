//  Authors:  Brendan C. Ward, Robert M. Scheller

using Landis.Core;
using Landis.SpatialModeling;

using System.Collections.Generic;

namespace Landis.Extension.Output.CohortStats
{
    public class PlugIn
        : ExtensionMain
    {
        public static readonly ExtensionType ExtType = new ExtensionType("output");
        public static readonly string ExtensionName = "Output Cohort Statistics";
        
        private static ICore modelCore;
        private string sppagestats_mapNames;
        private string siteagestats_mapNames;
        private string sitesppstats_mapNames;
        private Dictionary<string, IEnumerable<ISpecies>> ageStatSpecies;
        private List<string> siteAgeStats;
        private List<string> siteSppStats;
        private IInputParameters parameters;

        //---------------------------------------------------------------------

        public PlugIn()
            : base(ExtensionName, ExtType)
        {
        }

        //---------------------------------------------------------------------

        public static ICore ModelCore
        {
            get
            {
                return modelCore;
            }
        }

        //---------------------------------------------------------------------

        public override void AddCohortData()
        {
            return;
        }

        //---------------------------------------------------------------------

        public override void LoadParameters(string dataFile, ICore mCore)
        {
            modelCore = mCore;
            SiteVars.Initialize();
            InputParameterParser parser = new InputParameterParser();
            parameters = Landis.Data.Load<IInputParameters>(dataFile, parser);
            
        }
        //---------------------------------------------------------------------

        public override void Initialize()
        {

            Timestep = parameters.Timestep;
            sppagestats_mapNames = parameters.SppAgeStats_MapNames;
            siteagestats_mapNames = parameters.SiteAgeStats_MapNames;
            sitesppstats_mapNames = parameters.SiteSppStats_MapNames;
            ageStatSpecies = parameters.AgeStatSpecies;
            siteAgeStats = parameters.SiteAgeStats;
            siteSppStats = parameters.SiteSppStats;
            MetadataHandler.InitializeMetadata(sppagestats_mapNames, sitesppstats_mapNames, sitesppstats_mapNames, ageStatSpecies, siteAgeStats, siteSppStats);
        }

        //---------------------------------------------------------------------

        public override void Run()
        {
            
            //1) Create the output species age stats maps
            foreach (KeyValuePair<string, IEnumerable<ISpecies>> sppAgeStatIter in ageStatSpecies)
            {
                //statIter.Key = statistic name
                //set a function pointer here for the statistic, so we don't have to do the switch operation for every single pixel every single time??
                CohortUtils.SpeciesCohortStatDelegate species_stat_func;
                
                switch (sppAgeStatIter.Key)
                {
                    case "MAX":
                        species_stat_func = new CohortUtils.SpeciesCohortStatDelegate(CohortUtils.GetMaxAge);
                        break;
                    case "MIN":
                        species_stat_func = new CohortUtils.SpeciesCohortStatDelegate(CohortUtils.GetMinimumAge);                     
                        break;
                    case "MED":
                        species_stat_func = new CohortUtils.SpeciesCohortStatDelegate(CohortUtils.GetMedianAge);
                        break;
                    case "AVG":
                        species_stat_func = new CohortUtils.SpeciesCohortStatDelegate(CohortUtils.GetAvgAge);
                        break;
                    case "SD":
                        species_stat_func = new CohortUtils.SpeciesCohortStatDelegate(CohortUtils.GetStdDevAge);
                        break;

                    default:
                        //this shouldn't ever occur
                        System.Console.WriteLine("Unhandled statistic: {0}, using MaxAge Instead",sppAgeStatIter.Key);
                        species_stat_func = new CohortUtils.SpeciesCohortStatDelegate(CohortUtils.GetMaxAge);
                        break;
                }

                foreach (ISpecies species in sppAgeStatIter.Value)
                {
                    string path = SpeciesMapNames.ReplaceTemplateVars(sppagestats_mapNames, species.Name, sppAgeStatIter.Key, PlugIn.ModelCore.CurrentTime);
                    ModelCore.UI.WriteLine("   Writing {0} map for {1} to {2} ...", sppAgeStatIter.Key, species.Name, path);
                    using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(path, PlugIn.ModelCore.Landscape.Dimensions))
                    {
                        IntPixel pixel = outputRaster.BufferPixel;
                        foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                        {
                            if (!site.IsActive) // and has the spp we want
                                pixel.MapCode.Value = 0;
                            else
                            {
                                //need to do a switch on statistic
                                pixel.MapCode.Value = (int) species_stat_func(species, site); 
                            }

                            outputRaster.WriteBufferPixel();
                        }
                    }
                }
            }

            //2) Create the output site stats maps
            foreach(string ageStatIter in siteAgeStats)
            {
                //PlugIn.ModelCore.UI.WriteLine("   Age Stat = {0}", ageStatIter);

                CohortUtils.SiteCohortStatDelegate site_stat_func;
                switch (ageStatIter)
                {
                    case "MAX":
                        site_stat_func = new CohortUtils.SiteCohortStatDelegate(CohortUtils.GetMaxAge);
                        break;
                    case "MIN":
                        site_stat_func = new CohortUtils.SiteCohortStatDelegate(CohortUtils.GetMinimumAge);
                        break;
                    case "MED":
                        site_stat_func = new CohortUtils.SiteCohortStatDelegate(CohortUtils.GetMedianAge);
                        break;
                    case "AVG":
                        site_stat_func = new CohortUtils.SiteCohortStatDelegate(CohortUtils.GetAvgAge);
                        break;
                    case "SD":
                        site_stat_func = new CohortUtils.SiteCohortStatDelegate(CohortUtils.GetStdDevAge);
                        break;
                    case "COUNT":
                        site_stat_func = new CohortUtils.SiteCohortStatDelegate(CohortUtils.GetCohortCount);
                        break;
                    case "RICH":
                        site_stat_func = new CohortUtils.SiteCohortStatDelegate(CohortUtils.GetAgeRichness);
                        break;
                    case "EVEN":
                        site_stat_func = new CohortUtils.SiteCohortStatDelegate(CohortUtils.GetAgeEvenness);
                        break;                        

                    default:
                        System.Console.WriteLine("Unhandled statistic: {0}, using MaxAge Instead", ageStatIter);
                        site_stat_func = new CohortUtils.SiteCohortStatDelegate(CohortUtils.GetMaxAge);
                        break;
                }

                string path = SiteMapNames.ReplaceTemplateVars(siteagestats_mapNames, ageStatIter, ModelCore.CurrentTime);
                ModelCore.UI.WriteLine("   Writing {0} site map to {1} ...", ageStatIter, path);
                using (IOutputRaster<IntPixel> outputRaster = ModelCore.CreateRaster<IntPixel>(path, ModelCore.Landscape.Dimensions))
                {
                    IntPixel pixel = outputRaster.BufferPixel;
                    foreach (Site site in ModelCore.Landscape.AllSites)
                    {
                        if (!site.IsActive)
                            pixel.MapCode.Value = 0;
                        else
                            pixel.MapCode.Value = (int) site_stat_func(site);

                        outputRaster.WriteBufferPixel();
                    }
                }
            }

            //3) Create the output site species stats maps
            foreach (string sppStatIter in siteSppStats)
            {
                CohortUtils.SiteCohortStatDelegate site_stat_func;
                switch (sppStatIter)
                {
                    case "RICH":
                        site_stat_func = new CohortUtils.SiteCohortStatDelegate(CohortUtils.GetSppRichness);
                        break;
                    default:
                        System.Console.WriteLine("Unhandled statistic: {0}, using Species Richness Instead", sppStatIter);
                        site_stat_func = new CohortUtils.SiteCohortStatDelegate(CohortUtils.GetSppRichness);
                        break;
                }

                string path = SiteMapNames.ReplaceTemplateVars(sitesppstats_mapNames, sppStatIter, ModelCore.CurrentTime);
                ModelCore.UI.WriteLine("   Writing {0} site map to {1} ...", sppStatIter, path);
                using (IOutputRaster<IntPixel> outputRaster = ModelCore.CreateRaster<IntPixel>(path, ModelCore.Landscape.Dimensions))
                {
                    IntPixel pixel = outputRaster.BufferPixel;
                    foreach (Site site in ModelCore.Landscape.AllSites)
                    {
                        if (!site.IsActive)
                            pixel.MapCode.Value = 0;
                        else
                            pixel.MapCode.Value = (int) site_stat_func(site); 

                        outputRaster.WriteBufferPixel();
                    }
                }
            }



        }

    }
}

