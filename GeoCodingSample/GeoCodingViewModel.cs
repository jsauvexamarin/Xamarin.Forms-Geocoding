using System;
using Xamarin.Forms.Maps;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Linq;

namespace GeoCodingSample
{
    public class GeoCodingViewModel : BaseViewModel
    {
        public GeoCodingViewModel()
        {
            _Geocoder = new Geocoder();
        }

        readonly Geocoder _Geocoder;

        Position _Position;

        public Position Position
        {
            get { return _Position; }
            set
            {
                _Position = value;
                OnPropertyChanged("Position");
            }
        }

        private string _Address;

        public string Address
        {
            get { return _Address; }
            set
            {
                _Address = value;
                OnPropertyChanged("Address");
            }
        }

        ICommand _GeoCodeCommand;

        public ICommand GeoCodeCommand
        {
            get
            {  
                return _GeoCodeCommand ?? (_GeoCodeCommand = new Command<Label>(async label => await ExecuteGeoCodeCommandCommand()));
            }
        }

        async Task ExecuteGeoCodeCommandCommand()
        {
            if (String.IsNullOrWhiteSpace(Address))
                return;        

            Position = (await _Geocoder.GetPositionsForAddressAsync(Address)).FirstOrDefault();
        }
    }
}

