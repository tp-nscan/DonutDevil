using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibNode;

namespace La.ViewModel.Design
{
    public class DesignZeusSnapVm : ZeusSnapVm
    {
        public DesignZeusSnapVm() 
            : base(DesignAthenaTr, "hi there")
        {
        }

        public static AthenaTr DesignAthenaTr
        {
            get
            {
                return null;
            }
        }
    }
}
