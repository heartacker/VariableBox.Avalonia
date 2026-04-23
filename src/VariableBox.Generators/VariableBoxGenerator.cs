using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VariableBox.Generators;

[Generator]
public class VariableBoxGenerator : ISourceGenerator
{
    private const string AttributeSource = @"
using System;

namespace VariableBox;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class GenerateVariableBoxAttribute : Attribute
{
    public Type TargetType { get; }
    public string ClassName { get; }

    public GenerateVariableBoxAttribute(Type targetType, string className)
    {
        TargetType = targetType;
        ClassName = className;
    }
}
";

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new MySyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        context.AddSource("GenerateVariableBoxAttribute.g.cs", SourceText.From(AttributeSource, Encoding.UTF8));

        if (context.SyntaxReceiver is not MySyntaxReceiver receiver) return;

        foreach (var attr in receiver.Attributes)
        {
            // Parse manually from arguments
            if (attr.ArgumentList?.Arguments.Count >= 2)
            {
                var arg0 = attr.ArgumentList.Arguments[0].Expression;
                var arg1 = attr.ArgumentList.Arguments[1].Expression;

                string? targetType = null;
                if (arg0 is TypeOfExpressionSyntax typeofExp)
                {
                    targetType = typeofExp.Type.ToString();
                }

                string? className = null;
                if (arg1 is LiteralExpressionSyntax literal && arg1.Kind() == SyntaxKind.StringLiteralExpression)
                {
                    className = literal.Token.ValueText;
                }

                if (targetType != null && className != null)
                {
                    string source = GenerateClass(targetType, className);
                    context.AddSource($"{className}.g.cs", SourceText.From(source, Encoding.UTF8));
                }
            }
        }
    }

    private string GenerateClass(string targetType, string className)
    {
        return $@"
using System;
using VariableBox.Common;

namespace VariableBox;

public partial class {className}
{{
    protected override Type StyleKeyOverride => typeof(NumericUpDown);
    public {className}() : base(NumericOperations.{GetOperationName(targetType)}) {{ }}
}}
";
    }

    private string GetOperationName(string type)
    {
        return type switch
        {
            "int" => "Int",
            "uint" => "UInt",
            "long" => "Long",
            "ulong" => "ULong",
            "short" => "Short",
            "ushort" => "UShort",
            "byte" => "Byte",
            "sbyte" => "SByte",
            "float" => "Float",
            "double" => "Double",
            "decimal" => "Decimal",
            _ => "Int"
        };
    }

    class MySyntaxReceiver : ISyntaxReceiver
    {
        public List<AttributeSyntax> Attributes { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is AttributeSyntax attr && 
                (attr.Name.ToString() == "GenerateVariableBox" || attr.Name.ToString() == "GenerateVariableBoxAttribute"))
            {
                Attributes.Add(attr);
            }
        }
    }
}
