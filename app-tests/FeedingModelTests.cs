using Microsoft.Extensions.Logging;
using Moq;
using app.Pages;
using app.Services;
using app.Models;

namespace app_tests
{
    [TestClass]
    public class FeedingModelTests
    {
        private Mock<IFeedingService> _mockFeedingService = null!;
        private Mock<ILogger<FeedingModel>> _mockLogger = null!;
        private FeedingModel _feedingModel = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockFeedingService = new Mock<IFeedingService>();
            _mockLogger = new Mock<ILogger<FeedingModel>>();
            _feedingModel = new FeedingModel(_mockFeedingService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public void Constructor_InitializesCorrectly()
        {
            // Assert
            Assert.IsNotNull(_feedingModel);
        }

        [TestMethod]
        public void OnGet_GeneratesReport()
        {
            // Arrange
            var expectedReport = new FeedingReport
            {
                TotalFeedings = 5,
                AverageAmount = 2.5
            };
            _mockFeedingService.Setup(s => s.GenerateReport(null, null))
                .Returns(expectedReport);

            // Act
            _feedingModel.OnGet();

            // Assert
            Assert.IsNotNull(_feedingModel.Report);
            Assert.AreEqual(5, _feedingModel.Report.TotalFeedings);
            _mockFeedingService.Verify(s => s.GenerateReport(null, null), Times.Once);
        }

        [TestMethod]
        public void OnPost_RecordsFeedingEvent()
        {
            // Arrange
            _feedingModel.FoodType = "Flakes";
            _feedingModel.Amount = 2.5;
            _feedingModel.FishBehavior = "Active";
            _feedingModel.Notes = "Test";

            // Act
            var result = _feedingModel.OnPost();

            // Assert
            _mockFeedingService.Verify(s => s.RecordFeeding(
                It.Is<FeedingEvent>(e => 
                    e.FoodType == "Flakes" && 
                    e.Amount == 2.5 && 
                    e.FishBehavior == "Active")), 
                Times.Once);
        }

        [TestMethod]
        public void FeedingModel_InheritsFromPageModel()
        {
            // Assert
            Assert.IsInstanceOfType(_feedingModel, typeof(Microsoft.AspNetCore.Mvc.RazorPages.PageModel));
        }
    }
}
