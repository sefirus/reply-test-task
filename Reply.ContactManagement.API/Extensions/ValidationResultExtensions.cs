using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Reply.ContactManagement.API.Extensions;

internal static class ValidationResultExtensions
{
    public static ValidationProblemDetails ToValidationProblemDetails(this ValidationResult validationResult)
    {
        return new ValidationProblemDetails(validationResult.ToDictionary());
    }

    private static Dictionary<string, string[]> ToDictionary(this ValidationResult validationResult)
    {
        return validationResult.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).Distinct().ToArray());
    }
}
