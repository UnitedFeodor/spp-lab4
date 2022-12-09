﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp; //nuget Microsoft.CodeAnalysis.CSharp
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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


        public TestsGenerator(String sourceCode)
        {
            _sourceCode = sourceCode;

            Setup();
        }



        public void Setup()
        {
            _root = CSharpSyntaxTree.ParseText(_sourceCode).GetCompilationUnitRoot();
            _usings = _root.DescendantNodes().OfType<UsingDirectiveSyntax>().ToList();
            _usings.Add(UsingDirective(
           QualifiedName(
               IdentifierName("NUnit"),
               IdentifierName("Framework"))));

            _classes = _root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
            GetClassMethods();
            GetClassNamespaces();

        }

        public List<ResultTestClass> Generate()
        {
            List<ResultTestClass> list = new();
            
            
            var members = _classes.Select(FillTestClass).ToArray();
            for (int i = 0; i < _classes.Count; i++)
            {
                CompilationUnitSyntax unit = CompilationUnit()
                    .WithUsings(new SyntaxList<UsingDirectiveSyntax>(_usings)).AddMembers(members[i]);
                list.Add(new ResultTestClass($"{_classes[i].Identifier.Text}Tests.cs", unit.NormalizeWhitespace().ToFullString()));
            }
            return list;
        }

        private MemberDeclarationSyntax FillTestClass(ClassDeclarationSyntax classDeclaration)
        {
            //var classNamespace = _namespaceClasses[classDeclaration].Last();
            var namespaceName = _namespaceClasses[classDeclaration].Last().Name;
            
            var classNamespace = NamespaceDeclaration(QualifiedName(namespaceName, IdentifierName("Tests"))); ;

            var resultMethods = new List<MemberDeclarationSyntax>();
            var methods = _classMethods[classDeclaration];
            methods.Sort((method1, method2) => string.Compare(method1.Identifier.Text, method2.Identifier.Text, StringComparison.Ordinal));
            var methodNamesDic = new Dictionary<MethodDeclarationSyntax, int>();
            methods.ForEach(method => {
                var name = method;
                var newAmount = methodNamesDic.TryGetValue(name, out var amount) ? amount + 1 : 1;
                methodNamesDic[name] = newAmount;
            });

            var methodNamesList = methodNamesDic.ToList();
            methodNamesList.Sort((method1, method2) =>
                string.Compare(method1.Key.Identifier.Text, method2.Key.Identifier.Text, StringComparison.Ordinal));

            int index = 0;
            string methodName = methodNamesList[0].Key.Identifier.Text; ;

            for (var i = 0; i < methods.Count; i++)
            {
                if (methodNamesList[i].Value != 1 && (methodName == methodNamesList[i].Key.Identifier.Text
                    || index != 0))
                {
                    index++;

                }
                else
                {
                    index = 0;
                }
                methodName = methodNamesList[i].Key.Identifier.Text;



                var methodResName = methodNamesList[i].Key.Identifier.Text + (index != 0 ? $"{index}" : "");
                resultMethods.Add(MethodDeclaration(ParseTypeName("void"), methodResName + "Test").
                                                     AddModifiers(Token(SyntaxKind.PublicKeyword)).
                                                     AddBodyStatements(ParseStatement("Assert.Fail(\"autogenerated\");")).
                                                     AddAttributeLists(AttributeList().AddAttributes(Attribute(ParseName("Test")))));
            }

            
            
            ClassDeclarationSyntax testClass = ClassDeclaration(classDeclaration.Identifier.Text + "Test").
                                               AddModifiers(Token(SyntaxKind.PublicKeyword)).
                                               AddAttributeLists(AttributeList().AddAttributes(Attribute(ParseName("TestFixture")))).
                                               AddMembers(resultMethods.ToArray());
            
            return classNamespace.AddMembers(testClass);
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