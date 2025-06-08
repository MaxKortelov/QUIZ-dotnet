using System.Text.Json;

namespace quiz_api.Service
{
    public interface IUserService
    {
        Task<UserDataDto> LoadUser(string email);
    }

    public class UserService(IUserRepository userRepository, IQuizRepository quizRepository, IDateService dateService)
        : IUserService
    {
        public async Task<UserDataDto> LoadUser(string email)
        {
            var user = await userRepository.FindByEmailAsync(email);
            var quizTableResult = await quizRepository.GetUserQuizTableResultAsync(user.Uuid);
            var fastestTestTime = dateService.DateDifferenceFormatted(quizTableResult.BestQuizSession?.DateStarted,
                quizTableResult.BestQuizSession?.DateEnded);

            return new UserDataDto(user, quizTableResult, fastestTestTime);
        }
    }
}