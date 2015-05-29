namespace LibNode
open System
open MathUtils


    exception ErrorStr of string

    type ArrayLength = int

//    type GpuFloat32Array = { DeviceMemory: DeviceMemory<float32>; ArrayLength: ArrayLength }
//
//    type Float32CpuGpu(cpuArray:Option<float32[]>,
//                       gpuArray:Option<GpuFloat32Array>
//                       ) = 
//
//        let mutable _cpuArray = cpuArray;
//        let mutable _gpuArray = gpuArray
//
//        new(cpuArray) =
//            Float32CpuGpu(cpuArray, None)
//
//        new(gpuArray) =
//            Float32CpuGpu(None, gpuArray)
//
//        member this.CpuArray
//            with get() = _cpuArray
//
//        member this.GpuArray
//            with get() = _gpuArray
//
//        member this.CopyToGpu() =
//            match this.GpuArray with
//            | Some x -> ()
//            | None -> _gpuArray 
//                         <- Some({
//                                    DeviceMemory=CUBLAS.Default.Worker.Malloc(_cpuArray.Value); 
//                                    ArrayLength = _cpuArray.Value.Length })
//            
//        member this.CopyToCpu() =
//            match this.GpuArray with
//            | Some x -> _cpuArray <- Some(x.DeviceMemory.Gather())
//            | None -> ()


    type INamed<'a> =
        abstract member Data: unit -> 'a
        abstract member Name: string

    type NamedData<'a>(name:string, data:'a) =
        let _name = name
        let _data = data

        interface INamed<'a> with
            member this.Name =
                _name
            member this.Data() = 
                _data

        member this.Name  
            with get() = _name

        member this.Data  
            with get() = _data


    type NamedDataPtr<'a>(name:string, data:unit->'a) =
        let _name = name
        let _data = data

        interface INamed<'a> with
            member this.Name =
                _name
            member this.Data() = 
                _data()

        member this.Name  
            with get() = _name

        member this.Data  
            with get() = _data
