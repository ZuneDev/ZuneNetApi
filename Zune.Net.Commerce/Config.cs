using System;
using System.Reflection;

namespace CommerceZuneNet
{
    public class Config
    {
        public string Host { get; set; }
        public string Port { get; set; } = "80";
        public string SslPort { get; set; } = "443";

        public Config() { }

        public Config(string[] args)
        {
            Type cfgType = typeof(Config);
            foreach (string arg in args)
            {
                string key;
                object value;

                int idx = arg.IndexOf('=');
                if (idx >= 0)
                {
                    key = arg[..idx];
                    value = arg[(idx + 1)..];
                }
                else
                {
                    key = arg;
                    value = bool.TrueString;
                }

                PropertyInfo prop = cfgType.GetProperty(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (prop != null)
                {
                    prop.SetValue(this, value);
                }
                else
                {
                    Console.WriteLine($"Property \"{key}\" does not exist on {nameof(Config)}.");
                }
            }
        }
    }
}
