using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFunctionOlt60g16e.Models;

namespace TestFunctionOlt60g16e.ViewModels {
    public class AboutViewModel {

        public AboutViewModel() {
            _abouts.Add(new AboutModel() { ID = "1", Version="OLT001VN0U0001", Content = "Phát hành lần đầu", Date = "08/03/2021", ChangeType = "Tạo mới", Person = "Hồ Đức Anh" });
        }

        private ObservableCollection<AboutModel> _abouts = new ObservableCollection<AboutModel>();
        public ObservableCollection<AboutModel> Abouts {
            get { return _abouts; }
            set { _abouts = value; }
        }
    }
}
