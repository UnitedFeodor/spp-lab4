

using System.Threading.Tasks.Dataflow;
using tests_generator;
using tests_generator.app;


var dir = TryGetSolutionDir.TryGetSolutionDirectoryInfo().FullName;

var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
var executionDataFlowBlockOptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 5 };
TransformBlock<string, string> readAllText = new TransformBlock<string, string>
(
    async filePath => await File.ReadAllTextAsync(filePath),
    executionDataFlowBlockOptions
);

TransformManyBlock<string, ResultTestClass> generateTestFiles = new TransformManyBlock<string, ResultTestClass>
(
    sourceCode =>
    {
        TestsGenerator generator = new(sourceCode);
        return generator.Generate().ToArray();
    },
    executionDataFlowBlockOptions
);


ActionBlock<ResultTestClass> saveTestFiles = new ActionBlock<ResultTestClass>
(
    async testsFile => await File.WriteAllTextAsync(dir + "\\TestResults\\" + testsFile.FileName, testsFile.SourceCode),
    executionDataFlowBlockOptions
);


readAllText.LinkTo(generateTestFiles, linkOptions);
generateTestFiles.LinkTo(saveTestFiles, linkOptions);


string[] filePaths = Directory.GetFiles(dir + "\\FilesToTest\\");

foreach (string filePath in filePaths)
{
    if (filePath.EndsWith(".cs"))
        readAllText.Post(filePath);
}
readAllText.Complete();
saveTestFiles.Completion.Wait();