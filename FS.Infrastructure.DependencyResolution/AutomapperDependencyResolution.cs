using AutoMapper;
using System.Collections.Generic;

namespace FS.Infrastructure.DependencyResolution
{
    public class AutomapperDependencyResolution
    {
        public static void Configure(List<Profile> profiles)
        {
            if (profiles == null)
                profiles = new List<Profile>();
            
            Mapper.Initialize(cfg =>
            {
                profiles.ForEach(cfg.AddProfile);
            });
        }
    }
}
