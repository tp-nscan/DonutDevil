namespace LibNode
open System
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.Random
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix
open Rop
open MathUtils

module MathNetUtils =

    let SymRowMajorIndex x y =
        if (x >= y) then x*(x+1)/2 + y
        else y*(y+1)/2 + x

    let UtCoords stride =
       let rec qq stride coords =
           seq {
                    match coords with
                    | a, b when b < a -> 
                        yield (a, b)
                        yield! (qq stride (a, b+ 1))
                    | a, b when b < (stride- 1) -> 
                        yield (a, b)
                        yield! qq stride (a+ 1, 0)
                    | a, b ->
                        yield (a, b)
           }
       seq {
             yield!  qq stride (0, 0)
       }
         

    let MatrixF32ZeroD (matrix:Matrix<float32>) =
        DenseMatrix.init
            matrix.RowCount matrix.ColumnCount 
            (fun i j -> if(i=j) then 0.0f else matrix.[i,j])

    let RandNormalSqDenseSF32 stride rng sigm =
        let rSqData = Generators.NormalSF32 rng sigm
                    |> Seq.take(stride*stride) |> Seq.toArray
        DenseMatrix.init stride stride 
            (fun x y -> rSqData.[x + y*stride])

    let UpperTriangulate stride f =
        let cached = (UtCoords stride) 
                        |> Seq.map(fun (a,b) -> f a b)
                        |> Seq.toArray                            

        (fun x y -> cached.[SymRowMajorIndex x y] )

    let UpperTriangulateZd stride f =
        let cached = (UtCoords stride) 
                        |> Seq.map(fun (a,b) -> 
                                        match (a,b) with
                                        | (a,b) when a=b -> 0.0f
                                        | (a,b) -> f a b)
                        |> Seq.toArray                            

        (fun x y -> cached.[SymRowMajorIndex x y] )

    let RandNormalSqSymDenseSF32 stride rng stddev = 
        let siter = Generators.NormalSF32 rng stddev
        DenseMatrix.init stride stride
            (UpperTriangulate stride (fun x y -> siter |> Seq.head))

    let ToRowMajorSequence (mnMatrix:Matrix<'a>) = 
         mnMatrix.ToArray() |> Seq.cast<'a>
