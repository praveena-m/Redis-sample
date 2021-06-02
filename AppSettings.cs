using System.Collections.Generic;

namespace Company.Core.Helpers
{
    public class AppSettings
    {      
        public Redis Redis { get; set; }
    }    

    public class Redis
    {
        public bool CachingEnabled { get; set; }
        public string KeyPrefix { get; set; }
        public string KeyExpiration { get; set; }
        public string ConnectionString { get; set; }
    }
}
