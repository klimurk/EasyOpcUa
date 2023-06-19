using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcUa.Persistance.Startup;

public record PlcConfig(string Name, string PlcAddress, string VisuAddress);
