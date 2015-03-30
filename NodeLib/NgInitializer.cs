using System;
using System.Collections.Generic;
using NodeLib.Params;

namespace NodeLib
{
    public class NgInitializer
    {
        public static Func<IReadOnlyDictionary<string, IParameter>, INodeGroup> KStrideSquared(int k)
        {
            return d =>
            {
                var arrayStride = (int)d["ArrayStride"].Value;
                return NodeGroup.RandomNodeGroup(k * arrayStride * arrayStride, (int)DateTime.Now.Ticks);

            };
        }

    }
}
