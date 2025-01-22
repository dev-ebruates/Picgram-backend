namespace Application.Features.Reports
{
    public class GetAllApplicationReportCommand : IRequest<Response<GetAllApplicationReportCommandResponse>>
    {
    }

    public class GetAllApplicationReportCommandHandler : IRequestHandler<GetAllApplicationReportCommand, Response<GetAllApplicationReportCommandResponse>>
    {
        private readonly UnitOfWork unitOfWork;
        private readonly TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

        public GetAllApplicationReportCommandHandler(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Response<GetAllApplicationReportCommandResponse>> Handle(GetAllApplicationReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Türkiye saatine göre zaman
                var currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

                // Haftanın başlangıç günü (pazartesi)
                var startOfWeek = currentDate.Date.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday); 

                // Kullanıcıları al
                var users = await unitOfWork.UserRepository.GetCreatedAtList();
                var totalUserCount = users?.Count ?? 0;
                var dailyUserCount = users?.Count(u => u.Date == currentDate.Date) ?? 0;
                var monthlyUserCount = users?.Count(u =>
                    u.Month == currentDate.Month && 
                    u.Year == currentDate.Year) ?? 0;

                // Gönderileri al
                var posts = await unitOfWork.PostRepository.GetCreatedAtList();
                var totalPostCount = posts?.Count ?? 0;
                var dailyPostCount = posts?.Count(p => p.Date == currentDate.Date) ?? 0;
                var monthlyPostCount = posts?.Count(p =>
                    p.Month == currentDate.Month &&
                    p.Year == currentDate.Year) ?? 0;

                // Haftalık kullanıcı ve gönderi verilerini hesapla
                var weeklyUserData = new List<int>();
                var weeklyPostData = new List<int>();

                for (int i = 0; i < 7; i++)
                {
                    var date = startOfWeek.AddDays(i); // Haftanın günlerini al (pazartesiden başlar)
                    
                    var dailyUser = users?.Count(u => u.Date == date.Date) ?? 0;
                    var dailyPost = posts?.Count(p => p.Date == date.Date) ?? 0;

                    weeklyUserData.Add(dailyUser);
                    weeklyPostData.Add(dailyPost);
                }

                // Yanıt oluştur
                var response = new GetAllApplicationReportCommandResponse(
                    totalUserCount,
                    dailyUserCount,
                    monthlyUserCount,
                    totalPostCount,
                    dailyPostCount,
                    monthlyPostCount,
                    weeklyUserData,
                    weeklyPostData);

                return Response<GetAllApplicationReportCommandResponse>.CreateSuccessResponse(response);
            }
            catch (Exception ex)
            {
                // Hata durumunu detaylı olarak döndür
                return Response<GetAllApplicationReportCommandResponse>.CreateErrorResponse($"An error occurred: {ex.Message}");
            }
        }
    }

    public class GetAllApplicationReportCommandResponse
    {
        // Kullanıcı istatistikleri
        public int TotalUserCount { get; set; }
        public int DailyUserCount { get; set; }
        public int MonthlyUserCount { get; set; }

        // Gönderi istatistikleri
        public int TotalPostCount { get; set; }
        public int DailyPostCount { get; set; }
        public int MonthlyPostCount { get; set; }

        // Haftalık kullanıcı ve gönderi verileri
        public List<int> WeeklyUserData { get; set; }
        public List<int> WeeklyPostData { get; set; }

        public GetAllApplicationReportCommandResponse(
            int totalUserCount,
            int dailyUserCount,
            int monthlyUserCount,
            int totalPostCount,
            int dailyPostCount,
            int monthlyPostCount,
            List<int> weeklyUserData,
            List<int> weeklyPostData)
        {
            TotalUserCount = totalUserCount;
            DailyUserCount = dailyUserCount;
            MonthlyUserCount = monthlyUserCount;
            TotalPostCount = totalPostCount;
            DailyPostCount = dailyPostCount;
            MonthlyPostCount = monthlyPostCount;
            WeeklyUserData = weeklyUserData;
            WeeklyPostData = weeklyPostData;
        }
    }
}
