using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp; //nuget Microsoft.CodeAnalysis.CSharp
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace tests_generator
{
    public class TestsGenerator
    {

        private String _sourceCode;
        private List<UsingDirectiveSyntax> _usings;
        private CompilationUnitSyntax _root;
        private List<ClassDeclarationSyntax> _classes;
        private Dictionary<ClassDeclarationSyntax, List<MethodDeclarationSyntax>> _classMethods = new();
        private Dictionary<ClassDeclarationSyntax, List<NamespaceDeclarationSyntax>> _namespaceClasses = new();


        TestsGenerator(String sourceCode)
        {
            _sourceCode = sourceCode;

            Setup();
        }

        public void Setup()
        {
            _root = CSharpSyntaxTree.ParseText(_sourceCode).GetCompilationUnitRoot();
            _usings = _root.DescendantNodes().OfType<UsingDirectiveSyntax>().ToList();
            _classes = _root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            GetClassMethods();
            GetClassNamespaces();

        }

        public List<ResultTestClass> GetResultTestClasses()
        {


        }



        public void GetClassNamespaces()
        {

            foreach (var node in _classes)
            {
                _namespaceClasses.Add(node, node.Ancestors().OfType<NamespaceDeclarationSyntax>().ToList());

            }
 

        }

        public void GetClassMethods()
        {

            foreach (var node in _classes)
            {
                _classMethods.Add(node, GetPublicMethods(node));

            }


        }

        public List<MethodDeclarationSyntax> GetPublicMethods(ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Where(node => node.Modifiers.Any(n => n.IsKind(SyntaxKind.PublicKeyword))).ToList();
        }

        

    }
}