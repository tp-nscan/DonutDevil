using MathLib;

namespace NodeLib.Updaters
{
    public class UpdateUtils
    {

        public static float[] RingRadialCosBiases
            (
                float step,
                float rBias,
                float aBias
            )
        {
            return new[]
            {
                step*  (1.0f + rBias * aBias.UnitToCos(Perimeter.UC)),  //0
                step * (1.0f + rBias * aBias.UnitToCos(Perimeter.UR)),  //1
                step * (1.0f + rBias * aBias.UnitToCos(Perimeter.CR)),  //2
                step * (1.0f + rBias * aBias.UnitToCos(Perimeter.LR)),  //3
                step * (1.0f + rBias * aBias.UnitToCos(Perimeter.LC)),  //4
                step * (1.0f + rBias * aBias.UnitToCos(Perimeter.LF)),  //5
                step * (1.0f + rBias * aBias.UnitToCos(Perimeter.CF)),  //6
                step * (1.0f + rBias * aBias.UnitToCos(Perimeter.UF))   //7
            };

        }


        public static float[] RingRadialSinBiases
        (
            float step,
            float rBias,
            float aBias
        )
        {
            return new[]
                {
                    step*  (1.0f + rBias * aBias.UnitToSin(Perimeter.UC)), 
                    step * (1.0f + rBias * aBias.UnitToSin(Perimeter.UR)),
                    step * (1.0f + rBias * aBias.UnitToSin(Perimeter.CR)),
                    step * (1.0f + rBias * aBias.UnitToSin(Perimeter.LR)),
                    step * (1.0f + rBias * aBias.UnitToSin(Perimeter.LC)),
                    step * (1.0f + rBias * aBias.UnitToSin(Perimeter.LF)),
                    step * (1.0f + rBias * aBias.UnitToSin(Perimeter.CF)),
                    step * (1.0f + rBias * aBias.UnitToSin(Perimeter.UF))
                };

        }



    }
}
