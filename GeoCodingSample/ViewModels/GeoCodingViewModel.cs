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

            Position p;

            p = (await _Geocoder.GetPositionsForAddressAsync(Address)).FirstOrDefault();

            // The Android geocoder (the underlying implementation in Android itself) fails with some addresses unless they're rounded to the hundreds.
            // So, this deals with that edge case.
            if (p.Latitude == 0 && p.Longitude == 0 && AddressBeginsWithNumber(Address))
            {
                var roundedAddress = GetAddressWithRoundedStreetNumber(Address);

                p = (await _Geocoder.GetPositionsForAddressAsync(roundedAddress)).FirstOrDefault();
            }

            Position = p;
        }

        static bool AddressBeginsWithNumber(string address)
        {
            return !String.IsNullOrWhiteSpace(address) && Char.IsDigit(address.ToCharArray().First());
        }

        static string GetAddressWithRoundedStreetNumber(string address)
        {
            var endingIndex = GetEndingIndexOfNumericPortionOfAddress(address);

            if (endingIndex == 0)
                return address;

            int originalNumber = 0;
            int roundedNumber = 0;

            Int32.TryParse(address.Substring(0, endingIndex + 1), out originalNumber);

            if (originalNumber == 0)
                return address;

            roundedNumber = originalNumber.RoundToLowestHundreds();

            return address.Replace(originalNumber.ToString(), roundedNumber.ToString());
        }

        static int GetEndingIndexOfNumericPortionOfAddress(string address)
        {
            int endingIndex = 0;

            for (int i = 0; i < address.Length; i++)
            {
                if (Char.IsDigit(address[i]))
                    endingIndex = i;
                else
                    break;
            }

            return endingIndex;
        }
    }
}

