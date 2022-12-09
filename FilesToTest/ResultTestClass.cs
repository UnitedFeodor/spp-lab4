

namespace Mike
{
    public class TestClass1
    {
        public string FileName { get; set}
        public string SourceCode { get; set}

        public TestClass1(string fileName, string sourceCode)
        {
            this.FileName = fileName;
            this.SourceCode = sourceCode;
        }
		
		public int callMe() {
			return 1;
		}
		
		public int callMe(int a) {
			return a+1;
		}
		
		public void bye(int a,string b) {
			string b = "hi";
		}
    }
	
	public class TestClass2
    {
        public string FileName { get; set}
        public string SourceCode { get; set}

        public TestClass1(string fileName, string sourceCode)
        {
            this.FileName = fileName;
            this.SourceCode = sourceCode;
        }
		
		public int bruh() {
			return 111;
		}
		
		public int based(int a) {
			return a+2;
		}
		
    }
}

namespace Finger
{
	public class Kid
	{
	

		public void hi(int a, string b)
		{
			
		}
	}

	public class Named
	{
		public string ha()
        {
			return "ha";
        }

	}
}