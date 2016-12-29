using NUnit.Framework;
using System.Collections.Generic;
using NoCompany.Interfaces;
using System.Collections;
using static NoCompany.DataAnalyzer.Properties.Resources;
using System.Linq;
using System;
using static System.String;

namespace NoCompany.DataAnalyzer.Tests
{

    [TestFixture]
    public class DataAnalyzerTests
    {
        private const string c_newParentname = "newParentName";
        private const string c_parentname = "parentname";
        private const string c_parentvalue = "parentnamevalue";
        private const string c_childname = "childname";
        private const string c_childvalue = "childvalue";
        private const string c_subchildname = "subchildname";
        private const string c_subchildvalue = "subchildvalue";
        private const string c_newChildname = "newChildname";
        private const string c_newParentValue = "newParentValue";
        private static readonly IEnumerable<IChangeableData> s_emptyData = new List<IChangeableData>();

        [TestCaseSource("NullParameters")]
        public void DntNotAnalyzeIfSomeOfInputParametersAreNull(IEnumerable<IChangeableData> a, IEnumerable<IChangeableData> b)
        {
            List<string> detectedItems = new List<string>();
            DataAnalyzer analyzer = new DataAnalyzer();
            analyzer.DetectedDifferenceEvent += (o, e) => detectedItems.Add(e);

            analyzer.Analyze(a, b);
            Assert.IsEmpty(detectedItems);
        }

        static object[] NullParameters = { new object[] { null, new []{ new FakeChangeableData()} },
                                        new object[] { new []{ new FakeChangeableData() }, null }};



        private static IEnumerable<IChangeableData> DataTree()
        {
            IChangeableData parent = new FakeChangeableData(c_parentname, c_parentvalue);
            IChangeableData child = new FakeChangeableData(c_childname, c_childvalue);
            IChangeableData subchild = new FakeChangeableData(c_subchildname, c_subchildvalue);
            child.Childs = new[] { subchild };
            parent.Childs = new[] { child };

            return new[] { parent };
        }

        [TestCaseSource("TestCases")]
        public List<string> SimpleTestCases(IEnumerable<IChangeableData> a, IEnumerable<IChangeableData> b)
        {
            List<string> detectedItems = new List<string>();

            DataAnalyzer analyzer = new DataAnalyzer();
            analyzer.DetectedDifferenceEvent += (o, e) => detectedItems.Add(e);
            analyzer.Analyze(a, b);
            return detectedItems;
        }


        public static IEnumerable TestCases()
        {
            yield return new TestCaseData(s_emptyData, DataTree())
                .Returns(new List<string>() { Format(Event_DataIsNotActual, c_parentname, c_parentvalue) })
                .SetName("New presaved data");
            yield return new TestCaseData(DataTree(), s_emptyData)
                .Returns(new List<string>() { Format(Event_NewInfo, c_parentname, c_parentvalue) })
                .SetName("New external data");
            yield return new TestCaseData(DataTree(), DataTree())
                .Returns(s_emptyData)
                .SetName("No changes"); 
        }

        private static IEnumerable<IChangeableData> ChangedParent(Action<FakeChangeableData> changeAction)
        {
            var data = DataTree();
            changeAction(data.Cast<FakeChangeableData>().Single());
            return data;
        }

        private static IEnumerable<IChangeableData> ChangedChild(Action<FakeChangeableData> changeAction)
        {
            var data = DataTree();
            changeAction(data.Single().Childs.Cast<FakeChangeableData>().Single());
            return data;
        }


        [TestCaseSource("ChangedDataCases")]
        public IEnumerable<string> ChangedDataTest(IEnumerable<IChangeableData> a, IEnumerable<IChangeableData> b)
        {

            List<string> detectedItems = new List<string>();

            DataAnalyzer analyzer = new DataAnalyzer();
            analyzer.DetectedDifferenceEvent += (o, e) => detectedItems.Add(e);
            analyzer.Analyze(a, b);
            return detectedItems;
        }

        public static IEnumerable ChangedDataCases()
        {
            yield return new TestCaseData(ChangedParent(x => x.Name = c_newParentname), DataTree())
                                          .Returns(new[] { Format(Event_NewInfo, c_newParentname, c_parentvalue),
                                                           Format(Event_DataIsNotActual, c_parentname, c_parentvalue)})
                                          .SetName("Changed parent name");
            yield return new TestCaseData(ChangedChild(x => x.Name = c_newChildname), DataTree())
                                          .Returns(new[] { Format(Event_NewInfo, c_newChildname, c_childvalue),
                                                           Format(Event_DataIsNotActual, c_childname, c_childvalue)})
                                          .SetName("Changed child name");

            yield return new TestCaseData(ChangedParent(x => x.Value = c_newParentValue), DataTree())
                              .Returns(new[] { Format(Event_ItemsAreNotEqual, c_parentvalue, c_newParentValue) })
                              .SetName("Changed parent value");
        }

    }
}
