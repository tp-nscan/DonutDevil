//using System;
//using System.Collections.Generic;
//using MathLib.NumericTypes;

//namespace NodeLib.Indexers
//{
//    internal class D2IndexerTorusOfIntervalR : D2IndexerBase<float>
//    {
//        public D2IndexerTorusOfIntervalR(
//            string name,
//            Func<NodeGroup, IEnumerable<D2Val<float>>> indexingFunc,
//            int stride) :
//            base(name, indexingFunc, stride, f => f, f => f)
//        {
//        }
//    }

//    internal class D2IndexerTorusOfIntervalZ : D2IndexerBase<float>
//    {
//        public D2IndexerTorusOfIntervalZ(
//            string name,
//            Func<NodeGroup, IEnumerable<D2Val<float>>> indexingFunc,
//            int stride) :
//            base(name, indexingFunc, stride, f => f / 2 + 0.5f, f => f * 2 - 1.0f)
//        {
//        }
//    }

//    internal class D2IndexerTorusOfRing : D2IndexerBase<float>
//    {
//        public D2IndexerTorusOfRing(
//            string name,
//            Func<NodeGroup, IEnumerable<D2Val<float>>> indexingFunc,
//            int stride) :
//            base(name, indexingFunc, stride, f => f, f => f)
//        {
//        }
//    }

//    internal class D2IndexerTorusOfTorus : D2IndexerBase<float>
//    {
//        public D2IndexerTorusOfTorus(
//            string name,
//            Func<NodeGroup, IEnumerable<D2Val<float>>> indexingFunc,
//            int stride,
//            Func<float, float> valuesToUnitRange,
//            Func<float, float> unitRangeToValues) :
//            base(name, indexingFunc, stride, valuesToUnitRange, unitRangeToValues)
//        {
//        }
//    }

//    internal class D2IndexerTorusOfSpheres : D2IndexerBase<float>
//    {

//        public D2IndexerTorusOfSpheres(
//            string name,
//            Func<NodeGroup, IEnumerable<D2Val<float>>> indexingFunc,
//            int stride,
//            Func<float, float> valuesToUnitRange,
//            Func<float, float> unitRangeToValues) :
//            base(name, indexingFunc, stride, valuesToUnitRange, unitRangeToValues)
//        {
//        }

//        private IndexerDataType _indexerDataType;
//        public override IndexerDataType IndexerDataType
//        {
//            get { return _indexerDataType; }
//        }

//        private D2ArrayShape _d2ArrayShape;
//        public override D2ArrayShape D2ArrayShape
//        {
//            get { return _d2ArrayShape; }
//        }
//    }

//    internal class D2IndexerSquareMatrix : D2IndexerBase<float>
//    {
//        public D2IndexerSquareMatrix(
//            string name,
//            Func<NodeGroup, IEnumerable<D2Val<float>>> indexingFunc,
//            int stride,
//            Func<float, float> valuesToUnitRange,
//            Func<float, float> unitRangeToValues) :
//            base(name, indexingFunc, stride, valuesToUnitRange, unitRangeToValues)
//        {
//        }


//        public override IndexerDataType IndexerDataType
//        {
//            get { throw new NotImplementedException(); }
//        }

//        public override D2ArrayShape D2ArrayShape
//        {
//            get { throw new NotImplementedException(); }
//        }
//    }

//}