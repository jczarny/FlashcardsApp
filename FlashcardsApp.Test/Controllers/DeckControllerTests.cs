using FlashcardsApp.Controllers;
using FlashcardsApp.Dtos;
using FlashcardsApp.Entities;
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
using System.Data.Common;
using Xunit;

namespace FlashcardsApp.Test.Controllers
{
    public class DeckControllerTests
    {
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly ControllerContext _controllerContext;
        private readonly int _userId = 3;

        public DeckControllerTests()
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
        public async void CreateDeck_ValidNewDeck_ReturnsOk()
        {
            // Arrange
            NewDeckDto newDeckDto = new NewDeckDto
            {
                Title = "Test deck valid title",
                Description = "Test deck valid description"
            };
            int userId = 3;
            var mockDeckModel = new Mock<IDeckModel>();
            mockDeckModel.Setup(a => a.CreateDeck(newDeckDto, userId)).ReturnsAsync(true);

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._deckModel).Returns(mockDeckModel.Object);

            var _sut = new DeckController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

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
            mockDeckModel.Setup(a => a.CreateDeck(newDeckDto, _userId)).ReturnsAsync(true);

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

        [Fact]
        public async void GetDeck_DeckNotOwned_ReturnBadRequest()
        {
            // Arrange
            string deckId = "3";

            var mockDeckModel = new Mock<IDeckModel>();
            mockDeckModel.Setup(a => a.GetDeckInfo(Int32.Parse(deckId), _userId)).Throws(new ArgumentException());

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._deckModel).Returns(mockDeckModel.Object);

            var httpContext = new DefaultHttpContext();
            var cookie = new[] { $"userId=CorruptedId" };
            httpContext.Request.Headers["Cookie"] = cookie;
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            var _sut = new DeckController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = controllerContext;

            // Act
            var response = await _sut.GetDeck(deckId);

            // Assert
            response.Result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async void GetDeck_DeckOwned_ReturnOk()
        {
            // Arrange
            string deckId = "3";
            var deck = new DeckDto
            {
                Id = 3,
                IsOwner = true,
                CreatorName = "Test",
                Title = "Test",
                Description = "Test",
                isPrivate = true,
            };
            var mockDeckModel = new Mock<IDeckModel>();
            mockDeckModel.Setup(a => a.GetDeckInfo(int.Parse(deckId), _userId)).ReturnsAsync(deck);
            mockDeckModel.Setup(a => a.GetDeckCards(_userId)).ReturnsAsync(new List<CardDto>());

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._deckModel).Returns(mockDeckModel.Object);

            var _sut = new DeckController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.GetDeck(deckId);

            // Assert
            response.Result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void AddCard_CardValid_ReturnOk()
        {
            // Arrange
            string cardId = "73"; // Assigned on return value of adding card to deck
            var cardDto = new CardDto
            {
                DeckId = 3,
                Front = "Test",
                Reverse = "Test",
                Description = "Test"
            };

            var mockDeckModel = new Mock<IDeckModel>();
            mockDeckModel.Setup(a => a.AddCardToDeck(cardDto, _userId)).ReturnsAsync(cardId);

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._deckModel).Returns(mockDeckModel.Object);

            var _sut = new DeckController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.AddCard(cardDto);

            // Assert
            response.Result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public async void AddCard_DbDeckNotOwned_ReturnBadRequest()
        {
            // Arrange
            string cardId = "-1"; // Assigned on return value of adding card to deck
            var cardDto = new CardDto
            {
                DeckId = 3,
                Front = "Test",
                Reverse = "Test",
                Description = "Test"
            };

            var mockDeckModel = new Mock<IDeckModel>();
            mockDeckModel.Setup(a => a.AddCardToDeck(cardDto, _userId)).ReturnsAsync(cardId);

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._deckModel).Returns(mockDeckModel.Object);

            var _sut = new DeckController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.AddCard(cardDto);

            // Assert
            response.Result.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async void DeleteCard_DbDeleteFailed_ReturnBadRequest()
        {
            // Arrange
            string cardId = "111";

            var mockDeckModel = new Mock<IDeckModel>();
            mockDeckModel.Setup(a => a.DeleteCardFromDeck(int.Parse(cardId), _userId)).ReturnsAsync(false);

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._deckModel).Returns(mockDeckModel.Object);

            var _sut = new DeckController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.DeleteCard(cardId);

            // Assert
            response.Should().BeOfType(typeof(BadRequestResult));
        }

        [Fact]
        public async void DeleteCard_ValidCard_ReturnOk()
        {
            // Arrange
            string cardId = "111";

            var mockDeckModel = new Mock<IDeckModel>();
            mockDeckModel.Setup(a => a.DeleteCardFromDeck(int.Parse(cardId), _userId)).ReturnsAsync(true);

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._deckModel).Returns(mockDeckModel.Object);

            var _sut = new DeckController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.DeleteCard(cardId);

            // Assert
            response.Should().BeOfType(typeof(OkResult));
        }

        [Fact]
        public async void DeleteDeck_ValidDeck_ReturnOk()
        {
            // Arrange
            string deckId = "111";

            var mockDeckModel = new Mock<IDeckModel>();
            mockDeckModel.Setup(a => a.DeleteDeck(_userId, int.Parse(deckId))).ReturnsAsync(true);

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._deckModel).Returns(mockDeckModel.Object);

            var _sut = new DeckController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.DeleteDeck(deckId);

            // Assert
            response.Should().BeOfType(typeof(OkResult));
        }

        [Fact]
        public async void DeleteDeck_DbDeleteFailed_ReturnBadRequest()
        {
            // Arrange
            string deckId = "111";

            var mockDeckModel = new Mock<IDeckModel>();
            mockDeckModel.Setup(a => a.DeleteDeck(_userId, int.Parse(deckId))).Throws(new Exception());

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._deckModel).Returns(mockDeckModel.Object);

            var _sut = new DeckController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.DeleteDeck(deckId);

            // Assert
            response.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async void PublishDeck_DeckOwned_ReturnOk()
        {
            // Arrange
            string deckId = "111";

            var mockDeckModel = new Mock<IDeckModel>();
            mockDeckModel.Setup(a => a.PublishDeck(int.Parse(deckId), _userId)).ReturnsAsync(true);

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._deckModel).Returns(mockDeckModel.Object);

            var _sut = new DeckController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.PublishDeck(deckId);

            // Assert
            response.Should().BeOfType(typeof(OkResult));
        }

        [Fact]
        public async void PublishDeck_DbError_ReturnBadRequest()
        {
            // Arrange
            string deckId = "111";

            var mockDeckModel = new Mock<IDeckModel>();
            mockDeckModel.Setup(a => a.PublishDeck(int.Parse(deckId), _userId)).Throws(new Exception());

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._deckModel).Returns(mockDeckModel.Object);

            var _sut = new DeckController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.PublishDeck(deckId);

            // Assert
            response.Should().BeOfType(typeof(BadRequestObjectResult));
        }
    }
}
