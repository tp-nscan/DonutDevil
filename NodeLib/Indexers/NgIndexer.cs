﻿using System;
using System.Collections.Generic;
using System.Linq;
using MathLib.NumericTypes;

namespace NodeLib.Indexers
{
    public interface INgIndexer
    {
        string Name { get; }
        NgIndexerType NgIndexerType { get; }
        Func<INodeGroup, IEnumerable<D2Val<float>>> IndexingFunc { get; }
        int Height { get; }
        int Width { get; }
    }

    public enum NgIndexerType
    {
        D2Float,
        D2Point
    }

    public static class NgIndexer
    {
        public static INgIndexer MakeD2Float(string name, int squareSize, int offset=0)
        {
            return new NgIndexerImpl(
                name: name,
                indexingFunc:
                n => Enumerable.Range(offset, offset + squareSize*squareSize)
                                .Select(i => new D2Val<float>
                                    (
                                        (i - offset) / squareSize, 
                                        (i - offset) % squareSize, 
                                        n.Values[i])
                                    ),
                height: squareSize,
                width: squareSize
            );
        }
    }

    public class NgIndexerImpl : INgIndexer
    {
        private readonly string _name;
        private readonly Func<INodeGroup, IEnumerable<D2Val<float>>> _indexingFunc;
        private readonly int _height;
        private readonly int _width;

        public NgIndexerImpl(
            string name, 
            Func<INodeGroup, IEnumerable<D2Val<float>>> indexingFunc, 
            int height, 
            int width
            )
        {
            _name = name;
            _indexingFunc = indexingFunc;
            _height = height;
            _width = width;
        }

        public string Name
        {
            get { return _name; }
        }

        public NgIndexerType NgIndexerType
        {
            get { return NgIndexerType.D2Float; }
        }

        public Func<INodeGroup, IEnumerable<D2Val<float>>> IndexingFunc
        {
            get { return _indexingFunc; }
        }

        public int Height
        {
            get { return _height; }
        }

        public int Width
        {
            get { return _width; }
        }
    }
}