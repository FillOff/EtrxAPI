using System.Security.Cryptography;
using System.Text;
using Etrx.Application.Interfaces;

namespace Etrx.Application.Services;

public class HashService : IHashService
{
    public bool Matches(string expected, string? actual) => CryptographicOperations.FixedTimeEquals(
        SHA256.HashData(Encoding.UTF8.GetBytes(expected)),
        SHA256.HashData(Encoding.UTF8.GetBytes(actual ?? string.Empty)));
}
