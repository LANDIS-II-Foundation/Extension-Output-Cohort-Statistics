//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;

namespace Landis.Extension.Output.CohortStats
{
    public class IntPixel : Pixel
    {
        public Band<int> MapCode  = "The numeric code for each raster cell";

        public IntPixel()
        {
            SetBands(MapCode);
        }
    }
}
