namespace LibNode
open System
open MathNet.Numerics
open MathNet.Numerics.Random
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Matrix
open Rop

module MathNetUtils =

    let SymRowMajorIndex x y =
        if (x >= y) then x*(x+1)/2 + y
        else y*(y+1)/2 + x

    let MatrixF32ZeroD (matrix:Matrix<float32>) =
        DenseMatrix.init
            matrix.RowCount matrix.ColumnCount 
            (fun i j -> if(i=j) then 0.0f else matrix.[i,j])

    let RandNormalSqDenseSF32 stride rng sigm =
        let rSqData = Generators.NormalSF32 rng sigm
                    |> Seq.take(stride*stride) |> Seq.toArray
        DenseMatrix.init stride stride 
            (fun x y -> rSqData.[x + y*stride])

    let RandNormalSqSymDenseSF32 stride rng sigm =
        let rSqData = Generators.NormalSF32 rng sigm
                      |> Seq.take(stride*(stride + 1) / 2) |> Seq.toArray
        DenseMatrix.init stride stride 
            (fun x y -> rSqData.[SymRowMajorIndex x y])