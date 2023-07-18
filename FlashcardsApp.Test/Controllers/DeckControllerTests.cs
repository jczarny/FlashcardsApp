using FlashcardsApp.Controllers;
using FlashcardsApp.Dtos;
using FlashcardsApp.Interfaces;
using FlashcardsApp.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Collections.Generic;
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
        }

        [Fact]
        public async void CreateDeck_ValidDeck_ReturnsOk()
        {
            // Arrange
            NewDeckDto newDeckDto = new NewDeckDto
            {
                Title = "Test deck valid title",
                Description = "Test deck valid description"
            };

            var mockDeckModel = new Mock<IDeckModel>();
            mockDeckModel.Setup(a => a.CreateDeck(newDeckDto, 3)).ReturnsAsync(true);

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._deckModel).Returns(mockDeckModel.Object);

            var httpContext = new DefaultHttpContext();
            var cookie = new[] { "userId=3" };
            httpContext.Request.Headers["Cookie"] = cookie;
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            var _sut = new DeckController(_configurationMock.Object, mockRepo.Object)
            {
                ControllerContext = controllerContext
            };


            // Act
            var response = await _sut.CreateDeck(newDeckDto);

            // Assert
            response.Should().BeOfType(typeof(OkResult));
        }

        [Fact]
        public async void CreateDeck_CorruptedUserId_ReturnBadRequest()
        {
            // Arrange
            NewDeckDto newDeckDto = new NewDeckDto
            {
                Title = "Test deck valid title",
                Description = "Test deck valid description"
            };

            var mockDeckModel = new Mock<IDeckModel>();
            mockDeckModel.Setup(a => a.CreateDeck(newDeckDto, 3)).ReturnsAsync(true);

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._deckModel).Returns(mockDeckModel.Object);

            var httpContext = new DefaultHttpContext();
            var cookie = new[] { "userId=CorruptedIdExample" };
            httpContext.Request.Headers["Cookie"] = cookie;
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            var _sut = new DeckController(_configurationMock.Object, mockRepo.Object)
            {
                ControllerContext = controllerContext
            };

            // Act
            var response = await _sut.CreateDeck(newDeckDto);

            // Assert
            response.Should().BeOfType(typeof(BadRequestObjectResult));
        }
    }
}
