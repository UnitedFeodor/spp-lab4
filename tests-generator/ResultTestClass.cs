

namespace tests_generator
{
    public class GeneratedTestClass
    {
        public string FileName { get; set}
        public string SourceCode { get; set}

        public GeneratedTestClass(string fileName, string sourceCode)
        {
            this.FileName = fileName;
            this.SourceCode = sourceCode;
        }
    }
}