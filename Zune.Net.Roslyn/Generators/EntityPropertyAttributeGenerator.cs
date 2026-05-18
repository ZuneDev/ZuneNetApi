using Microsoft.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace Zune.Net.Roslyn.Generators
{
    [Generator]
    public class EntityPropertyAttributeGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(static postInitializationContext =>
            {
                postInitializationContext.AddEmbeddedAttributeDefinition();
                postInitializationContext.AddSource("Attributes.g.cs", SourceText.From($$"""
                    using System;
                    using Microsoft.CodeAnalysis;
                    using {{Constants.ZuneNetOntology}};
                    namespace {{Constants.ZuneNetOntologyCore}};
                    
                    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true), Embedded]
                    internal sealed class {{Constants.EntityReferencePropertyAttribute}}(EntityFact fact) : Attribute
                    {
                        public EntityFact Fact => fact;
                    }
                    
                    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true), Embedded]
                    internal sealed class {{Constants.EntityReferenceListPropertyAttribute}}(EntityFact fact) : Attribute
                    {
                        public EntityFact Fact => fact;
                    }
                    """, Encoding.UTF8));
            });
        }
    }
}