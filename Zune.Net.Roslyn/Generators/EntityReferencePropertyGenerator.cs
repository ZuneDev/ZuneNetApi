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
            var pipelineSingle = context.SyntaxProvider.ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: Constants.EntityReferencePropertyAttributeFqn,
                predicate: static (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax,
                transform: GetTransform(false)
            );
            context.RegisterSourceOutput(pipelineSingle, ProduceSource);
            
            var pipelineList = context.SyntaxProvider.ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: Constants.EntityReferenceListPropertyAttributeFqn,
                predicate: static (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax,
                transform: GetTransform(true)
            );
            context.RegisterSourceOutput(pipelineList, ProduceSource);
        }

        private static Func<GeneratorAttributeSyntaxContext, CancellationToken, EntityReferencePropertyModel>
            GetTransform(bool isReferenceListProperty)
        {
            return (context, _) =>
            {
                var factEnums = context.Attributes.Select(a => a.ConstructorArguments.First())
                    .Select(c => c.ToCSharpString())
                    .ToArray();

                var containingClass = context.TargetSymbol.ContainingType;
                var ns = containingClass.ContainingNamespace?.ToDisplayString(
                    SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(
                        SymbolDisplayGlobalNamespaceStyle.Omitted)) ?? throw new NotSupportedException();

                return new EntityReferencePropertyModel(
                    Namespace: ns,
                    ParentClassName: containingClass.Name,
                    EntityName: context.TargetSymbol.Name,
                    FactEnums: new EquatableArray<string>(factEnums),
                    IsList: isReferenceListProperty);
            };
        }

        private static void ProduceSource(SourceProductionContext context, EntityReferencePropertyModel model)
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
                    sourceWriter.WriteLine();
                    
                    foreach (var factEnum in model.FactEnums)
                    {
                        var memberAccessorIndex = factEnum.LastIndexOf('.');
                        var factName = factEnum[(memberAccessorIndex + 1)..];

                        if (!model.IsList)
                        {
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
                        else
                        {
                            // Generate untyped reference property
                            sourceWriter.WriteLine($"public static TypedEntityListProperty<T> {factName}Ids<T>(ITypedEntityProperty<T> idProp)");
                            sourceWriter.IncreaseIndent();
                            sourceWriter.WriteLine($"=> new(EntityType.{model.EntityName}, {factEnum}, idProp);");
                            sourceWriter.DecreaseIndent();
                            
                            // Generate typed reference property
                            sourceWriter.WriteLine($"public static EntityListProperty {factName}Ids(IEntityProperty idProp)");
                            sourceWriter.IncreaseIndent();
                            sourceWriter.WriteLine($"=> new(EntityType.{model.EntityName}, {factEnum}, idProp);");
                            sourceWriter.DecreaseIndent();
                        }
                        
                        sourceWriter.WriteLine();
                    }
                }
            }
            
            var propertyCategoryName = model.IsList ? "ReferenceList" : "Reference";

            context.AddSource($"{model.EntityName}_Entity{propertyCategoryName}Properties.g.cs",
                SourceText.From(sourceWriter.ToString(), Encoding.UTF8));
        }

        private record EntityReferencePropertyModel(string Namespace, string ParentClassName, string EntityName,
            EquatableArray<string> FactEnums, bool IsList);
    }
}