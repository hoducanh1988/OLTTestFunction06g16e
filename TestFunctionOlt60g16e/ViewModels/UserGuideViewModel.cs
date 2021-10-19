using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFunctionOlt60g16e.Models;

namespace TestFunctionOlt60g16e.ViewModels {

    public class UserGuideViewModel {

        public UserGuideViewModel() {
            _guide = new UserGuideModel();
        }

        //binding setting name
        UserGuideModel _guide;
        public UserGuideModel Guide {
            get { return _guide; }
        }
    }
}
