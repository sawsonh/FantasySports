using FS.Core.Entities;

namespace FS.Core.Services
{
    public interface IConfigService : IDataService<AppSetting>
    {
        int AppId { get; set; }
        T Get<T>(string key);
    }
}
