using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlagsTest.Models
{
    public class WeatherModel
    {
        public WeatherLocationModel Location { get; set; }
        public WeatherCurrentModel Current { get; set; }
    }
}
