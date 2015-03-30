using System;
using System.Collections.Generic;
using NodeLib.Params;

namespace NodeLib.Updaters
{
    public static class NgUpdaterBuilder
    {

        public static Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> ForRing()
        {
            return d =>
            {
                var arrayStride = (int)d["ArrayStride"].Value;
                NeighborhoodType neighborhoodType;
                Enum.TryParse((string)d["NeighborhoodType"].Value, out neighborhoodType);
                var stepSize = (float)d["StepSize"].Value;
                var noise = (float)d["Noise"].Value;
                var tent = (float)d["Tent"].Value;
                var saw = (float)d["Saw"].Value;

                return NgUpdaterRing.Standard(
                    name: "StandardRing",
                    squareSize: arrayStride,
                    stepSize: stepSize,
                    neighborhoodType: neighborhoodType,
                    noise: noise,
                    tent: tent,
                    saw: saw
                );
            };
        }

        public static Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> ForDonut()
        {
            return d =>
            {
                var arrayStride = (int)d["ArrayStride"].Value;
                NeighborhoodType neighborhoodType;
                Enum.TryParse((string)d["NeighborhoodType"].Value, out neighborhoodType);
                var stepSizeX = (float)d["StepSize_X"].Value;
                var stepSizeY = (float)d["StepSize_Y"].Value;
                var noise = (float)d["Noise"].Value;
                var tent = (float)d["Tent"].Value;
                var saw = (float)d["Saw"].Value;

                return NgUpdaterDonut.Standard(
                    name: "StandardTorus",
                    squareSize: arrayStride,
                    stepSizeX: stepSizeX,
                    stepSizeY: stepSizeY,
                    neighborhoodType: neighborhoodType,
                    noise: noise,
                    tent: tent,
                    saw: saw
                );
            };
        }


        public static Func<IReadOnlyDictionary<string, IParameter>, INgUpdater> ForTwister()
        {
            return d =>
            {
                var arrayStride = (int)d["ArrayStride"].Value;
                NeighborhoodType neighborhoodType;
                Enum.TryParse((string)d["NeighborhoodType"].Value, out neighborhoodType);
                var stepSizeX = (float)d["StepSize_X"].Value;
                var stepSizeY = (float)d["StepSize_Y"].Value;
                var bias = (float)d["Bias"].Value;
                var noise = (float)d["Noise"].Value;
                var tent = (float)d["Tent"].Value;
                var saw = (float)d["Saw"].Value;

                return NgUpdaterTwister.Standard(
                    name: "StandardTorus",
                    squareSize: arrayStride,
                    stepSizeX: stepSizeX,
                    stepSizeY: stepSizeY,
                    bias : bias,
                    neighborhoodType: neighborhoodType,
                    noise: noise,
                    tent: tent,
                    saw: saw
                );

            };
        }



    }
}
