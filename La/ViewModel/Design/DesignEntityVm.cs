using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibNode;

namespace La.ViewModel.Design
{
    public class DesignEntityVm : EntityVm
    {
        public DesignEntityVm() : base(null)
        {
        }

        private static Entity _designEntity;

        //static Entity DesignEntity
        //{
        //    get
        //    {
        //        if (_designEntity == null)
        //        {
        //            _designEntity = new Entity(name: Entvert.ToEntityName("Design name"), 
        //                                       entityId: Entvert.ToEntityId(Guid.NewGuid()),
        //                                       generatorId: new GeneratorId("Design generator name", 1)
                                                
        //                                       );
        //        }
        //    }
        //}
    }
}
