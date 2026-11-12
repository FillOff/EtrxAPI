namespace Etrx.Application.Interfaces;

public interface IHashService
{
    bool Matches(string expected, string? actual);
}
