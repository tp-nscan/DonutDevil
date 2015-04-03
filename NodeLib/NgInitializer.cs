using System;
using System.Collections.Generic;
using NodeLib.Params;

namespace NodeLib
{
    public class NgInitializer
    {
        public static Func<IReadOnlyDictionary<string, IParameter>, INodeGroup> KStrideSquaredUnitR(int k)
        {
            return d =>
            {
                var arrayStride = (int)d["ArrayStride"].Value;
                return NodeGroup.RandomNodeGroupUnitR(k * arrayStride * arrayStride, (int)DateTime.Now.Ticks);

            };
        }

        public static Func<IReadOnlyDictionary<string, IParameter>, INodeGroup> KStrideSquaredUnitZ(int k)
        {
            return d =>
            {
                var arrayStride = (int)d["ArrayStride"].Value;
                return NodeGroup.RandomNodeGroupUnitZ(k * arrayStride * arrayStride, (int)DateTime.Now.Ticks);

            };
        }
    }
}
