using testAspOracle01.Models;

namespace testAspOracle01.Profiles {
    public class MapperProfile : AutoMapper.Profile {
        public MapperProfile () {
            CreateMap<AppUserToolModel, testModel> ()
            .ForMember (d => d.ID, o => o.MapFrom(s => s.TOOL_ID))
            .ForMember (d => d.appName, o => o.MapFrom (s => s.TOOL_NAME))
            .ForMember (d => d.img, o => o.MapFrom (s => s.ICON_SRC));
        }
    }
}