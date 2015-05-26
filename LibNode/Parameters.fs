namespace LibNode
open System
open MathUtils

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

module Parameters=
    let UpdateBoParam = {Param.Name="name"; Value=ParamValue.Bool{BoolParam.Value=true;}; CanChangeAtRunTime=true}

    let UpdateFrequencyParam = {Param.Name="UpdateFrequency"; Value=ParamValue.Int{Value=10; Min=1; Max=50}; CanChangeAtRunTime=true}
    let ArrayStrideParam = {Param.Name="ArrayStride"; Value=ParamValue.Int{Value=128; Min=8; Max=1024}; CanChangeAtRunTime=false}
    let StartSeedParam = {Param.Name="StartSeed"; Value=ParamValue.Int{Value=123; Min=1; Max=32000}; CanChangeAtRunTime=false}
    let MemSeedParam = {Param.Name="MemSeed"; Value=ParamValue.Int{Value=123; Min=1; Max=32000}; CanChangeAtRunTime=false}
    let CnxnSeedParam = {Param.Name="CnxnSeed"; Value=ParamValue.Int{Value=123; Min=1; Max=32000}; CanChangeAtRunTime=false}
    let MemCountParam = {Param.Name="MemCount"; Value=ParamValue.Int{Value=10; Min=1; Max=1024}; CanChangeAtRunTime=false}
    let EnsembleCountParam = {Param.Name="EnsembleCount"; Value=ParamValue.Int{Value=50; Min=1; Max=1000}; CanChangeAtRunTime=false}


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
            NoiseParam;
        |] |> Array.map(fun p -> p.Name, p.Value)  |> Dict.ofArray

    let RandomCliqueSet =
        [|
            ArrayStrideParam;
            StepSizeParam;
            NoiseParam;
        |] |> Array.map(fun p -> p.Name, p.Value)  |> Dict.ofArray

    let RingSet =
        [|
            ArrayStrideParam;
            StepSizeParam;
            NoiseParam;
        |] |> Array.map(fun p -> p.Name, p.Value)  |> Dict.ofArray

    let TwisterSet =
        [|
            ArrayStrideParam;
            StepSize_XParam;
            StepSize_YParam;
            TwistBiasParam;
        |] |> Array.map(fun p -> p.Name, p.Value)  |> Dict.ofArray

    let SpotSet =
        [|
            ArrayStrideParam;
            StepSize_XParam;
            StepSize_YParam;
            TwistBiasParam;
        |] |> Array.map(fun p -> p.Name, p.Value)  |> Dict.ofArray

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