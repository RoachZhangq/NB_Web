using Furion.ConfigurableOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NB.Core.Options
{
    public class SerilogOptions : IConfigurableOptions
    {
        public string AppName { get; set; }
        public SerilongOptionsWriteToTencentCloud WriteToTencentCloud { get; set; }
    }

    public class SerilongOptionsWriteToTencentCloud
    {
        public string TopicId { get; set; }
        public string RequestBaseUri { get; set; }
    }

    public class ParameterOptions : IConfigurableOptions
    {
        public string OrganizationCode { get; set; }
        public int DistributorId { get; set; }
        public string Seller { get; set; }

        public string IdentityClientAddress { get; set; }

        public string AppId { get; set; }

        public string AppSecret { get; set; }
        public string PayMerchant { get; set; }
    }

    public class RedisOptions : IConfigurableOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
        public int PoolTimeOutSeconds { get; set; }
        public int DbIndex { get; set; }
        public string PrefixKey { get; set; }
    }
}
