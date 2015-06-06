namespace LibNode
    open System
    open System.Collections.Generic
    open MathUtils

    type OneFloat32 =
        | Real
        | UnitU
        | UnitS
        | Ring

    type TwoFloat32 =
        | Real
        | Torus
        | UnitU
        | UnitS

    type OneInt =
        | UnitU
        | UnitS


    type CompFloat32 =
        | OneFloat32
        | TwoFloat32

    type NumericShapeFloat = { Comp: CompFloat32; GroupShape: GroupShape }
    type NumericShapeInt = { Comp: OneInt; GroupShape: GroupShape }

//module CompositeData =


