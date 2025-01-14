using AutoMapper;
using MyHealthProfile.Models;
using MyHealthProfile.Models.Dtos;

namespace DoodiServicesShopingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMap()
        {
            var MappingConnfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Patient, PatientDto>().ReverseMap();

            });
            return MappingConnfig;
        }
    }
}
