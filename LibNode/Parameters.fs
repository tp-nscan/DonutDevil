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
        | Int of IntParam
        | LocalCpls1D of LocalCpls1D
        | LocalCpls2D of LocalCpls2D

    type Param = {Name:string; Value: ParamValue; CanChangeAtRunTime: bool}

module Parameters =
    let UpdateBoParam = {Param.Name="name"; Value=ParamValue.Bool{BoolParam.Value=true;}; CanChangeAtRunTime=true}

    let UpdateFrequencyParam = {Param.Name="UpdateFrequency"; Value=ParamValue.Int{Value=10; Min=1; Max=50}; CanChangeAtRunTime=true}
    let ArrayStrideParam = {Param.Name="ArrayStride"; Value=ParamValue.Int{Value=128; Min=8; Max=1024}; CanChangeAtRunTime=false}
    let StartSeedParam = {Param.Name="StartSeed"; Value=ParamValue.Int{Value=123; Min=1; Max=32000}; CanChangeAtRunTime=false}
    let MemSeedParam = {Param.Name="MemSeed"; Value=ParamValue.Int{Value=123; Min=1; Max=32000}; CanChangeAtRunTime=false}
    let CnxnSeedParam = {Param.Name="CnxnSeed"; Value=ParamValue.Int{Value=123; Min=1; Max=32000}; CanChangeAtRunTime=false}
    let MemCountParam = {Param.Name="MemCount"; Value=ParamValue.Int{Value=10; Min=1; Max=1024}; CanChangeAtRunTime=false}
    let EnsembleCountParam = {Param.Name="EnsembleCount"; Value=ParamValue.Int{Value=50; Min=1; Max=1000}; CanChangeAtRunTime=false}
    let NodeCountParam = {Param.Name="NodeCount"; Value=ParamValue.Int{Value=50; Min=1; Max=1000}; CanChangeAtRunTime=false}


    let StepSizeParam = {Param.Name="StepSize"; Value=ParamValue.Float{Value=0.1f; Min=0.01f; Max=1.0f}; CanChangeAtRunTime=true}
    let NoiseParam = {Param.Name="Noise"; Value=ParamValue.Float{Value=0.1f; Min=0.01f; Max=1.0f}; CanChangeAtRunTime=true}
    let StartMagParam = {Param.Name="StartMag"; Value=ParamValue.Float{Value=0.1f; Min=0.0f; Max=1.0f}; CanChangeAtRunTime=false}
    let CnxnMagParam = {Param.Name="CnxnMag"; Value=ParamValue.Float{Value=0.1f; Min=0.0f; Max=1.0f}; CanChangeAtRunTime=true}
    let StepSize_XParam = {Param.Name="StepSize_X"; Value=ParamValue.Float{Value=0.1f; Min=0.0f; Max=1.0f}; CanChangeAtRunTime=true}
    let StepSize_YParam = {Param.Name="StepSize_Y"; Value=ParamValue.Float{Value=0.1f; Min=0.0f; Max=1.0f}; CanChangeAtRunTime=true}
    let TwistBiasParam = {Param.Name="TwistBias"; Value=ParamValue.Float{Value=0.1f; Min=0.0f; Max=1.0f}; CanChangeAtRunTime=true}

    let LinearLocalSet =
        [|
            ArrayStrideParam;
            StepSizeParam;
            StartSeedParam;
            NoiseParam;
        |] |> Array.map(fun p -> p.Name, p)  |> Dict.ofArray

    let RandomCliqueSet =
        [|
            NodeCountParam;
            StepSizeParam;
            StartSeedParam;
            NoiseParam;
            EnsembleCountParam;
        |] |> Array.map(fun p -> p.Name, p)  |> Dict.ofArray

    let RingSet =
        [|
            ArrayStrideParam;
            StepSizeParam;
            NoiseParam;
        |] |> Array.map(fun p -> p.Name, p)  |> Dict.ofArray

    let TwisterSet =
        [|
            ArrayStrideParam;
            StepSize_XParam;
            StepSize_YParam;
            TwistBiasParam;
        |] |> Array.map(fun p -> p.Name, p)  |> Dict.ofArray

    let SpotSet =
        [|
            ArrayStrideParam;
            StepSize_XParam;
            StepSize_YParam;
            TwistBiasParam;
        |] |> Array.map(fun p -> p.Name, p)  |> Dict.ofArray


    let GetParamByName (prams:IDictionary<string,Param>) (name:string) =
        if prams.ContainsKey(name) then
            Rop.succeed prams.[name]
        else
            Rop.fail (sprintf "Name: %s not found in dictionary" name)

    let GetParamValueBool pram =
        match pram.Value with
        | Bool bp -> Rop.succeed bp.Value
        | Enum _ -> Rop.fail (sprintf "Param: %s is Enum not bool" pram.Name)
        | Float _ -> Rop.fail (sprintf "Param: %s is Float not bool" pram.Name)
        | Int _ -> Rop.fail (sprintf "Param: %s is Int not bool" pram.Name)
        | LocalCpls1D _ -> Rop.fail (sprintf "Param: %s is LocalCpls1D not bool" pram.Name)
        | LocalCpls2D _ -> Rop.fail (sprintf "Param: %s is LocalCpls2D not bool" pram.Name)

    let GetParamValueEnum pram =
        match pram.Value with
        | Bool _ -> Rop.fail (sprintf "Param: %s is bool not enum" pram.Name)
        | Enum ep -> Rop.succeed ep.Value
        | Float _ -> Rop.fail (sprintf "Param: %s is Float not enum" pram.Name)
        | Int _ -> Rop.fail (sprintf "Param: %s is Int not enum" pram.Name)
        | LocalCpls1D _ -> Rop.fail (sprintf "Param: %s is LocalCpls1D not enum" pram.Name)
        | LocalCpls2D _ -> Rop.fail (sprintf "Param: %s is LocalCpls2D not enum" pram.Name)

    let GetParamValueFloat32 pram =
        match pram.Value with
        | Bool _ -> Rop.fail (sprintf "Param: %s is bool not float" pram.Name)
        | Enum _ -> Rop.fail (sprintf "Param: %s is Enum not float" pram.Name)
        | Float fp -> Rop.succeed fp.Value
        | Int _ -> Rop.fail (sprintf "Param: %s is Int not float" pram.Name)
        | LocalCpls1D _ -> Rop.fail (sprintf "Param: %s is LocalCpls1D not float" pram.Name)
        | LocalCpls2D _ -> Rop.fail (sprintf "Param: %s is LocalCpls2D not float" pram.Name)

    let GetParamValueInt pram =
        match pram.Value with
        | Bool _ -> Rop.fail (sprintf "Param: %s is bool not int" pram.Name)
        | Enum _ -> Rop.fail (sprintf "Param: %s is Enum not int" pram.Name)
        | Float _ -> Rop.fail (sprintf "Param: %s is Float not int" pram.Name)
        | Int ip -> Rop.succeed ip.Value
        | LocalCpls1D _ -> Rop.fail (sprintf "Param: %s is LocalCpls1D not int" pram.Name)
        | LocalCpls2D _ -> Rop.fail (sprintf "Param: %s is LocalCpls2D not int" pram.Name)

    let GetParamValueLocalCpls1D pram =
        match pram.Value with
        | Bool _ -> Rop.fail (sprintf "Param: %s is bool not LocalCpls1D" pram.Name)
        | Enum _ -> Rop.fail (sprintf "Param: %s is Enum not LocalCpls1D" pram.Name)
        | Float _ -> Rop.fail (sprintf "Param: %s is Float not LocalCpls1D" pram.Name)
        | Int _ -> Rop.fail (sprintf "Param: %s is Int not LocalCpls1D" pram.Name)
        | LocalCpls1D ip -> Rop.succeed ip
        | LocalCpls2D _ -> Rop.fail (sprintf "Param: %s is LocalCpls2D not LocalCpls1D" pram.Name)

    let GetParamValueLocalCpls2D pram =
        match pram.Value with
        | Bool _ -> Rop.fail (sprintf "Param: %s is bool not LocalCpls2D" pram.Name)
        | Enum _ -> Rop.fail (sprintf "Param: %s is Enum not LocalCpls2D" pram.Name)
        | Float _ -> Rop.fail (sprintf "Param: %s is Float not LocalCpls2D" pram.Name)
        | Int _ -> Rop.fail (sprintf "Param: %s is Int not LocalCpls2D" pram.Name)
        | LocalCpls1D _ -> Rop.fail (sprintf "Param: %s is LocalCpls1D not LocalCpls2D" pram.Name)
        | LocalCpls2D _ -> Rop.succeed pram.Value

    let GetBoolParam (prams:IDictionary<string,Param>) (name:string) =
        GetParamValueBool >>= (GetParamByName prams name)

    let GetEnumParam (prams:IDictionary<string,Param>) (name:string) =
        GetParamValueEnum >>= (GetParamByName prams name)

    let GetFloat32Param (prams:IDictionary<string,Param>) (name:string) =
        GetParamValueFloat32 >>= (GetParamByName prams name)

    let GetIntParam (prams:IDictionary<string,Param>) (name:string) =
        GetParamValueInt >>= (GetParamByName prams name)

    let GetLocalCpls1DParam (prams:IDictionary<string,Param>) (name:string) =
        GetParamValueLocalCpls1D >>= (GetParamByName prams name)

    let GetLocalCpls2DParam (prams:IDictionary<string,Param>) (name:string) =
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