using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Pack.MenuItems
{
    interface IContextMenuItem
    {
        void SearchValue(object sender, EventArgs e);
    }
}
