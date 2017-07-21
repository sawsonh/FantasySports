using System;
using FS.Core.Services;
using System.Collections.Generic;
using FS.Core.Entities;
using System.Configuration;
using System.Linq;
using FS.Core.Repositories;

namespace FS.Infrastructure.Services
{
    public class ConfigService : DataService<AppSetting>, IConfigService
    {
        private static object Lock = new object();
        private static Dictionary<int, Dictionary<string, string>> _appSettings;
        
        private readonly IDataRepository<AppSetting> _repo;

        public ConfigService(IDataRepository<AppSetting> repo) : base(repo)
        {
            _repo = repo;
        }

        private int? _appId;
        public int AppId
        {
            get
            {
                return _appId ?? Convert.ToInt32(ConfigurationManager.AppSettings["AppId"]);
            }
            set
            {
                _appId = value;
            }
        }

        public T Get<T>(string keyName)
        {
            lock (Lock)
            {
                if (_appSettings == null)
                    _appSettings = new Dictionary<int, Dictionary<string, string>>();
                if (!_appSettings.ContainsKey(AppId))
                    _appSettings[AppId] = _repo.GetList(setting => setting.AppId == AppId)
                        .ToDictionary(setting => setting.KeyName, setting => setting.Value, StringComparer.OrdinalIgnoreCase);
                if (!_appSettings[AppId].ContainsKey(keyName))
                {
                    var appSetting = _repo.GetSingle(setting => setting.AppId == AppId && setting.KeyName.Equals(keyName, StringComparison.OrdinalIgnoreCase));
                    if (appSetting == null)
                        _appSettings[AppId][keyName] = appSetting == null ? string.Empty : appSetting.Value;
                }
            }
            if (string.IsNullOrEmpty(_appSettings[AppId][keyName]))
                return default(T);
            return (T)Convert.ChangeType(_appSettings[AppId][keyName], typeof(T));
        }
        
        public override void Update(params AppSetting[] settings)
        {
            foreach (var setting in settings)
            {
                var appSetting = _repo.GetSingle(d => d.Id == setting.Id);
                if (appSetting == null)
                {
                    throw new Exception($"Cannot find app settings for Id {setting.Id}");
                }
                else
                {
                    var hasChanged = !appSetting.Value.Equals(setting.Value, StringComparison.OrdinalIgnoreCase);
                    if (hasChanged)
                    {
                        appSetting.Value = setting.Value;
                        _repo.Update(appSetting);
                    }
                }
            }
        }
    }
}
