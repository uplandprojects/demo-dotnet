using Microsoft.Extensions.Logging;
using Moq;
using app.Services;
using app.Models;

namespace app_tests
{
    [TestClass]
    public class FeedingServiceTests
    {
        private Mock<ILogger<FeedingService>> _mockLogger = null!;
        private FeedingService _feedingService = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<FeedingService>>();
            _feedingService = new FeedingService(_mockLogger.Object);
        }

        [TestMethod]
        public void RecordFeeding_AddsEventToList()
        {
            // Arrange
            var feedingEvent = new FeedingEvent
            {
                Timestamp = DateTime.Now,
                FoodType = "Flakes",
                Amount = 2.5,
                FishBehavior = "Active",
                Notes = "Test feeding"
            };

            // Act
            _feedingService.RecordFeeding(feedingEvent);
            var feedings = _feedingService.GetAllFeedings();

            // Assert
            Assert.AreEqual(1, feedings.Count);
            Assert.AreEqual("Flakes", feedings[0].FoodType);
        }

        [TestMethod]
        public void RecordFeeding_AssignsIdIfEmpty()
        {
            // Arrange
            var feedingEvent = new FeedingEvent
            {
                Timestamp = DateTime.Now,
                FoodType = "Pellets",
                Amount = 3.0,
                FishBehavior = "Normal"
            };

            // Act
            _feedingService.RecordFeeding(feedingEvent);

            // Assert
            Assert.AreNotEqual(Guid.Empty, feedingEvent.Id);
        }

        [TestMethod]
        public void GetAllFeedings_ReturnsDescendingOrder()
        {
            // Arrange
            var oldFeeding = new FeedingEvent
            {
                Timestamp = DateTime.Now.AddDays(-2),
                FoodType = "Flakes",
                Amount = 2.0,
                FishBehavior = "Normal"
            };
            var newFeeding = new FeedingEvent
            {
                Timestamp = DateTime.Now,
                FoodType = "Pellets",
                Amount = 3.0,
                FishBehavior = "Active"
            };

            // Act
            _feedingService.RecordFeeding(oldFeeding);
            _feedingService.RecordFeeding(newFeeding);
            var feedings = _feedingService.GetAllFeedings();

            // Assert
            Assert.AreEqual(2, feedings.Count);
            Assert.IsTrue(feedings[0].Timestamp > feedings[1].Timestamp);
        }

        [TestMethod]
        public void GenerateReport_CalculatesCorrectAverageAmount()
        {
            // Arrange
            _feedingService.RecordFeeding(new FeedingEvent
            {
                Timestamp = DateTime.Now,
                FoodType = "Flakes",
                Amount = 2.0,
                FishBehavior = "Active"
            });
            _feedingService.RecordFeeding(new FeedingEvent
            {
                Timestamp = DateTime.Now,
                FoodType = "Pellets",
                Amount = 4.0,
                FishBehavior = "Normal"
            });

            // Act
            var report = _feedingService.GenerateReport();

            // Assert
            Assert.AreEqual(3.0, report.AverageAmount, 0.001);
        }

        [TestMethod]
        public void GenerateReport_CountsTotalFeedings()
        {
            // Arrange
            for (int i = 0; i < 5; i++)
            {
                _feedingService.RecordFeeding(new FeedingEvent
                {
                    Timestamp = DateTime.Now,
                    FoodType = "Flakes",
                    Amount = 2.0,
                    FishBehavior = "Active"
                });
            }

            // Act
            var report = _feedingService.GenerateReport();

            // Assert
            Assert.AreEqual(5, report.TotalFeedings);
        }

        [TestMethod]
        public void GenerateReport_IdentifiesMostCommonBehavior()
        {
            // Arrange
            _feedingService.RecordFeeding(new FeedingEvent
            {
                Timestamp = DateTime.Now,
                FoodType = "Flakes",
                Amount = 2.0,
                FishBehavior = "Active"
            });
            _feedingService.RecordFeeding(new FeedingEvent
            {
                Timestamp = DateTime.Now,
                FoodType = "Pellets",
                Amount = 3.0,
                FishBehavior = "Active"
            });
            _feedingService.RecordFeeding(new FeedingEvent
            {
                Timestamp = DateTime.Now,
                FoodType = "Flakes",
                Amount = 2.5,
                FishBehavior = "Normal"
            });

            // Act
            var report = _feedingService.GenerateReport();

            // Assert
            Assert.AreEqual("Active", report.MostCommonBehavior);
        }

        [TestMethod]
        public void GenerateReport_DetectsHighFeedingFrequency()
        {
            // Arrange
            for (int i = 0; i < 6; i++)
            {
                _feedingService.RecordFeeding(new FeedingEvent
                {
                    Timestamp = DateTime.Now.AddHours(-i),
                    FoodType = "Flakes",
                    Amount = 2.0,
                    FishBehavior = "Active"
                });
            }

            // Act
            var report = _feedingService.GenerateReport();

            // Assert
            Assert.IsTrue(report.Alerts.Any(a => a.Contains("High feeding frequency")));
        }

        [TestMethod]
        public void GenerateReport_DetectsUnusualBehavior()
        {
            // Arrange
            for (int i = 0; i < 5; i++)
            {
                _feedingService.RecordFeeding(new FeedingEvent
                {
                    Timestamp = DateTime.Now,
                    FoodType = "Flakes",
                    Amount = 2.0,
                    FishBehavior = "Lethargic"
                });
            }

            // Act
            var report = _feedingService.GenerateReport();

            // Assert
            Assert.IsTrue(report.Alerts.Any(a => a.Contains("Unusual behavior patterns")));
        }

        [TestMethod]
        public void ExportFeedingData_ReturnsJsonString()
        {
            // Arrange
            _feedingService.RecordFeeding(new FeedingEvent
            {
                Timestamp = DateTime.Now,
                FoodType = "Flakes",
                Amount = 2.0,
                FishBehavior = "Active",
                Notes = "Test"
            });

            // Act
            var exportData = _feedingService.ExportFeedingData();

            // Assert
            Assert.IsTrue(exportData.Contains("Flakes"));
            Assert.IsTrue(exportData.Contains("ExportedAt"));
            Assert.IsTrue(exportData.Contains("TotalRecords"));
        }

        [TestMethod]
        public void GenerateReport_WithEmptyData_ReturnsValidReport()
        {
            // Act
            var report = _feedingService.GenerateReport();

            // Assert
            Assert.AreEqual(0, report.TotalFeedings);
            Assert.AreEqual(0, report.AverageAmount);
            Assert.AreEqual("No data", report.MostCommonBehavior);
        }

        [TestMethod]
        public void GenerateReport_FiltersDateRange()
        {
            // Arrange
            _feedingService.RecordFeeding(new FeedingEvent
            {
                Timestamp = new DateTime(2024, 1, 1),
                FoodType = "Flakes",
                Amount = 2.0,
                FishBehavior = "Active"
            });
            _feedingService.RecordFeeding(new FeedingEvent
            {
                Timestamp = new DateTime(2024, 6, 1),
                FoodType = "Pellets",
                Amount = 3.0,
                FishBehavior = "Normal"
            });

            // Act
            var report = _feedingService.GenerateReport(
                new DateTime(2024, 5, 1), 
                new DateTime(2024, 7, 1)
            );

            // Assert
            Assert.AreEqual(1, report.TotalFeedings);
            Assert.AreEqual(3.0, report.AverageAmount);
        }
    }
}
