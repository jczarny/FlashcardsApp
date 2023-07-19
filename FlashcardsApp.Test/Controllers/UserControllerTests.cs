using FlashcardsApp.Controllers;
using FlashcardsApp.Dtos;
using FlashcardsApp.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlashcardsApp.Test.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly ControllerContext _controllerContext;
        private readonly int _userId = 3;

        public UserControllerTests()
        {
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "SQLServer")]).Returns("mock value");
            _configurationMock.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);

            var httpContext = new DefaultHttpContext();
            var cookie = new[] { $"userId={_userId}" };
            httpContext.Request.Headers["Cookie"] = cookie;
            _controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async void GetOwnedDecks_NoDecksOwned_ReturnsEmptyOk()
        {
            // Arrange
            var deckList = new List<DeckDto>();
            var dict = new Dictionary<int, int>();

            var mockUserModel = new Mock<IUserModel>();
            mockUserModel.Setup(a => a.GetUsersDeckInfo(_userId)).ReturnsAsync(deckList);

            var mockLearnModel = new Mock<ILearnModel>();
            mockLearnModel.Setup(a => a.GetReviseCardAmount(_userId)).ReturnsAsync(dict);

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._learnModel).Returns(mockLearnModel.Object);
            mockRepo.SetupGet(a => a._userModel).Returns(mockUserModel.Object);

            var _sut = new UserController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.GetOwnedDecks();

            // Assert
            var res = response.Result as OkObjectResult;
            string actual = (string)res.Value;

            res.Should().BeOfType(typeof(OkObjectResult));
            actual.Should().Be("[]");
        }

        [Fact]
        public async void GetOwnedDecks_FewDecksOwned_ReturnsOkWithDeckDto()
        {
            // Arrange
            var CARDSTOREVISE = 87;
            var deck = new DeckDto
            {
                Id = 5,
                IsOwner = true,
                CreatorName = "Test",
                Title = "Test",
                Description = "Test",
                isPrivate = true
            };
            var deckList = new List<DeckDto> { deck };
            var dict = new Dictionary<int, int> { { 5, CARDSTOREVISE } };

            var mockUserModel = new Mock<IUserModel>();
            mockUserModel.Setup(a => a.GetUsersDeckInfo(_userId)).ReturnsAsync(deckList);

            var mockLearnModel = new Mock<ILearnModel>();
            mockLearnModel.Setup(a => a.GetReviseCardAmount(_userId)).ReturnsAsync(dict);

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._learnModel).Returns(mockLearnModel.Object);
            mockRepo.SetupGet(a => a._userModel).Returns(mockUserModel.Object);

            var _sut = new UserController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.GetOwnedDecks();

            // Assert
            var properDeckOutput = deck;
            properDeckOutput.CardsToRevise = CARDSTOREVISE;
            var properDeckList = new List<DeckDto> { properDeckOutput };

            var res = response.Result as OkObjectResult;
            string actual = (string)res.Value;
            string expected = JsonSerializer.Serialize(properDeckList);

            res.Should().BeOfType(typeof(OkObjectResult));
            actual.Should().Be(expected);
        }

        [Fact]
        public async void GetOwnedDecks_DbError_ReturnsBadRequest()
        {
            // Arrange
            var deckList = new List<DeckDto>();
            var dict = new Dictionary<int, int>();

            var mockUserModel = new Mock<IUserModel>();
            mockUserModel.Setup(a => a.GetUsersDeckInfo(_userId)).Throws(new Exception());

            var mockLearnModel = new Mock<ILearnModel>();
            mockLearnModel.Setup(a => a.GetReviseCardAmount(_userId)).ReturnsAsync(dict);

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._learnModel).Returns(mockLearnModel.Object);
            mockRepo.SetupGet(a => a._userModel).Returns(mockUserModel.Object);

            var _sut = new UserController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.GetOwnedDecks();

            // Assert
            var res = response.Result as BadRequestObjectResult;
            res.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async void Acquire_ValidDeck_ReturnsOk()
        {
            // Arrange
            int deckId = 5;

            var mockUserModel = new Mock<IUserModel>();

            mockUserModel.Setup(a => a.AcquirePublicDeck(_userId, deckId)).ReturnsAsync(Results.Ok());

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._userModel).Returns(mockUserModel.Object);

            var _sut = new UserController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.Acquire(deckId.ToString());

            // Assert
            response.Should().BeOfType(typeof(OkResult));
        }

        [Fact]
        public async void Acquire_InvalidDeckId_ReturnsBadRequest()
        {
            // Arrange
            string deckId = "214d2";

            var mockUserModel = new Mock<IUserModel>();

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._userModel).Returns(mockUserModel.Object);

            var _sut = new UserController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.Acquire(deckId);

            // Assert
            response.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async void Acquire_DbError_ReturnsBadRequest()
        {
            // Arrange
            int deckId = 5;

            var mockUserModel = new Mock<IUserModel>();
            mockUserModel.Setup(a => a.AcquirePublicDeck(_userId, deckId)).Throws(new Exception());

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._userModel).Returns(mockUserModel.Object);

            var _sut = new UserController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.Acquire(deckId.ToString());

            // Assert
            response.Should().BeOfType(typeof(BadRequestObjectResult));
        }
    }
}
