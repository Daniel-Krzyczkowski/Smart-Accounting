namespace SmartAccounting.Common.CommonResponse
{
    public record OperationError
    {
        public string Details { get; }

        public OperationError(string details) => (Details) = (details);
    }
}
