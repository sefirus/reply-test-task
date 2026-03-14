using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reply.ContactManagement.API.Extensions;
using Reply.ContactManagement.Application.Interfaces;
using Reply.ContactManagement.Core.Contracts.Common;
using Reply.ContactManagement.Core.Contracts.CustomFields;

namespace Reply.ContactManagement.API.Controllers;

[ApiController]
[Route("api/custom-fields")]
public sealed class CustomFieldsController(
    ICustomFieldService customFieldService,
    IValidator<CreateCustomFieldRequest> createValidator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<CustomFieldResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CustomFieldResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var customFields = await customFieldService.GetAllAsync(cancellationToken);

        return Ok(customFields);
    }

    [HttpPost]
    [ProducesResponseType<CreatedEntityResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreatedEntityResponse>> CreateAsync(
        [FromBody] CreateCustomFieldRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToValidationProblemDetails());
        }

        var customField = await customFieldService.CreateAsync(request, cancellationToken);
        return Created($"/api/custom-fields/{customField.Id}", customField);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await customFieldService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
