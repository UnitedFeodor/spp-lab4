

namespace tests_generator
{
    public class ResultTestClass
    {
        public string FileName { get; set; }
        public string SourceCode { get; set; }

        public ResultTestClass(string fileName, string sourceCode)
        {
            this.FileName = fileName;
            this.SourceCode = sourceCode;
        }
    }
}