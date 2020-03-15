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
            signalTypes = new ObservableCollection<SignalType> { new SignalType(AppSettings.signalTypeAnalogIn), new SignalType(AppSettings.signalTypeDigital), new SignalType(AppSettings.signalTypeAll), new SignalType(AppSettings.signalTypeCanBeMeasByDAQ) };
            SelectedType = signalTypes[2];
        }

        public bool Check(FoundSignal sig)
        {
            bool result = false;

            if (SelectedType.Name == AppSettings.signalTypeAnalogIn)
            {
                result = (sig.HbmSignal is AnalogInSignal);
            }
            else if(SelectedType.Name == AppSettings.signalTypeDigital)
            {
                result = (sig.HbmSignal is DigitalSignal);
            }
            else if(SelectedType.Name == AppSettings.signalTypeCanBeMeasByDAQ)
            {
                result = (sig.HbmSignal.IsMeasurable);
            }
            else if(SelectedType.Name == AppSettings.signalTypeAll)
            {
                result = (sig.HbmSignal is Signal);
            }

            return result;
            
        }
    }
}
