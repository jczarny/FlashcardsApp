using FlashcardsApp.Controllers;
using FlashcardsApp.Dtos;
using FlashcardsApp.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace FlashcardsApp.Test.Controllers
{
    public class LearnControllerTests
    {
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly ControllerContext _controllerContext;
        private readonly int _userId = 3;

        public LearnControllerTests()
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
        public async void Evaluate_ValidLearnDto_ReturnsOk()
        {
            // Arrange
            var learnDto = new LearnDto{
                CardId = 15,
                DeckId = 1,
                Response = 3,
            };

            var mockLearnModel = new Mock<ILearnModel>();
            mockLearnModel.Setup(a => a.EvaluateResult(_userId, learnDto.CardId, learnDto.DeckId, learnDto.Response)).ReturnsAsync(true);

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._learnModel).Returns(mockLearnModel.Object);

            var _sut = new LearnController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.Evaluate(learnDto);

            // Assert
            response.Should().BeOfType(typeof(OkResult));
        }

        [Fact]
        public async void Evaluate_DbFail_ReturnsBadRequest()
        {
            // Arrange
            var learnDto = new LearnDto
            {
                CardId = 15,
                DeckId = 1,
                Response = 3,
            };

            var mockLearnModel = new Mock<ILearnModel>();
            mockLearnModel.Setup(a => a.EvaluateResult(_userId, learnDto.CardId, learnDto.DeckId, learnDto.Response)).ReturnsAsync(false);

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._learnModel).Returns(mockLearnModel.Object);

            var _sut = new LearnController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.Evaluate(learnDto);

            // Assert
            response.Should().BeOfType(typeof(BadRequestResult));
        }

        [Fact]
        public async void Evaluate_CorruptedUserId_ReturnsBadRequest()
        {
            // Arrange
            var learnDto = new LearnDto
            {
                CardId = 15,
                DeckId = 1,
                Response = 3,
            };

            var mockLearnModel = new Mock<ILearnModel>();
            mockLearnModel.Setup(a => a.EvaluateResult(_userId, learnDto.CardId, learnDto.DeckId, learnDto.Response)).ReturnsAsync(true);

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._learnModel).Returns(mockLearnModel.Object);

            var httpContext = new DefaultHttpContext();
            var cookie = new[] { $"userId=CorruptedUserId" };
            httpContext.Request.Headers["Cookie"] = cookie;
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            var _sut = new LearnController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = controllerContext;

            // Act
            var response = await _sut.Evaluate(learnDto);

            // Assert
            response.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public async void GetLearningCards_FoundCardsToLearn_ReturnsOkWithCardsInJson()
        {
            // Arrange
            int deckId = 3;
            int amount = 5;
            CardDto card = new CardDto{
                DeckId = 5,
                Front = "Test",
                Reverse = "Test",
                Description= "Test"
            };
            List<CardDto> cards = new List<CardDto>
            { card, card, card, card, card };

            var mockLearnModel = new Mock<ILearnModel>();
            mockLearnModel.Setup(a => a.GetLearningCards(_userId, deckId, amount)).ReturnsAsync(cards);

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._learnModel).Returns(mockLearnModel.Object);

            var _sut = new LearnController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.GetLearningCards(deckId, amount);

            // Assert
            dynamic model = response.Result!;
            string actual = (string)model.Value;
            string expected = JsonSerializer.Serialize(cards);

            response.Result.Should().BeOfType(typeof(OkObjectResult));
            actual.Should().NotBeNullOrEmpty();
            actual.Should().Be(expected);
        }

        [Fact]
        public async void GetLearningCards_NoCardsFound_ReturnsEmptyOk()
        {
            // Arrange
            int deckId = 3;
            int amount = 5;
            List<CardDto> cards = new List<CardDto>();

            var mockLearnModel = new Mock<ILearnModel>();
            mockLearnModel.Setup(a => a.GetLearningCards(_userId, deckId, amount)).ReturnsAsync(cards);

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._learnModel).Returns(mockLearnModel.Object);

            var _sut = new LearnController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.GetLearningCards(deckId, amount);

            // Assert
            dynamic model = response.Result!;
            string actual = (string)model.Value;

            response.Result.Should().BeOfType(typeof(OkObjectResult));
            actual.Should().BeNullOrEmpty();

        }

        [Fact]
        public async void GetLearningCards_DbError_ReturnsBadRequest()
        {
            // Arrange
            int deckId = 3;
            int amount = 5;
            List<CardDto> cards = new List<CardDto>();

            var mockLearnModel = new Mock<ILearnModel>();
            mockLearnModel.Setup(a => a.GetLearningCards(_userId, deckId, amount)).Throws(new Exception());

            var mockRepo = new Mock<IFlashcardsRepository>();
            mockRepo.SetupGet(a => a._learnModel).Returns(mockLearnModel.Object);

            var _sut = new LearnController(_configurationMock.Object, mockRepo.Object);
            _sut.ControllerContext = _controllerContext;

            // Act
            var response = await _sut.GetLearningCards(deckId, amount);

            // Assert
            response.Result.Should().BeOfType(typeof(BadRequestObjectResult));

        }
    }
}
