using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La.ViewModel.Pram
{
    public interface IPramVm
    {
        void Clean();
        bool IsDirty { get; }
    }
}
