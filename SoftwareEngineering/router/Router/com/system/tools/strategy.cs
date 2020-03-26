using router.com.system;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router.com.system.tools
{
    interface strategy
    {
        List<vehicle> getVehicles(List<kid> k, List<vehicle> v);
        void init(List<kid> k, List<vehicle> v);
    }
}
