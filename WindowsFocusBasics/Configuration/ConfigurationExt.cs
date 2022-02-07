using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace System.Configuration
{
    public static class ConfigurationExt
    {
        private static readonly KeyValueConfigurationCollection _Settings;
                
        static ConfigurationExt()
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _Settings = configuration.AppSettings.Settings;
        }

        public static KeyValueConfigurationElement GetSetting(this AppDomain domain, string key, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
        {
            if (_Settings.AllKeys.Any(p=> string.Equals(p,key, comparison)))
            {
                return _Settings[key];
            }
            else
            {
                throw new KeyNotFoundException($"配置文件中不存在名为 {key} 的应用程序设置");
            }
        }
    }
}