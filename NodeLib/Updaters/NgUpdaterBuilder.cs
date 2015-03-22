using System;
using System.Collections.Generic;
using System.Linq;
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
                    new ParamEnum(typeof (NeighborhoodType), NeighborhoodType.Perimeter.ToString(), "NeighborhoodType"),
                    new ParamFloat(0.0f, 1.0f, 0.5f, "StepSize"),
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

                NeighborhoodType neighborhoodType;
                Enum.TryParse((string)d["NeighborhoodType"].Value, out neighborhoodType);
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
