using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SmartAccounting.Common.CommonResponse;
using SmartAccounting.FileProcessor.API.Application.ErrorHandling;
using SmartAccounting.FileProcessor.API.BackgroundServices.Channels;
using SmartAccounting.FileProcessor.API.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartAccounting.FileProcessor.API.Application.Commands
{
    internal class UploadFileCommand : IRequest<OperationResponse>
    {
        public IList<IFormFile> Files { get; set; }
    }

    internal class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, OperationResponse>
    {
        private readonly ILogger<UploadFileCommandHandler> _logger;
        private readonly IIdentityService _identityService;
        private readonly FileProcessingChannel _fileProcessingChannel;

        public UploadFileCommandHandler(ILogger<UploadFileCommandHandler> logger,
                                        IIdentityService identityService,
                                        FileProcessingChannel fileProcessingChannel)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _fileProcessingChannel = fileProcessingChannel ?? throw new ArgumentNullException(nameof(fileProcessingChannel));
        }

        public async Task<OperationResponse> Handle(UploadFileCommand request,
                                                    CancellationToken cancellationToken)
        {
            var filesToUpload = request.Files;
            var userIdentity = _identityService.GetUserIdentity()
                                               .ToString();

            if (filesToUpload == null)
            {
                _logger.LogWarning("No file found to upload");
                return new OperationResponse()
                                .SetAsFailureResponse(OperationErrorDictionary.FileUpload.NoFileFound());
            }

            long size = filesToUpload.Sum(f => f?.Length ?? 0);
            if (size == 0)
            {
                _logger.LogWarning("No file found to upload");
                return new OperationResponse()
                                .SetAsFailureResponse(OperationErrorDictionary.FileUpload.NoFileFound());
            }

            foreach (var formFile in filesToUpload)
            {
                var fileTempPath = @$"{Path.GetTempPath()}{formFile.FileName}";

                using (var stream = new FileStream(fileTempPath, FileMode.Create, FileAccess.Write))
                {
                    await formFile.CopyToAsync(stream, cancellationToken);
                }

                var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(3));

                try
                {
                    var fileProcessingChannelElement = new FileProcessingChannelElement
                    {
                        FilePath = fileTempPath,
                        UserId = userIdentity
                    };

                    var fileWritten = await _fileProcessingChannel.AddFileAsync(fileProcessingChannelElement, cts.Token);

                    if (!fileWritten)
                    {
                        _logger.LogError($"An error occurred when processing file: {formFile.FileName}");
                        return new OperationResponse()
                                        .SetAsFailureResponse(OperationErrorDictionary.FileUpload
                                                                                        .UploadFailed(formFile.FileName));
                    }
                }
                catch (OperationCanceledException) when (cts.IsCancellationRequested)
                {
                    File.Delete(fileTempPath);
                    throw;
                }
            }

            return new OperationResponse();
        }
    }
}
