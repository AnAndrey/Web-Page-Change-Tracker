using NUnit.Framework;
using System.Collections.Generic;
using NoCompany.Interfaces;
using Moq;
using System.Linq;

namespace NoCompany.CommonClasess.Tests
{
    [TestFixture]
    public class ChageableDataExtensionsTests
    {
        const int c_itemsCount = 5;
        const int c_duplicatesCount = 3;

        [Test]
        public void ItemsToDictionaryExcludingDuplicates()
        {
            List<IChangeableData> items = null;
            List<IChangeableData> duplicates = null;

            CreateItemsAndDuplicates(out items, out duplicates);

            var res = duplicates.ToDictionary();
            CollectionAssert.AreEqual(res.Values.ToList(), items);            
        }

        [Test]
        public void ItemsToDictionaryExcludingDuplicatesAndReactOnDuplication()
        {
            List<IChangeableData> items = null;
            List<IChangeableData> duplicates = null;

            CreateItemsAndDuplicates(out items, out duplicates);

            int count = 0;
            var res = duplicates.ToDictionary( x=> count++);
            CollectionAssert.AreEqual(res.Values.ToList(), items);
            Assert.AreEqual(c_duplicatesCount, count);
        }

        private void CreateItemsAndDuplicates(out List<IChangeableData> items,out List<IChangeableData> duplicates)
        {
            items = new List<IChangeableData>();
            duplicates = new List<IChangeableData>();

            for (int t = 0; t < c_itemsCount; t++)
            {
                var item = new Mock<IChangeableData>();
                item.SetupGet(x => x.Name).Returns("name" + t);
                item.SetupGet(x => x.Value).Returns("value" + t);
                items.Add(item.Object);
                if (t < c_duplicatesCount)
                {
                    duplicates.Add(item.Object);
                }
            }
            duplicates.AddRange(items);
        }
    }
}
