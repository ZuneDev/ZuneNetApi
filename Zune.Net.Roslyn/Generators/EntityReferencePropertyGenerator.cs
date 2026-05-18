using Microsoft.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Zune.Net.Roslyn.Helpers;

namespace Zune.Net.Roslyn.Generators
{
    [Generator]
    public class EntityReferencePropertyGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: Constants.EntityReferencePropertyAttributeFqn,
                predicate: static (syntaxNode, cancellationToken) => syntaxNode is ClassDeclarationSyntax,
                transform: static (context, cancellationToken) =>
                {
                    var factEnums = context.Attributes
                        .Select(a => a.ConstructorArguments.First())
                        .Select(c => c.ToCSharpString())
                        .ToArray();
                    
                    var containingClass = context.TargetSymbol.ContainingType;
                    
                    return new EntityReferencePropertyModel(
                        // Note: this is a simplified example. You will also need to handle the case where the type is in a global namespace, nested, etc.
                        Namespace: containingClass.ContainingNamespace?
                            .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted))
                            ?? throw new NotSupportedException(),
                        ParentClassName: containingClass.Name,
                        EntityName: context.TargetSymbol.Name,
                        FactEnums: new EquatableArray<string>(factEnums));
                }
            );

            context.RegisterSourceOutput(pipeline, static (context, model) =>
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
                        foreach (var factEnum in model.FactEnums)
                        {
                            var memberAccessorIndex = factEnum.LastIndexOf('.');
                            var factName = factEnum[(memberAccessorIndex + 1)..];
                            
                            // Generate typed reference property
                            sourceWriter.WriteLine($"public static TypedEntityReferenceProperty<TId, TProviderEntities> {factName}Id<TId, TProviderEntities>(ITypedEntityIdProperty<TId, TProviderEntities> idProp)");
                            sourceWriter.IncreaseIndent();
                            sourceWriter.WriteLine($"where TProviderEntities : Enum");
                            sourceWriter.WriteLine($"=> new(EntityType.{model.EntityName}, {factEnum}, idProp);");
                            sourceWriter.DecreaseIndent();
                            
                            // Generate untyped reference property
                            sourceWriter.WriteLine($"public static EntityReferenceProperty {factName}Id(IEntityIdProperty idProp)");
                            sourceWriter.IncreaseIndent();
                            sourceWriter.WriteLine($"=> new(EntityType.{model.EntityName}, {factEnum}, idProp);");
                            sourceWriter.DecreaseIndent();
                        }
                    }
                }

                context.AddSource(
                    $"{model.EntityName}_EntityReferenceProperties.g.cs",
                    SourceText.From(sourceWriter.ToString(), Encoding.UTF8));
            });
        }

        private record EntityReferencePropertyModel(string Namespace, string ParentClassName, string EntityName,
            EquatableArray<string> FactEnums);
    }
}