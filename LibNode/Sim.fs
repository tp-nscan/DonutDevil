namespace LibNode
open System
open MathUtils
open LibNode.Generators
open System.Collections.Generic

    type LaSim = { DataBlocks:IDictionary<string, DataBlock>; Generation:int }

    
module Sim =
    let foo = None