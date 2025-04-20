using Etrx.Application.Interfaces;

namespace Etrx.Application.Services;

public class LastUpdateTimeService : ILastUpdateTimeService
{
    private DateTime _contestsLastTimeUpdate;
    private DateTime _problemsLastTimeUpdate;
    private DateTime _usersLastTimeUpdate;

    public DateTime GetLastUpdateTime(string tableName)
    {
        switch (tableName.ToLower())
        {
            case "contests":
                return _contestsLastTimeUpdate;
            case "problems":
                return _problemsLastTimeUpdate;
            case "users":
                return _usersLastTimeUpdate;
            default:
                throw new Exception($"There are no tables named {tableName}");
        }
    }

    public void UpdateLastUpdateTime(string tableName, DateTime currentTime)
    {
        switch (tableName.ToLower())
        {
            case "contests":
                _contestsLastTimeUpdate = currentTime;
                break;
            case "problems":
                _problemsLastTimeUpdate = currentTime;
                break;
            case "users":
                _usersLastTimeUpdate = currentTime;
                break;
            default:
                throw new Exception($"There are no tables named {tableName}");
        }
    }
}
