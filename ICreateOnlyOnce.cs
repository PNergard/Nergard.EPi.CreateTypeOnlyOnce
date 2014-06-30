using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer;

namespace Nergard.EPi.CreateTypeOnlyOnce
{
    public interface ICreateOnlyOnce
    {
        bool CreateOnlyOnce();
    }
}
