using Etrx.Application.Exceptions.BadRequest;

namespace Etrx.Application.Services;

public static class ProtocolDateRange
{
    // Protocol calendar days use Belarus/Moscow time (UTC+3), regardless of server TZ.
    public static (long From, long Until) ToUnix(
        int fYear, int fMonth, int fDay, int tYear, int tMonth, int tDay)
    {
        try
        {
            var offset = TimeSpan.FromHours(3);
            var from = new DateTimeOffset(fYear, fMonth, fDay, 0, 0, 0, offset);
            var to = new DateTimeOffset(tYear, tMonth, tDay, 0, 0, 0, offset);
            if (from > to)
                throw new BadRequestException("Начальная дата должна быть не позже конечной.");
            return (from.ToUnixTimeSeconds(), to.AddDays(1).ToUnixTimeSeconds());
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new BadRequestException("Указана некорректная дата.");
        }
    }
}
