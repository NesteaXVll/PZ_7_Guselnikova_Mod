using Module_PZ_3.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module_PZ_3.Services
{
    public class Helper
    {
        private static Real_Estate_AgencyEntities _context;

        public static Real_Estate_AgencyEntities GetContext()
        {
            if (_context == null)
            {
                _context = new Real_Estate_AgencyEntities();
            }
            return _context;
        }
    }
}
