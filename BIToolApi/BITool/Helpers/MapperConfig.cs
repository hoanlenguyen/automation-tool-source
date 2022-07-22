using BITool.Models;
using Mapster;

namespace BITool.Helpers
{
    public static class MapperConfig
    {
        public static void AddCustomConfigs()
        {
            TypeAdapterConfig<CampaignCreateOrEditDto, Campaign>.NewConfig()
                            .Ignore(s=>s.CreationTime)
                            .Ignore(s=>s.CreatorUserId)
                            .Ignore(s=>s.LastModificationTime)
                            .Ignore(s=>s.LastModifierUserId)
                            ;
        }
    }
}
