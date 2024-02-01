using Furion.ConfigurableOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NB.Core.Options
{
    public class RemoteRequestOptions : IConfigurableOptions
    {
        public List<ConfigureClient> Clients { get; set; }
    }

    public class ConfigureClient
    {
        public string Name { get; set; }
        public string BaseAddress { get; set; }

    }
}
