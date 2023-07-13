using FlashcardsApp.Controllers;
using FlashcardsApp.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Data;
using Xunit;

namespace FlashcardsApp.Test.Controllers
{
    public class DeckControllerTests
    {
        private readonly DeckController _sut;
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();

        public DeckControllerTests()
        {
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "SQLServer")]).Returns("mock value");

            _configurationMock.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);
            _sut = new DeckController(_configurationMock.Object);
        }

        [Fact]
        public async void DeckController_CreateDeck_ReturnsOk()
        {
            //// Arrange
            //NewDeckDto newDeckDto = new NewDeckDto{
            //    UserId = 2,
            //    Title= "Testerr",
            //    Description= "Testrerrraas"
            //};

            //// Act
            //var response = await _sut.CreateDeck(newDeckDto);

            //// Assert
            //response.Should().BeOfType(typeof(OkObjectResult));
        }
    }
}
