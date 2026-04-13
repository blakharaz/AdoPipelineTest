namespace AdoPipelineTest.UnitTests;

internal class Program
{
    public static int Main(string[] args)
    {
        return global::Xunit.Runner.InProc.SystemConsole.ConsoleRunner.Run(args).GetAwaiter().GetResult();
    }
}

