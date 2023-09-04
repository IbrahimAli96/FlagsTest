using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlagsTest.Models
{
    public class WeatherCurrentModel
    {
        public int Temperature { get; set; }
        public string[] Weather_Descriptions { get; set; }
        public string[] Weather_Icons { get; set; }
    }
}
