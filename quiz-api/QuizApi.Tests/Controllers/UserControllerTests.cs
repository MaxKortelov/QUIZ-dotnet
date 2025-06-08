using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using quiz_api.Controllers;
using quiz_api.Service;

namespace QuizApi.Tests.Controllers
{
    public class UserControllerTests
    {
        [Fact]
        public async Task LoadUser_ReturnsOk_WithUserDataDto()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var sampleEmail = "test@example.com";

            var sampleUserData = new UserDataDto(
                new quiz_api.Models.User
                {
                    Uuid = Guid.NewGuid(),
                    Email = sampleEmail,
                    Username = "TestUser",
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow
                },
                new quiz_api.Models.QuizTableResult
                {
                    QuizAmountTaken = 5,
                    CorrectAnswers = 20
                },
                "00:30"
            );

            mockUserService.Setup(service => service.LoadUser(sampleEmail))
                .ReturnsAsync(sampleUserData);

            var controller = new UserController(mockUserService.Object);

            var loadUserDto = new LoadUserDto
            {
                Email = sampleEmail
            };

            // Act
            var result = await controller.LoadUser(loadUserDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUserData = Assert.IsType<UserDataDto>(okResult.Value);

            Assert.Equal(sampleEmail, returnedUserData.Email);
            Assert.Equal("TestUser", returnedUserData.Username);
        }
    }
}