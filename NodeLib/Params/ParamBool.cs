﻿using System;

namespace NodeLib.Params
{
    public class ParamBool : IParameter
    {
        public ParamBool(bool value, string name)
        {
            _value = value;
            _name = name;
        }

        public ParamType ParamType
        {
            get { return ParamType.Bool; }
        }

        public Type Type
        {
            get { return typeof(bool); }
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private readonly object _value;
        public object Value
        {
            get { return _value; }
        }
    }
}