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

        private static readonly IEnumerable<IChangeableData> s_emptyData = new List<IChangeableData>();

        [TestCaseSource("NullParameters")]
        public void DntNotNalyzeIfInputParameterIsNull(IEnumerable<IChangeableData> a, IEnumerable<IChangeableData> b)
        {
            List<string> detectedItems = new List<string>();
            DataAnalyzer analyzer = new DataAnalyzer();
            analyzer.DetectedDifferenceEvent += (o, e) => detectedItems.Add(e);

            analyzer.Analyze(a, b);
            Assert.IsEmpty(detectedItems);
        }

        static object[] NullParameters = { new object[] { null, new []{ new FakeChangeableData()} },
                                        new object[] { new []{ new FakeChangeableData() }, null }};

        static object[] Parameters = { new [] { s_emptyData, DataTree() } ,
                                       new [] { DataTree(), s_emptyData }
        };

        private static IEnumerable<IChangeableData> DataTree()
        {
            IChangeableData parent = new FakeChangeableData("parentname", "parentnamevalue");
            IChangeableData child = new FakeChangeableData("childname", "childvalue");
            IChangeableData subchild = new FakeChangeableData("subchildname", "subchildvalue");
            child.Childs = new[] { subchild };
            parent.Childs = new[] { child };

            return new[] { parent };
        }

        [TestCaseSource("TestCases")]
        public IEnumerable<string> DonNotAnalyzeIfInputParameterIsNull(IEnumerable<IChangeableData> a, IEnumerable<IChangeableData> b)
        {
            List<string> detectedItems = new List<string>();

            DataAnalyzer analyzer = new DataAnalyzer();
            analyzer.DetectedDifferenceEvent += (o, e) => detectedItems.Add(e);
            analyzer.Analyze(a, b);
            return detectedItems;
        }


        public static IEnumerable TestCases()
        {
            yield return new TestCaseData(s_emptyData, DataTree()).Returns(new[] { Format(Event_DataIsNotActual, "parentname", "parentnamevalue") });
            yield return new TestCaseData(DataTree(), s_emptyData).Returns(new[] { Format(Event_NewInfo, "parentname", "parentnamevalue") });
            yield return new TestCaseData(DataTree(), DataTree()).Returns(s_emptyData);
            yield return new TestCaseData(ChangeData(DataTree(), x => x.First().Name = "newParentname"),
                                          DataTree()).Returns(new[] { Format(Event_NewInfo, "newParentname", "parentnamevalue"),
                                                                       Format(Event_DataIsNotActual, "parentname", "parentnamevalue")});
        }

        private static IEnumerable<IChangeableData> ChangeData(IEnumerable<IChangeableData> data,
                                                               Action<IEnumerable<FakeChangeableData>> changeAction)
        {
            changeAction?.Invoke(data.Cast<FakeChangeableData>());
            return data;
        }
    }
}
