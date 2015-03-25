using System;
using System.Collections.Generic;
using NodeLib.Params;

namespace NodeLib.Updaters
{
    public static class NgUpdaterBuilder
    {

        public static Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> ForStandardRing(
            int squareSize,
            int arrayOffset
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
                    arrayOffset: arrayOffset,
                    stepSize: stepSize,
                    neighborhoodType: neighborhoodType,
                    noise: noise
                );
            };
        }

        public static Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> ForStandardTorus(
            int squareSize,
            int arrayOffset
            )
        {
            return d =>
            {
                var stepSize = (float)d["StepSize"].Value;
                DualInteractionType dualInteractionType;
                Enum.TryParse((string)d["DualInteractionType"].Value, out dualInteractionType);

                return NgUpdaterTorus.Standard(
                    name: "StandardTorus",
                    squareSize: squareSize,
                    arrayOffset: arrayOffset,
                    stepSize: stepSize,
                    dualInteractionType: dualInteractionType,
                    otherParams: d
                );
            };
        }


    }
}
