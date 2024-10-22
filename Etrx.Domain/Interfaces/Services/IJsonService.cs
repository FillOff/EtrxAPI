using Etrx.Domain.Models;
using System.Text.Json;

namespace Etrx.Domain.Interfaces.Services
{
    public interface IJsonService
    {
        Submission JsonToSubmission(JsonElement jsonSubmission);
    }
}