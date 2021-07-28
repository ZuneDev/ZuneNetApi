using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zune.DB.Models
{
    public class Tuner
    {
        public Tuner()
        {

        }

        public Tuner(Xml.Commerce.TunerRegisterInfo regInfo = null)
        {
            if (regInfo != null)
                SetFromTunerRegistrationInfo(regInfo);
        }

        [Key]
        public string Id { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public Member Member { get; set; }

        public void SetFromTunerRegistrationInfo(Xml.Commerce.TunerRegisterInfo regInfo)
        {
            Id = regInfo.Id;
        }

        public Xml.Commerce.TunerRegisterInfo GetTunerRegisterInfo()
        {
            return new Xml.Commerce.TunerRegisterInfo
            {
                Id = this.Id
            };
        }
    }
}
