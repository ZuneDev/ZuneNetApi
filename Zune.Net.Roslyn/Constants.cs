namespace Zune.Net.Roslyn;

public static class Constants
{
    public const string ZuneNet = "Zune.Net";
    
    public const string ZuneNetOntology = $"{ZuneNet}.Ontology";
    
    public const string ZuneNetOntologyCore = $"{ZuneNetOntology}.Core";
    
    public const string ZuneNetOntologyBaseProperties = $"{ZuneNetOntology}.BaseProperties";
    
    public const string ZuneNetOntologyIdentifiers = $"{ZuneNetOntology}.Identifiers";
    
    public const string EntityReferencePropertyAttribute = "EntityReferencePropertyAttribute";
    public const string EntityReferencePropertyAttributeFqn = $"{ZuneNetOntologyCore}.{EntityReferencePropertyAttribute}";
    
    public const string EntityReferenceListPropertyAttribute = "EntityReferenceListPropertyAttribute";
    public const string EntityReferenceListPropertyAttributeFqn = $"{ZuneNetOntologyCore}.{EntityReferenceListPropertyAttribute}";
}