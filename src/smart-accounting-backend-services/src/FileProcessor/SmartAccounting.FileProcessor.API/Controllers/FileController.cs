using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartAccounting.FileProcessor.API.Application.Commands;
using SmartAccounting.FileProcessor.API.Application.DTO;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAccounting.FileProcessor.API.Controllers
{
    [Authorize(Policy = "AccessAsUser")]
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FileController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> Post([FromForm] FileToProcessDto fileToProcessDto, CancellationToken cancellationToken)
        {
            var uploadFileCommand = new UploadFileCommand
            {
                Files = fileToProcessDto.Files
            };


            var response = await _mediator.Send(uploadFileCommand);

            if (response.CompletedWithSuccess)
            {
                return Ok();
            }

            else
            {
                return BadRequest(response.OperationError);
            }
        }
    }
}
