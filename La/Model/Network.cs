using System.Collections.Generic;
using LibNode;

namespace La.Model
{
    //public interface INetwork
    //{
    //    int Generation { get; }
    //    IDictionary<string, Param> Params { get; }
    //    IDictionary<string, DataBlock> DataBlocks { get; }
    //}

    //public static class Network
    //{
    //    public static INetwork MakeLinearNetwork(int memLength, int memCount)
    //    {
    //        const string memName = "Memories";
    //        var dd = new Dictionary<string, DataBlock>();
    //        dd[memName] = SimpleNetwork.MakeRandomLinearBinaryMemories(memLength, memCount, memName);

    //        return new NetworkImpl(
    //                dataBlocks: dd,
    //                prams: Parameters.LinearLocalSet,
    //                generation: 0
    //            );
    //    }
    //}

    //class NetworkImpl : INetwork
    //{
    //    public NetworkImpl(IDictionary<string, DataBlock> dataBlocks, 
    //                       IDictionary<string, Param> prams, 
    //                       int generation)
    //    {
    //        DataBlocks = dataBlocks;
    //        Params = prams;
    //        Generation = generation;
    //    }

    //    public int Generation { get; }
    //    public IDictionary<string, Param> Params { get; }
    //    public IDictionary<string, DataBlock> DataBlocks { get; }
    //}
}
