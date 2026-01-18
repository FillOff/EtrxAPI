namespace Etrx.Application.Dtos.Common;

public record ErrorResponseDto<TError>
{
    public int StatusCode { get; set; }
    public TError Errors { get; set; }

    public ErrorResponseDto(TError errors, int statusCode)
    {
        Errors = errors;
        StatusCode = statusCode;
    }
}