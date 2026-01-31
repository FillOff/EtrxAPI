namespace Etrx.Application.Constants;

public static class Verdicts
{
    public const string Failed = "FAILED";
    public const string Ok = "OK";
    public const string Partial = "PARTIAL";
    public const string CompilationError = "COMPILATION_ERROR";
    public const string RuntimeError = "RUNTIME_ERROR";
    public const string WrongAnswer = "WRONG_ANSWER";
    public const string TimeLimitExceeded = "TIME_LIMIT_EXCEEDED";
    public const string MemoryLimitExceeded = "MEMORY_LIMIT_EXCEEDED";
    public const string IdlenessLimitExceeded = "IDLENESS_LIMIT_EXCEEDED";
    public const string SecurityViolated = "SECURITY_VIOLATED";
    public const string Crashed = "CRASHED";
    public const string InputPreparationCrashed = "INPUT_PREPARATION_CRASHED";
    public const string Challenged = "CHALLENGED";
    public const string Skipped = "SKIPPED";
    public const string Testing = "TESTING";
    public const string Rejected = "REJECTED";
    public const string Submitted = "SUBMITTED";

    private static readonly IReadOnlyList<string> _all =
    [
        Failed, Ok, Partial, CompilationError, RuntimeError, WrongAnswer,
        TimeLimitExceeded, MemoryLimitExceeded, IdlenessLimitExceeded,
        SecurityViolated, Crashed, InputPreparationCrashed, Challenged,
        Skipped, Testing, Rejected, Submitted
    ];

    public static IReadOnlyList<string> GetAll() => _all;
}