namespace AdoPipelineTest.Model;

public class InvalidPipelineException : Exception
{
    public string? FilePath { get; }
    public int? LineNumber { get; }
    public int? ColumnNumber { get; }

    public override string Message
    {
        get
        {
            var baseMessage = base.Message;
            if (FilePath != null && LineNumber.HasValue && ColumnNumber.HasValue)
            {
                return $"{baseMessage} (at {FilePath}:{LineNumber}:{ColumnNumber})";
            }
            return baseMessage;
        }
    }

    public InvalidPipelineException()
    {
    }

    public InvalidPipelineException(string message) : base(message)
    {
    }

    public InvalidPipelineException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public InvalidPipelineException(string message, string filePath, int lineNumber, int columnNumber) 
        : base(message)
    {
        FilePath = filePath;
        LineNumber = lineNumber;
        ColumnNumber = columnNumber;
    }

    public InvalidPipelineException(string message, string filePath, int lineNumber, int columnNumber, Exception innerException) 
        : base(message, innerException)
    {
        FilePath = filePath;
        LineNumber = lineNumber;
        ColumnNumber = columnNumber;
    }
}