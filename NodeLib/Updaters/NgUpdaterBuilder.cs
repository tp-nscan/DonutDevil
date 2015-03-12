using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeLib.Params;

namespace NodeLib.Updaters
{
    public static class NgUpdaterBuilder
    {
        public static IReadOnlyDictionary<string, IParameter> StandardRingParams()
        {
           return
                new IParameter[]
                {
                    new ParamEnum(typeof (NeighboorhoodType), NeighboorhoodType.Perimeter.ToString(), "NeighboorhoodType"),
                    new ParamFloat(0.0f, 1.0f, 0.5f, "Step size"),
                    new ParamFloat(0.0f, 1.0f, 0.0f, "Noise")
                }.ToDictionary(v => v.Name);
        }

        public static Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> StandardRing(
            int squareSize
            )
        {
            return d =>
            {
                var stepSize = (float)d["StepSize"].Value;
                var neighborhoodType = (NeighboorhoodType)d["NeighboorhoodType"].Value;
                var noise = (float)d["Noise"].Value;

                return NgUpdaterRing.Standard(
                    name: "StandardRing",
                    squareSize:squareSize,
                    offset: 0,
                    stepSize: stepSize,
                    neighborhoodType: neighborhoodType,
                    noise: noise
                );
            };
        }
    }
}
