using System.Text.Json;
using System.Text.Json.Serialization;

namespace CleanArch.UseCase;

internal static class Serialization
{
    internal static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.Preserve,
        Converters = { new JsonStringEnumConverter() }
    };
}