using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMB_Utility
{
    public class Family
    {
        private ObservableCollection<HBMFamily> families;
        public ObservableCollection<HBMFamily> Families
        {
            get
            {
                return families;
            }
        }

        public Family()
        {
            families = new ObservableCollection<HBMFamily>() { new HBMFamily(AppSettings.PmxName, AppSettings.PmxPort), new HBMFamily(AppSettings.QuantumxName, AppSettings.QuantumXPort), new HBMFamily(AppSettings.MgcName, AppSettings.MgcPort) };
        }

        public HBMFamily SelectedFamily { get; set; }


    }
}
