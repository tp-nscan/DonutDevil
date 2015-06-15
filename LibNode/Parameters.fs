namespace LibNode
open System
open System.Collections.Generic
open MathUtils
open Rop

    type BoolParam = {Value:bool;}
    type FloatParam = {Value:float32; Min:float32; Max:float32;}
    type IntParam = {Value:int; Min:int; Max:int;}
    type EnumParam = {Type:Type; Value:string;}
    type GaussParam = {Radius:int; Decay:float32;}
    type GuidParam = {Value:Guid;}
    type SymmetricGlauberParam = {Radius:int; Decay:float32; Freq:float32;}

    type LocalCpls1D =
        | Const of int
        | Gaussian of GaussParam
        | Glauber of SymmetricGlauberParam

    type LocalCpls2D =
        | Const of int
        | Gaussian of GaussParam
        | SymmetricGlauber of SymmetricGlauberParam

    type ParamValue =
        | Bool of BoolParam
        | Enum of EnumParam
        | Float of FloatParam
        | Guid of GuidParam
        | Int of IntParam
        | LocalCpls1D of LocalCpls1D
        | LocalCpls2D of LocalCpls2D

    type Param = {Name:string; Value: ParamValue; CanChangeAtRunTime: bool}

module Parameters =

    let GuidParam name guid = {Param.Name=name; Value=ParamValue.Guid{Value=guid}; CanChangeAtRunTime=false}
    let BoolParam name value canChange = {Param.Name=name; Value=ParamValue.Bool{BoolParam.Value=value;}; CanChangeAtRunTime=canChange}
    let IntParam name value canChange = {Param.Name=name; Value=ParamValue.Int{Value=value; Min=1; Max=1000}; CanChangeAtRunTime=canChange}
    let FloatParam name value canChange = {Param.Name="MaxValue"; Value=ParamValue.Float{Value=value; Min=0.01f; Max=1.0f}; CanChangeAtRunTime=canChange}


    let ParentIdParam = {Param.Name="ParentId"; Value=ParamValue.Guid{Value=Guid.NewGuid()}; CanChangeAtRunTime=false}


