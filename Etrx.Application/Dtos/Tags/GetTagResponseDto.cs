using System;

namespace Etrx.Application.Dtos.Tags;

public class GetTagResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Complexity { get; set; }
}