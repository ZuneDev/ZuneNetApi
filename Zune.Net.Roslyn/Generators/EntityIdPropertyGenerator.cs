using Microsoft.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Zune.Net.Roslyn.Helpers;

namespace Zune.Net.Roslyn.Generators
{
    [Generator]
    public class EntityIdPropertyGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: Constants.EntityIdPropertyAttributeFqn,
                predicate: static (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax,
                transform: (ctx, _) =>
                {
                    var containingClass = ctx.TargetSymbol.ContainingType;
                    var ns = containingClass.ContainingNamespace?.ToDisplayString(
                        SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(
                            SymbolDisplayGlobalNamespaceStyle.Omitted)) ?? throw new NotSupportedException();

                    return new EntityIdPropertyModel(
                        Namespace: ns,
                        ParentClassName: containingClass.Name,
                        EntityName: ctx.TargetSymbol.Name);
                }
            );
            context.RegisterSourceOutput(pipeline, ProduceSource);
        }

        private static void ProduceSource(SourceProductionContext context, EntityIdPropertyModel model)
        {
            using var sourceWriter = new IndentedTextWriter();

            sourceWriter.WriteLine($"using System;");
            sourceWriter.WriteLine($"using {Constants.ZuneNetOntologyBaseProperties};");
            sourceWriter.WriteLine();
            sourceWriter.WriteLine($"namespace {model.Namespace};");
            sourceWriter.WriteLine();

            sourceWriter.WriteLine($"partial class {model.ParentClassName}");
            using (sourceWriter.WriteBlock())
            {
                sourceWriter.WriteLine($"partial class {model.EntityName}");
                using (sourceWriter.WriteBlock())
                {
                    // Generate generic ID property
                    sourceWriter.WriteLine($"public static readonly EntityIdProperty Id = new(EntityType.{model.EntityName}, EntityFact.Id);");
                }
            }

            context.AddSource($"{model.EntityName}_EntityIdProperties.g.cs",
                SourceText.From(sourceWriter.ToString(), Encoding.UTF8));
        }

        private record EntityIdPropertyModel(string Namespace, string ParentClassName, string EntityName);
    }
}