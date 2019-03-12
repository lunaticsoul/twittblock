using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwittBlock.WebUtils
{
    public class WebForm
    {
        public string Method { get; set; }
        public string Action { get; set; }
        public List<WebInput> Inputs { get; set; } = new List<WebInput>();
    }
}
