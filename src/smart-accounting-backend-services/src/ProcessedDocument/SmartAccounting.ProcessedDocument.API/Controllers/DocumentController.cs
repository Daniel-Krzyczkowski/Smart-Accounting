using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartAccounting.ProcessedDocument.API.Application.DTO;
using SmartAccounting.ProcessedDocument.API.Application.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartAccounting.ProcessedDocument.API.Controllers
{
    [Authorize(Policy = "AccessAsUser")]
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly ILogger<DocumentController> _logger;
        private readonly IMediator _mediator;

        public DocumentController(ILogger<DocumentController> logger,
                                  IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Gets all scanned invoices for the user
        /// </summary>
        /// <returns>
        /// List of all scanned invoices for the user
        /// </returns> 
        /// <response code="200">List with scanned invoices</response>
        /// <response code="401">Access denied</response>
        /// <response code="404">Cars list not found</response>
        /// <response code="500">Oops! something went wrong</response>
        [ProducesResponseType(typeof(IEnumerable<UserInvoiceDto>), 200)]
        [HttpGet]
        public async Task<IActionResult> GetAllInvoicesAsync()
        {
            var response = await _mediator.Send(new GetUserInvoicesQuery());

            if (response.CompletedWithSuccess)
            {
                return Ok(response.Result);
            }

            else
            {
                return BadRequest(response.OperationError);
            }
        }

        /// <summary>
        /// Gets user invoice by ID
        /// </summary>
        /// <returns>
        /// Specific user invoice
        /// </returns> 
        /// <response code="200">User invoice data</response>
        /// <response code="401">Access denied</response>
        /// <response code="404">Cars list not found</response>
        /// <response code="500">Oops! something went wrong</response>
        [ProducesResponseType(typeof(IEnumerable<UserInvoiceDto>), 200)]
        [HttpGet("{invoiceId}")]
        public async Task<IActionResult> GetInvoiceAsync([FromRoute] string invoiceId)
        {
            var response = await _mediator.Send(new GetUserInvoiceQuery(invoiceId));

            if (response.CompletedWithSuccess)
            {
                return Ok(response.Result);
            }

            else
            {
                return BadRequest(response.OperationError);
            }
        }
    }
}
