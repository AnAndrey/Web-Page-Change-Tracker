using System;
using System.Collections.Generic;
using NoCompany.Interfaces;
using System.Linq;

namespace NoCompany.DataAnalyzer.Tests
{
    internal class FakeChangeableData : IChangeableData
    {
        IEnumerable<IChangeableData> _childs = null;
        public FakeChangeableData() { }

        public FakeChangeableData(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public IEnumerable<IChangeableData> Childs
        {
            get
            {
                return _childs;
            }

            set
            {
                _childs = value;
            }
        }

        public bool HasChilds{ get{ return _childs == null? false : _childs.Any(); }}

        public string Name { get; set; } = "fakename";
        public string Value { get; set; } = "fakevalue";

    }
}