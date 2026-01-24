using YamlDotNet.RepresentationModel;

namespace AdoPipelineTest.Model;

public class InvalidPipelineException : Exception
{
    public string? FilePath { get; }
    public long? StartLineNumber { get; }
    public long? StartColumnNumber { get; }
    public long? EndLineNumber { get; }
    public long? EndColumnNumber { get; }

    public override string Message
    {
        get
        {
            var baseMessage = base.Message;
            if (FilePath == null || !StartLineNumber.HasValue || !StartColumnNumber.HasValue)
            {
                return baseMessage;
            }
            
            if (EndLineNumber.HasValue && EndColumnNumber.HasValue)
            {
                return $"{baseMessage} (at {FilePath}:{StartLineNumber}:{StartColumnNumber} - {EndLineNumber}:{EndColumnNumber})";
            }
            
            return $"{baseMessage} (at {FilePath}:{StartLineNumber}:{StartColumnNumber})";
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

    public InvalidPipelineException(string message, string filePath, long startLineNumber, long startColumnNumber) 
        : base(message)
    {
        FilePath = filePath;
        StartLineNumber = startLineNumber;
        StartColumnNumber = startColumnNumber;
    }

    public InvalidPipelineException(string message, string filePath, long startLineNumber, long startColumnNumber, long endLineNumber, long endColumnNumber) 
        : base(message)
    {
        FilePath = filePath;
        StartLineNumber = startLineNumber;
        StartColumnNumber = startColumnNumber;
        EndLineNumber = endLineNumber;
        EndColumnNumber = endColumnNumber;
    }

    public InvalidPipelineException(string message, string filePath, long startLineNumber, long startColumnNumber, Exception innerException) 
        : base(message, innerException)
    {
        FilePath = filePath;
        StartLineNumber = startLineNumber;
        StartColumnNumber = startColumnNumber;
    }

    public InvalidPipelineException(string message, string filePath, long startLineNumber, long startColumnNumber, long endLineNumber, long endColumnNumber, Exception innerException) 
        : base(message, innerException)
    {
        FilePath = filePath;
        StartLineNumber = startLineNumber;
        StartColumnNumber = startColumnNumber;
        EndLineNumber = endLineNumber;
        EndColumnNumber = endColumnNumber;
    }

    public InvalidPipelineException(string message, string filePath, YamlNode yamlNode)
       : this(message, filePath, yamlNode.Start.Line, yamlNode.Start.Column, yamlNode.End.Line, yamlNode.End.Column)
    {
    }
}