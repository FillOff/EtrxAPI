namespace Etrx.Application.Constants;

public static class RequestLimits
{
    public const int MaxHandleLength = 100;
    public const int MinPasswordLength = 10;
    public const int MaxPasswordLength = 128;
    public const int MaxActivationCodeLength = 512;
    public const int MinActivationCodeLength = 32;
    public const int MaxProblemIndexLength = 20;
    public const int MaxFilterValueLength = 4096;
    public const int PreferencesBodyBytes = 32 * 1024;
    public const int MaxFailedLoginAttempts = 5;
    public const int LoginLockoutMinutes = 15;
}
