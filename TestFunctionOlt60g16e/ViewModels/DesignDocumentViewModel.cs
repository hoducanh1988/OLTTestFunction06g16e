using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFunctionOlt60g16e.Models;

namespace TestFunctionOlt60g16e.ViewModels {
    public class DesignDocumentViewModel {

        public DesignDocumentViewModel() {
            _design = new DesignDocumentModel();
        }

        //binding setting name
        DesignDocumentModel _design;
        public DesignDocumentModel Design {
            get { return _design; }
        }

    }
}
