using System;
using System.Collections.Generic;
using System.Linq;
//using System.Data;
using System.Text;
using Landis.Library.Metadata;
using Landis.Core;
using Landis.Utilities;
using System.IO;
using Flel = Landis.Utilities;

namespace Landis.Extension.Output.CohortStats
{
    public static class MetadataHandler
    {

        public static ExtensionMetadata Extension { get; set; }
        public static void InitializeMetadata(string speciesAgeMap, string siteAgeMap, string siteSpeciesMap, Dictionary<string, IEnumerable<ISpecies>> ageStatSpecies, List<string> siteAgeStats, List<string> siteSppStats)
        {

            ScenarioReplicationMetadata scenRep = new ScenarioReplicationMetadata()
            {
                RasterOutCellArea = PlugIn.ModelCore.CellArea,
                TimeMin = PlugIn.ModelCore.StartTime,
                TimeMax = PlugIn.ModelCore.EndTime,
            };

            Extension = new ExtensionMetadata(PlugIn.ModelCore)
            //Extension = new ExtensionMetadata()
            {
                Name = PlugIn.ExtensionName,
                TimeInterval = PlugIn.ModelCore.CurrentTime,
                ScenarioReplicationMetadata = scenRep
            };

            //---------------------------------------            
            //          map outputs:         
            //---------------------------------------

            //OutputMetadata mapOut_BiomassRemoved = new OutputMetadata()
            //{
            //    Type = OutputType.Map,
            //    Name = "biomass removed",
            //    FilePath = @HarvestMapName,
            //    Map_DataType = MapDataType.Continuous,
            //    Map_Unit = FieldUnits.Mg_ha,
            //    Visualize = true,
            //};
            //Extension.OutputMetadatas.Add(mapOut_BiomassRemoved);

            foreach (KeyValuePair<string, IEnumerable<ISpecies>> sppAgeStatIter in ageStatSpecies)
            {
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
                        System.Console.WriteLine("Unhandled statistic: {0}, using MaxAge Instead", sppAgeStatIter.Key);
                        species_stat_func = new CohortUtils.SpeciesCohortStatDelegate(CohortUtils.GetMaxAge);
                        break;
                }

                foreach (ISpecies species in sppAgeStatIter.Value)
                {
                    OutputMetadata mapOut_age_stats = new OutputMetadata()
                    {
                        Type = OutputType.Map,
                        Name = species.Name + "_age_stats_map",
                        FilePath = SpeciesMapNames.ReplaceTemplateVars(speciesAgeMap, species.Name, sppAgeStatIter.Key, PlugIn.ModelCore.CurrentTime),
                        Map_DataType = MapDataType.Continuous,
                        Visualize = true,
                        //Map_Unit = "categorical",
                    };
                    Extension.OutputMetadatas.Add(mapOut_age_stats);
                }
            }

            foreach (string ageStatIter in siteAgeStats)
            {
                CohortUtils.SiteCohortStatDelegate site_stat_func;
                switch (ageStatIter)
                {
                    case "MAX":
                        site_stat_func = new CohortUtils.SiteCohortStatDelegate(CohortUtils.GetMaxAge);
                        break;
                    case "MIN":
                        site_stat_func = new CohortUtils.SiteCohortStatDelegate(CohortUtils.GetMinAge);
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

                OutputMetadata mapOut_site_age_stats = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = ageStatIter + "_age_stats_map",
                    FilePath = SiteMapNames.ReplaceTemplateVars(siteAgeMap, ageStatIter, PlugIn.ModelCore.CurrentTime),
                    Map_DataType = MapDataType.Continuous,
                    Visualize = true,
                    //Map_Unit = "categorical",
                };
                Extension.OutputMetadatas.Add(mapOut_site_age_stats);

            }
            foreach (string sppStatIter in siteSppStats)
            {
                CohortUtils.SiteCohortStatDelegate site_stat_func;
                switch (sppStatIter)
                {
                    case "RICH":
                        //FIXME
                        site_stat_func = new CohortUtils.SiteCohortStatDelegate(CohortUtils.GetSppRichness);
                        break;
                    //add in richness
                    default:
                        System.Console.WriteLine("Unhandled statistic: {0}, using Species Richness Instead", sppStatIter);
                        site_stat_func = new CohortUtils.SiteCohortStatDelegate(CohortUtils.GetSppRichness);
                        break;
                }

                OutputMetadata mapOut_site_species_stats = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = sppStatIter + "_age_stats_map",
                    FilePath = SiteMapNames.ReplaceTemplateVars(siteSpeciesMap, sppStatIter, PlugIn.ModelCore.CurrentTime),
                    Map_DataType = MapDataType.Continuous,
                    Visualize = true,
                    //Map_Unit = "categorical",
                };
                Extension.OutputMetadatas.Add(mapOut_site_species_stats);

            }

            //---------------------------------------
            MetadataProvider mp = new MetadataProvider(Extension);
            mp.WriteMetadataToXMLFile("Metadata", Extension.Name, Extension.Name);
        }
        public static void CreateDirectory(string path)
        {
            //Require.ArgumentNotNull(path);
            path = path.Trim(null);
            if (path.Length == 0)
                throw new ArgumentException("path is empty or just whitespace");

            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
            {
                Flel.Directory.EnsureExists(dir);
            }

            //return new StreamWriter(path);
            return;
        }
    }
}
