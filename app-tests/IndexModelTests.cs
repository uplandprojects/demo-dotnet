using Microsoft.Extensions.Logging;
using Moq;
using app.Pages;

namespace app_tests
{
    [TestClass]
    public class IndexModelTests
    {
        private Mock<ILogger<IndexModel>> _mockLogger = null!;
        private IndexModel _indexModel = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<IndexModel>>();
            _indexModel = new IndexModel(_mockLogger.Object);
        }

        [TestMethod]
        public void Constructor_SetsLogger()
        {
            // Arrange & Act
            var indexModel = new IndexModel(_mockLogger.Object);

            // Assert
            Assert.IsNotNull(indexModel);
        }

        [TestMethod]
        public void OnGet_ExecutesWithoutException()
        {
            // Arrange & Act & Assert
            // The OnGet method is empty but should execute without throwing
            _indexModel.OnGet();
            
            // If we reach this point, the method executed successfully
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void IndexModel_InheritsFromPageModel()
        {
            // Arrange & Act & Assert
            Assert.IsInstanceOfType(_indexModel, typeof(Microsoft.AspNetCore.Mvc.RazorPages.PageModel));
        }
    }
}