//    let UpdateFrequencyParam = {Param.Name="UpdateFrequency"; Value=ParamValue.Int{Value=10; Min=1; Max=50}; CanChangeAtRunTime=true}
//    let ArrayStrideParam = {Param.Name="ArrayStride"; Value=ParamValue.Int{Value=128; Min=8; Max=1024}; CanChangeAtRunTime=false}
//    let NoiseSeedParam = {Param.Name="NoiseSeed"; Value=ParamValue.Int{Value=123; Min=1; Max=32000}; CanChangeAtRunTime=false}
//    let MemSeedParam = {Param.Name="MemSeed"; Value=ParamValue.Int{Value=123; Min=1; Max=32000}; CanChangeAtRunTime=false}
//    let CnxnSeedParam = {Param.Name="CnxnSeed"; Value=ParamValue.Int{Value=1237; Min=1; Max=32000}; CanChangeAtRunTime=false}
//    let SeedParam = {Param.Name="Seed"; Value=ParamValue.Int{Value=1267; Min=1; Max=32000}; CanChangeAtRunTime=false}
//    let MemCountParam = {Param.Name="MemCount"; Value=ParamValue.Int{Value=10; Min=1; Max=1024}; CanChangeAtRunTime=false}
//    let EnsembleCountParam = {Param.Name="EnsembleCount"; Value=ParamValue.Int{Value=150; Min=1; Max=1000}; CanChangeAtRunTime=false}
//    let NodeCountParam = {Param.Name="NodeCount"; Value=ParamValue.Int{Value=50; Min=1; Max=1000}; CanChangeAtRunTime=false}
//    let RowCountParam = {Param.Name="RowCount"; Value=ParamValue.Int{Value=50; Min=1; Max=1000}; CanChangeAtRunTime=false}
//    let ColCountParam = {Param.Name="ColCount"; Value=ParamValue.Int{Value=50; Min=1; Max=1000}; CanChangeAtRunTime=false}
//
//    let StepSizeParam = {Param.Name="StepSize"; Value=ParamValue.Float{Value=0.1f; Min=0.01f; Max=1.0f}; CanChangeAtRunTime=true}
//    let NoiseLevelParam = {Param.Name="NoiseLevel"; Value=ParamValue.Float{Value=0.1f; Min=0.01f; Max=1.0f}; CanChangeAtRunTime=true}
//    let MaxValueParam = {Param.Name="MaxValue"; Value=ParamValue.Float{Value=0.3f; Min=0.01f; Max=1.0f}; CanChangeAtRunTime=true}
//    let StartMagParam = {Param.Name="StartMag"; Value=ParamValue.Float{Value=0.1f; Min=0.0f; Max=1.0f}; CanChangeAtRunTime=false}
//    let CnxnMagParam = {Param.Name="CnxnMag"; Value=ParamValue.Float{Value=0.1f; Min=0.0f; Max=1.0f}; CanChangeAtRunTime=true}
//    let StepSize_XParam = {Param.Name="StepSize_X"; Value=ParamValue.Float{Value=0.1f; Min=0.0f; Max=1.0f}; CanChangeAtRunTime=true}
//    let StepSize_YParam = {Param.Name="StepSize_Y"; Value=ParamValue.Float{Value=0.1f; Min=0.0f; Max=1.0f}; CanChangeAtRunTime=true}
//    let TwistBiasParam = {Param.Name="TwistBias"; Value=ParamValue.Float{Value=0.1f; Min=0.0f; Max=1.0f}; CanChangeAtRunTime=true}

    let RandomMatrixSet rowCount colCount seed maxValue =
        [
            IntParam "RowCount" rowCount false;
            IntParam "ColCount" colCount false;
            IntParam "Seed" seed false;
            BoolParam "Unsigned" false false;
            FloatParam "MaxValue" maxValue false;
        ]

    let LinearLocalSet =
        [
//            ArrayStrideParam;
//            StepSizeParam;
//            NoiseSeedParam;
//            CnxnSeedParam;
//            NoiseLevelParam;
        ]

    let RandomCliqueSet entityId synapseMatrixId ensembleMatrixId stepSize noiseSeed noiseLevel =
        [
            GuidParam "SynapseMatrixId" synapseMatrixId;
            GuidParam "EnsembleMatrixId" ensembleMatrixId;
            FloatParam "StepSize" stepSize true;
            IntParam "NoiseSeed" noiseSeed false;
            FloatParam "NoiseLevel" noiseLevel true;
        ]

    let RingSet =
        [
//            ArrayStrideParam;
//            StepSizeParam;
//            NoiseLevelParam;
        ]

    let TwisterSet =
        [
//            ArrayStrideParam;
//            StepSize_XParam;
//            StepSize_YParam;
//            TwistBiasParam;
        ]

    let SpotSet =
        [
//            ArrayStrideParam;
//            StepSize_XParam;
//            StepSize_YParam;
//            TwistBiasParam;
        ]

    let GetParamByName (prams:Param list) (name:string) =
        try
            prams |> List.find(fun p -> p.Name = name) |> Rop.succeed

        with 
        | ex ->  (sprintf "Name: %s not found in list" name) |> Rop.fail


    let GetParamValueBool pram =
        match pram.Value with
        | Bool bp -> Rop.succeed bp.Value
        | Enum _ -> Rop.fail (sprintf "Param: %s is Enum not bool" pram.Name)
        | Float _ -> Rop.fail (sprintf "Param: %s is Float not bool" pram.Name)
        | Int _ -> Rop.fail (sprintf "Param: %s is Int not bool" pram.Name)
        | Guid _ -> Rop.fail (sprintf "Param: %s is Guid not bool" pram.Name)
        | LocalCpls1D _ -> Rop.fail (sprintf "Param: %s is LocalCpls1D not bool" pram.Name)
        | LocalCpls2D _ -> Rop.fail (sprintf "Param: %s is LocalCpls2D not bool" pram.Name)

    let GetParamValueEnum pram =
        match pram.Value with
        | Bool _ -> Rop.fail (sprintf "Param: %s is bool not enum" pram.Name)
        | Enum ep -> Rop.succeed ep.Value
        | Float _ -> Rop.fail (sprintf "Param: %s is Float not enum" pram.Name)
        | Int _ -> Rop.fail (sprintf "Param: %s is Int not enum" pram.Name)
        | Guid _ -> Rop.fail (sprintf "Param: %s is Guid not enum" pram.Name)
        | LocalCpls1D _ -> Rop.fail (sprintf "Param: %s is LocalCpls1D not enum" pram.Name)
        | LocalCpls2D _ -> Rop.fail (sprintf "Param: %s is LocalCpls2D not enum" pram.Name)

    let GetParamValueFloat32 pram =
        match pram.Value with
        | Bool _ -> Rop.fail (sprintf "Param: %s is bool not float" pram.Name)
        | Enum _ -> Rop.fail (sprintf "Param: %s is Enum not float" pram.Name)
        | Float fp -> Rop.succeed fp.Value
        | Int _ -> Rop.fail (sprintf "Param: %s is Int not float" pram.Name)
        | Guid _ -> Rop.fail (sprintf "Param: %s is Guid not float" pram.Name)
        | LocalCpls1D _ -> Rop.fail (sprintf "Param: %s is LocalCpls1D not float" pram.Name)
        | LocalCpls2D _ -> Rop.fail (sprintf "Param: %s is LocalCpls2D not float" pram.Name)

    let GetParamValueGuid pram =
        match pram.Value with
        | Bool _ -> Rop.fail (sprintf "Param: %s is bool not Guid" pram.Name)
        | Enum _ -> Rop.fail (sprintf "Param: %s is Enum not Guid" pram.Name)
        | Float _ -> Rop.fail (sprintf "Param: %s is Float not Guid" pram.Name)
        | Guid ip -> Rop.succeed ip.Value
        | Int ip -> Rop.fail (sprintf "Param: %s is Float not Guid" pram.Name)
        | LocalCpls1D _ -> Rop.fail (sprintf "Param: %s is LocalCpls1D not Guid" pram.Name)
        | LocalCpls2D _ -> Rop.fail (sprintf "Param: %s is LocalCpls2D not Guid" pram.Name)

    let GetParamValueInt pram =
        match pram.Value with
        | Bool _ -> Rop.fail (sprintf "Param: %s is bool not int" pram.Name)
        | Enum _ -> Rop.fail (sprintf "Param: %s is Enum not int" pram.Name)
        | Float _ -> Rop.fail (sprintf "Param: %s is Float not int" pram.Name)
        | Int ip -> Rop.succeed ip.Value
        | Guid _ -> Rop.fail (sprintf "Param: %s is Guid not int" pram.Name)
        | LocalCpls1D _ -> Rop.fail (sprintf "Param: %s is LocalCpls1D not int" pram.Name)
        | LocalCpls2D _ -> Rop.fail (sprintf "Param: %s is LocalCpls2D not int" pram.Name)

    let GetParamValueLocalCpls1D pram =
        match pram.Value with
        | Bool _ -> Rop.fail (sprintf "Param: %s is bool not LocalCpls1D" pram.Name)
        | Enum _ -> Rop.fail (sprintf "Param: %s is Enum not LocalCpls1D" pram.Name)
        | Float _ -> Rop.fail (sprintf "Param: %s is Float not LocalCpls1D" pram.Name)
        | Int _ -> Rop.fail (sprintf "Param: %s is Int not LocalCpls1D" pram.Name)
        | Guid _ -> Rop.fail (sprintf "Param: %s is Guid not LocalCpls1D" pram.Name)
        | LocalCpls1D ip -> Rop.succeed ip
        | LocalCpls2D _ -> Rop.fail (sprintf "Param: %s is LocalCpls2D not LocalCpls1D" pram.Name)

    let GetParamValueLocalCpls2D pram =
        match pram.Value with
        | Bool _ -> Rop.fail (sprintf "Param: %s is bool not LocalCpls2D" pram.Name)
        | Enum _ -> Rop.fail (sprintf "Param: %s is Enum not LocalCpls2D" pram.Name)
        | Float _ -> Rop.fail (sprintf "Param: %s is Float not LocalCpls2D" pram.Name)
        | Int _ -> Rop.fail (sprintf "Param: %s is Int not LocalCpls2D" pram.Name)
        | Guid _ -> Rop.fail (sprintf "Param: %s is Guid not LocalCpls2D" pram.Name)
        | LocalCpls1D _ -> Rop.fail (sprintf "Param: %s is LocalCpls1D not LocalCpls2D" pram.Name)
        | LocalCpls2D _ -> Rop.succeed pram.Value

    let GetBoolParam (prams: Param list) (name:string) =
        GetParamValueBool >>= (GetParamByName prams name)

    let GetEnumParam (prams: Param list) (name:string) =
        GetParamValueEnum >>= (GetParamByName prams name)

    let GetFloat32Param (prams: Param list) (name:string) =
        GetParamValueFloat32 >>= (GetParamByName prams name)

    let GetGuidParam (prams: Param list) (name:string) =
        GetParamValueGuid >>= (GetParamByName prams name)

    let GetIntParam (prams: Param list) (name:string) =
        GetParamValueInt >>= (GetParamByName prams name)

    let GetLocalCpls1DParam (prams: Param list) (name:string) =
        GetParamValueLocalCpls1D >>= (GetParamByName prams name)

    let GetLocalCpls2DParam (prams: Param list) (name:string) =
        GetParamValueLocalCpls2D >>= (GetParamByName prams name)

    let MakeBoolParam (name:string) (value:bool) (canChangeAtRunTime:bool) =
        {Name=name; Value=ParamValue.Bool({BoolParam.Value = value}); CanChangeAtRunTime=canChangeAtRunTime}

    let MakeFloatParam (name:string) (value:float32) (minValue:float32) (maxValue:float32) (canChangeAtRunTime:bool) =
        {Name=name; Value=ParamValue.Float({FloatParam.Value=value; FloatParam.Min=minValue; FloatParam.Max=maxValue}); CanChangeAtRunTime=canChangeAtRunTime}

    let MakeIntParam (name:string) (value:int) (minValue:int) (maxValue:int) (canChangeAtRunTime:bool) =
        {Name=name; Value=ParamValue.Int({IntParam.Value=value;IntParam.Min=minValue;IntParam.Max=maxValue}); CanChangeAtRunTime=canChangeAtRunTime}

    let MakeLocalCpls1DConstParam (name:string) (value:int) (canChangeAtRunTime:bool) =
        let lc1DConst = LocalCpls1D.Const(value)
        {Name=name; Value=ParamValue.LocalCpls1D(lc1DConst); CanChangeAtRunTime=canChangeAtRunTime}

    let MakeLocalCpls1DGaussianParam (name:string) (radius:int) (decay:float32) (canChangeAtRunTime:bool) =
        let lc1DGaussian = LocalCpls1D.Gaussian { Radius=radius; Decay=decay; }
        {Name=name; Value=ParamValue.LocalCpls1D(lc1DGaussian); CanChangeAtRunTime=canChangeAtRunTime}

    let MakeLocalCpls1DIntParam (name:string) (radius:int) (decay:float32) (freq:float32) (canChangeAtRunTime:bool) =
        let lc1DGlauber = LocalCpls1D.Glauber {Radius=radius; Decay=decay; Freq=freq}
        {Name=name; Value=ParamValue.LocalCpls1D(lc1DGlauber); CanChangeAtRunTime=canChangeAtRunTime}

    let MakeLocalCpls2DConstParam (name:string) (value:int) (canChangeAtRunTime:bool) =
        let lc2DConst = LocalCpls2D.Const(value)
        {Name=name; Value=ParamValue.LocalCpls2D(lc2DConst); CanChangeAtRunTime=canChangeAtRunTime}

    let MakeLocalCpls2DGaussianParam (name:string) (radius:int) (decay:float32) (canChangeAtRunTime:bool) =
        let lc2DGaussian = LocalCpls2D.Gaussian { Radius=radius; Decay=decay; }
        {Name=name; Value=ParamValue.LocalCpls2D(lc2DGaussian); CanChangeAtRunTime=canChangeAtRunTime}

    let MakeLocalCpls2DIntParam (name:string) (radius:int) (decay:float32) (freq:float32) (canChangeAtRunTime:bool) =
        let lc2DGlauber = LocalCpls2D.SymmetricGlauber {Radius=radius; Decay=decay; Freq=freq}
        {Name=name; Value=ParamValue.LocalCpls2D(lc2DGlauber); CanChangeAtRunTime=canChangeAtRunTime}