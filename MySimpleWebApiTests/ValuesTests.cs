using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySimpleWebApi.Controllers;

namespace MySimpleWebApiTests
{
    [TestClass]
    public class ValuesTests : PostgresTestBase
    {
        [TestMethod]
        public void Get_NoValuesSaved_ReturnEmptyList()
        {
            var controller = new ValuesController();

            var result = controller.Get();

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetById_NoValuesSaved_ReturnNull()
        {
            var controller = new ValuesController();

            var result = controller.Get(1);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Post_SingleValue_GetByIdReturnsValue()
        {
            var controller = new ValuesController();

            var postResult = controller.Post("1234");
            var result = controller.Get(postResult);

            Assert.AreEqual("1234", result);
        }

        [TestMethod]
        public void Post_ThreeValues_GetReturnsAllValues()
        {
            var controller = new ValuesController();

            controller.Post("1");
            controller.Post("2");
            controller.Post("3");
            var result = controller.Get();

            CollectionAssert.AreEquivalent(new[]{"1","2","3"}, result.ToArray());
        }

        [TestMethod]
        public void Delete_AddOneValueThenDeleteIt_GetReturnsEmptySquance()
        {
            var controller = new ValuesController();

            var postResult = controller.Post("1");
            controller.Delete(postResult);
            var result = controller.Get();

            Assert.AreEqual(0, result.Count());
        }
    }
}
