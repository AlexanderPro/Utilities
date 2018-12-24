using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Utilities.Tests.Extensions
{
    enum TrafficLight
    {
        [Description("Red light.")]
        Red,
        
        [Description("Yellow light.")]
        Yellow,

        [Description("Green light.")]
        Green,

        Unspecified
    }
}