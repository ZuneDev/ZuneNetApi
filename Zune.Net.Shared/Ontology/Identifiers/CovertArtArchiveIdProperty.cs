using Zune.Net.Ontology.BaseProperties;

namespace Zune.Net.Ontology.Identifiers;

public class CoverArtArchiveIdProperty(CoverArtArchiveEntityType providerEntityType, EntityType entityType)
    : TypedEntityIdProperty<string, CoverArtArchiveEntityType>(providerEntityType, entityType)
{
    public static CoverArtArchiveIdProperty ImageFront => new(CoverArtArchiveEntityType.ImageFront, EntityType.Image);
    public static CoverArtArchiveIdProperty ImageBack => new(CoverArtArchiveEntityType.ImageBack, EntityType.Image);
    public static CoverArtArchiveIdProperty Image => new(CoverArtArchiveEntityType.Image, EntityType.Image);

    public override string Parse(string value) => value;
}

public enum CoverArtArchiveEntityType
{
    Image,
    ImageFront,
    ImageBack,
}