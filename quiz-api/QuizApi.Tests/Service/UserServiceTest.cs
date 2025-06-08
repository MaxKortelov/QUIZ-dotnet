using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using quiz_api.Service;
using quiz_api.Models;
// using quiz_api.Dto;

namespace QuizApi.Tests.Service
{
    public class UserServiceTests
    {
        [Fact]
        public async Task LoadUser_ReturnsCorrectUserDataDto()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            var mockQuizRepository = new Mock<IQuizRepository>();
            var mockDateService = new Mock<IDateService>();

            var sampleUser = new User
            {
                Uuid = Guid.NewGuid(),
                Email = "test@example.com",
                Username = "TestUser",
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            var sampleQuizSession = new QuizSession
            {
                DateStarted = DateTime.UtcNow.AddMinutes(-10),
                DateEnded = DateTime.UtcNow
            };

            var sampleQuizTableResult = new QuizTableResult
            {
                QuizAmountTaken = 3,
                CorrectAnswers = 15,
                BestQuizSession = sampleQuizSession
            };

            var expectedFastestTestTime = "00:10";

            mockUserRepository.Setup(repo => repo.FindByEmailAsync(sampleUser.Email))
                .ReturnsAsync(sampleUser);

            mockQuizRepository.Setup(repo => repo.GetUserQuizTableResultAsync(sampleUser.Uuid))
                .ReturnsAsync(sampleQuizTableResult);

            mockDateService.Setup(service =>
                    service.DateDifferenceFormatted(sampleQuizSession.DateStarted, sampleQuizSession.DateEnded))
                .Returns(expectedFastestTestTime);

            var userService = new UserService(
                mockUserRepository.Object,
                mockQuizRepository.Object,
                mockDateService.Object
            );

            // Act
            var result = await userService.LoadUser(sampleUser.Email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(sampleUser.Uuid, result.Uuid);
            Assert.Equal(sampleUser.Email, result.Email);
            Assert.Equal(sampleUser.Username, result.Username);
            Assert.Equal(sampleQuizTableResult.QuizAmountTaken, result.QuizAmountTaken);
            Assert.Equal(expectedFastestTestTime, result.FastestTestTime);
            Assert.Equal(sampleQuizTableResult.CorrectAnswers, result.CorrectAnswers);
        }
    }
}
