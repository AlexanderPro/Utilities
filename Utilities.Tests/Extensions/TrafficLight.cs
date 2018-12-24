using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Utilities.Tests.Extensions
{
    enum TrafficLight
    {
        [Display(Name = "Red", ShortName = "R", Description = "Red light"), Description("Red light")]
        Red,

        [Display(Name = "Yellow", ShortName = "Y", Description = "Yellow light"), Description("Yellow light")]
        Yellow,

        [Display(Name = "Green", ShortName = "G", Description = "Green light"), Description("Green light")]
        Green,

        Unspecified
    }
}