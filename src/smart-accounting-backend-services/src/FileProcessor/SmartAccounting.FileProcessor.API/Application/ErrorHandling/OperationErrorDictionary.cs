using SmartAccounting.Common.CommonResponse;

namespace SmartAccounting.FileProcessor.API.Application.ErrorHandling
{
    public static class OperationErrorDictionary
    {
        public static class FileUpload
        {
            public static OperationError UploadFailed(string fileName) =>
               new OperationError($"An error occurred when processing file: {fileName}");

            public static OperationError NoFileFound() =>
               new OperationError("No file found to upload");
        }
    }
}
