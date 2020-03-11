using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hbm.Api.Common.Entities.Signals;

namespace HMB_Utility
{
    public class Filter
    {
        private ObservableCollection<SignalType> signalTypes;
        public ObservableCollection<SignalType> SignalTypes
        {
            get
            {
                return signalTypes;
            }
        }
        public SignalType SelectedType { get; set; }

        public Filter()
        {
            signalTypes = new ObservableCollection<SignalType> { new SignalType("Analog Input"), new SignalType("Digital Signals"), new SignalType("All Signals"), new SignalType("Can be measured by DAQ") };
            SelectedType = signalTypes[2];
        }

        public bool Check(FoundSignal sig)
        {
            switch (SelectedType.Name)
            {
                case "Analog Input":
                    return (sig.HbmSignal is AnalogInSignal);
                case "Digital Signals":
                    return (sig.HbmSignal is DigitalSignal);
                case "Can be measured by DAQ":
                    return (sig.HbmSignal.IsMeasurable);
                case "All Signals":
                    return (sig.HbmSignal is Signal);
                default:
                    return (sig.HbmSignal is Signal);
            }
        }
    }
}
