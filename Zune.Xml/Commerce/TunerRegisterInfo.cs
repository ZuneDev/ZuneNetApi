namespace Zune.Xml.Commerce
{
    public class TunerRegisterInfo
    {
#if NETSTANDARD1_1_OR_GREATER
        [System.ComponentModel.DataAnnotations.Key]
#endif
        public string Id { get; set; }
    }
}