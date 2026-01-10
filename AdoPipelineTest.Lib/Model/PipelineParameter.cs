namespace AdoPipelineTest.Model;

public class PipelineParameter
{
    public required string Name { get; init; }
    public string? DisplayName { get; init; }
    public object? Value { get; init; }
    public object? DefaultValue { get; init; }
    public IList<object>? AllowedValues { get; init; }
}