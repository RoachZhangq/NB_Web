using Furion.ConfigurableOptions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NB.Core.Options
{
    public class ConnectionConfigOptions : IConfigurableOptions
    {
        public List<ConnectionConfig> Connections { get; set; }
    }

}
