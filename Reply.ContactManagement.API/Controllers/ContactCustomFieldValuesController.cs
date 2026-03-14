using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reply.ContactManagement.API.Extensions;
using Reply.ContactManagement.Application.Interfaces;
using Reply.ContactManagement.Core.Contracts.Contacts;
using Reply.ContactManagement.Core.Contracts.CustomFields;

namespace Reply.ContactManagement.API.Controllers;

[ApiController]
[Route("api/contacts/{contactId:guid}/custom-fields")]
public sealed class ContactCustomFieldValuesController(
    ICustomFieldService customFieldService,
    IValidator<AssignCustomFieldValueRequest> assignValidator) : ControllerBase
{
    [HttpPut("{customFieldId:guid}/value")]
    [ProducesResponseType<ContactCustomFieldValueResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactCustomFieldValueResponse>> AssignValueAsync(
        Guid contactId,
        Guid customFieldId,
        [FromBody] AssignCustomFieldValueRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await assignValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToValidationProblemDetails());
        }

        var result = await customFieldService.AssignValueAsync(contactId, customFieldId, request, cancellationToken);

        return Ok(result);
    }
}
