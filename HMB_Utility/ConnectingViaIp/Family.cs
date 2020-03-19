using System.Collections.ObjectModel;

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
            set
            {
                families = value;
            }
        }

        public Family()
        {
            Families = new ObservableCollection<HBMFamily>() { new HBMFamily(AppSettings.PmxName, AppSettings.PmxPort), new HBMFamily(AppSettings.QuantumxName, AppSettings.QuantumXPort), new HBMFamily(AppSettings.MgcName, AppSettings.MgcPort) };
        }

        public HBMFamily SelectedFamily { get; set; }


    }
}
