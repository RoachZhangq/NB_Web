using System;
using Furion.ConfigurableOptions;

namespace NB.Core.Options
{
    public class ConsulOptions : IConfigurableOptions
    {
        public bool ConsulEnabled { get; set; }

        public string ServiceName { get; set; }

        public string ServiceTags { get; set; }

        public string ServiceIP { get; set; }

        public int ServicePort { get; set; }

        public string ServiceHealthCheck { get; set; }

        public string Weight { get; set; }

        public string Address { get; set; }
    }

}

