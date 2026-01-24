using AdoPipelineTest.Model;
using AdoPipelineTest.Parsing;

namespace AdoPipelineTest.UnitTests.Parsing;

[TestFixture]
public class StepsParserTest
{
    [Test]
    public void ParseStep_WithEmptyScriptNode_ThrowsInvalidPipelineException()
    {
        var ex = Assert.Throws<InvalidPipelineException>(
            () => PipelineParser.Parse("test_data/pipeline_parser/pipeline_with_empty_script.yaml")
        );
        
        Assert.That(ex?.Message, Does.Contain("script node has no content"));
    }
}
