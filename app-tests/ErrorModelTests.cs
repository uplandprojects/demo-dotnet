using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;
using app.Pages;

namespace app_tests
{
    [TestClass]
    public class ErrorModelTests
    {
        private Mock<ILogger<ErrorModel>> _mockLogger = null!;
        private Mock<HttpContext> _mockHttpContext = null!;
        private ErrorModel _errorModel = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<ErrorModel>>();
            _mockHttpContext = new Mock<HttpContext>();
            _errorModel = new ErrorModel(_mockLogger.Object);
        }

        [TestMethod]
        public void Constructor_SetsLogger()
        {
            // Arrange & Act
            var errorModel = new ErrorModel(_mockLogger.Object);

            // Assert
            Assert.IsNotNull(errorModel);
        }

        [TestMethod]
        public void ShowRequestId_ReturnsFalse_WhenRequestIdIsNull()
        {
            // Arrange
            _errorModel.RequestId = null;

            // Act
            var result = _errorModel.ShowRequestId;

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShowRequestId_ReturnsFalse_WhenRequestIdIsEmpty()
        {
            // Arrange
            _errorModel.RequestId = string.Empty;

            // Act
            var result = _errorModel.ShowRequestId;

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ShowRequestId_ReturnsTrue_WhenRequestIdHasValue()
        {
            // Arrange
            _errorModel.RequestId = "test-request-id";

            // Act
            var result = _errorModel.ShowRequestId;

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void OnGet_SetsRequestIdFromCurrentActivity()
        {
            // Arrange
            using var activity = new Activity("test").Start();
            activity.SetIdFormat(ActivityIdFormat.W3C);

            // Act
            _errorModel.OnGet();

            // Assert
            Assert.IsNotNull(_errorModel.RequestId);
        }

        [TestMethod]
        public void OnGet_SetsRequestIdFromHttpContext_WhenNoCurrentActivity()
        {
            // Arrange
            Activity.Current = null;
            var traceId = "test-trace-id";
            _mockHttpContext.Setup(x => x.TraceIdentifier).Returns(traceId);
            
            // Set up the PageContext to use our mock HttpContext
            _errorModel.PageContext = new PageContext
            {
                HttpContext = _mockHttpContext.Object
            };

            // Act
            _errorModel.OnGet();

            // Assert
            Assert.AreEqual(traceId, _errorModel.RequestId);
        }

        [TestMethod]
        public void RequestId_CanBeSetAndRetrieved()
        {
            // Arrange
            var requestId = "custom-request-id";

            // Act
            _errorModel.RequestId = requestId;

            // Assert
            Assert.AreEqual(requestId, _errorModel.RequestId);
        }
    }
}