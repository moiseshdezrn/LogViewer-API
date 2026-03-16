using LogViewer.Core.DTOs;

namespace LogViewer.Tests
{
    public class LogQueryParametersTests
    {
        [Fact]
        public void PageSize_WithValueAbove100_ClampsTo100()
        {
            // Arrange
            var parameters = new LogQueryParameters();

            // Act
            parameters.PageSize = 150;

            // Assert
            Assert.Equal(100, parameters.PageSize);
        }

        [Fact]
        public void PageSize_WithValueBelow1_ClampsTo1()
        {
            // Arrange
            var parameters = new LogQueryParameters();

            // Act
            parameters.PageSize = 0;

            // Assert
            Assert.Equal(1, parameters.PageSize);
        }

        [Fact]
        public void PageSize_WithNegativeValue_ClampsTo1()
        {
            // Arrange
            var parameters = new LogQueryParameters();

            // Act
            parameters.PageSize = -10;

            // Assert
            Assert.Equal(1, parameters.PageSize);
        }

        [Fact]
        public void PageSize_WithValidValue_SetsCorrectly()
        {
            // Arrange
            var parameters = new LogQueryParameters();

            // Act
            parameters.PageSize = 50;

            // Assert
            Assert.Equal(50, parameters.PageSize);
        }

        [Fact]
        public void DefaultValues_AreSetCorrectly()
        {
            // Arrange & Act
            var parameters = new LogQueryParameters();

            // Assert
            Assert.Equal(1, parameters.Page);
            Assert.Equal(25, parameters.PageSize);
            Assert.Equal("Timestamp", parameters.SortBy);
            Assert.Equal("desc", parameters.SortDirection);
            Assert.Null(parameters.Level);
            Assert.Null(parameters.StartDate);
            Assert.Null(parameters.EndDate);
            Assert.Null(parameters.Source);
            Assert.Null(parameters.Application);
            Assert.Null(parameters.Search);
            Assert.Null(parameters.CorrelationId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(25)]
        [InlineData(50)]
        [InlineData(100)]
        public void PageSize_WithBoundaryValues_WorksCorrectly(int pageSize)
        {
            // Arrange
            var parameters = new LogQueryParameters();

            // Act
            parameters.PageSize = pageSize;

            // Assert
            Assert.Equal(pageSize, parameters.PageSize);
        }
    }
}
