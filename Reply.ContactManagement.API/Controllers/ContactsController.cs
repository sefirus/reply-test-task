using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reply.ContactManagement.API.Extensions;
using Reply.ContactManagement.Application.Interfaces;
using Reply.ContactManagement.Core.Contracts.Common;
using Reply.ContactManagement.Core.Contracts.Contacts;

namespace Reply.ContactManagement.API.Controllers;

[ApiController]
[Route("api/contacts")]
public sealed class ContactsController(
    IContactService contactService,
    IContactBulkMergeService contactBulkMergeService,
    IValidator<CreateContactRequest> createValidator,
    IValidator<UpdateContactRequest> updateValidator,
    IValidator<GetContactsRequest> getValidator,
    IValidator<BulkMergeContactsRequest> bulkMergeValidator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CreatedEntityResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatedEntityResponse>> CreateAsync(
        [FromBody] CreateContactRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToValidationProblemDetails());
        }

        var contact = await contactService.CreateAsync(request, cancellationToken);

        return Created($"/api/contacts/{contact.Id}", contact);
    }

    [HttpGet]
    [ProducesResponseType<PagedResponse<ContactResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<ContactResponse>>> GetAsync(
        [FromQuery] GetContactsRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await getValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToValidationProblemDetails());
        }

        var contacts = await contactService.GetAsync(request, cancellationToken);

        return Ok(contacts);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ContactResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var contact = await contactService.GetByIdAsync(id, cancellationToken);

        return Ok(contact);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<ContactResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactResponse>> UpdateAsync(
        Guid id,
        [FromBody] UpdateContactRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToValidationProblemDetails());
        }

        var contact = await contactService.UpdateAsync(id, request, cancellationToken);

        return Ok(contact);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await contactService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }

    [HttpPut("bulk-merge")]
    [ProducesResponseType<BulkMergeContactsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BulkMergeContactsResponse>> BulkMergeAsync(
        [FromBody] BulkMergeContactsRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await bulkMergeValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToValidationProblemDetails());
        }

        var result = await contactBulkMergeService.MergeAsync(request, cancellationToken);

        return Ok(result);
    }
}
