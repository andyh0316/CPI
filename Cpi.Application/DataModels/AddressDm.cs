using Cpi.Application.DataModels.Base;
using System.ComponentModel.DataAnnotations;

namespace Cpi.Application.DataModels
{
    public class AddressDm : BaseDm
    {
        [MaxLength(100)]
        public string Street { get; set; }

        [MaxLength(100)]
        public string Vilas { get; set; }

        [MaxLength(100)]
        public string Sonka { get; set; }

        [MaxLength(100)]
        public string District { get; set; }

        [MaxLength(100)]
        public string City { get; set; }
    }

    public class AddressMap : BaseMap<AddressDm>
    {
        public AddressMap()
        {
            ToTable("Address");
        }
    }
}
