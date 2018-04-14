using EyeInTheSky.Exceptions;

namespace EyeInTheSky.Services.Interfaces
{
    public interface IBugReporter
    {
        void ReportBug(BugException ex);
    }
}