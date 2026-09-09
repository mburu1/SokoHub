using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Common.Errors;

public record ValidationError(string Field, string Description) : ApplicationError("validation_error", $"Validation failed for {Field}: {Description}");
