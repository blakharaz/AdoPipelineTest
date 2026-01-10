namespace AdoPipelineTest.Parsing.RawModel;

public class RawPipelineParameter
{
    public required string Name { get; init; }
    public required string Type { get; init; }
    public string? DisplayName { get; init; }
    public object? DefaultValue { get; init; }
    public IList<object>? AllowedValues { get; init; }
}