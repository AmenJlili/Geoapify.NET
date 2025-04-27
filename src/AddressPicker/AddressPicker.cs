using Geoapify.NET;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlueByte.Wpf.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Geoapify.Wpf"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Geoapify.Wpf;assembly=Geoapify.Wpf"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class AddressPicker : Control, INotifyPropertyChanged, IDisposable
    {
        static AddressPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AddressPicker), new FrameworkPropertyMetadata(typeof(AddressPicker)));
        }

        public AddressPicker()
        {

            this.PropertyChanged += AddressPicker_PropertyChanged;
            this.IsBusy = false;
        }

        public void Dispose()
        {
            this.PropertyChanged -= AddressPicker_PropertyChanged;
        }



        private async void AddressPicker_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Query))
            {

                if (Addresses != null && Addresses.Any(x => Query.Equals(x.Formatted)))
                    return;

                IsBusy = true;

                if (string.IsNullOrWhiteSpace(Query))
                    Addresses.Clear();

                var addresses = await AutocompleteHelper.GetAddressAsync(query, APIKey);

                if (addresses != null)
                {
                    var toBeRemovedAddresses = Addresses.Where(x => addresses.Select(j => j.Formatted).Contains(x.Formatted) == false).ToArray();
                    var toBeAddedAddresses = addresses.ToList().Where(x => Addresses.Select(j => j.Formatted).Contains(x.Formatted) == false).ToArray();


                    foreach (var toBeRemovedAddresse in toBeRemovedAddresses)
                        Addresses.Remove(toBeRemovedAddresse);

                    toBeAddedAddresses.ToList().ForEach(x => Addresses.Add(x));

                    NeedsOpen = true;

                    IsBusy = false;
                }



            }

        }

        // Dependency Property: APIKey
        public string APIKey
        {
            get { return (string)GetValue(APIKeyProperty); }
            set { SetValue(APIKeyProperty, value); }
        }

        public static readonly DependencyProperty APIKeyProperty =
            DependencyProperty.Register(nameof(APIKey), typeof(string), typeof(AddressPicker), new PropertyMetadata(string.Empty));

        // Dependency Property: SelectedAddress
        public Geoapify.NET.Address SelectedAddress
        {
            get { return (Geoapify.NET.Address)GetValue(SelectedAddressProperty); }
            set { SetValue(SelectedAddressProperty, value); }
        }

        public static readonly DependencyProperty SelectedAddressProperty =
            DependencyProperty.Register(nameof(SelectedAddress), typeof(Geoapify.NET.Address), typeof(AddressPicker), new PropertyMetadata(OnSelectedAddressChanged));

        private static void OnSelectedAddressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldValue = e.OldValue as Address;
            if (oldValue != null)
                oldValue.IsSelected = false;


            var NewValue = e.NewValue as Address;
            if (NewValue != null)
                NewValue.IsSelected = true;
        }

        private void ComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            var combobox = sender as ComboBox;

            if (e.Key == Key.Enter)
            {
                if (SelectedAddress != null && Addresses.Contains(SelectedAddress))
                {
                    NeedsOpen = false;
                }
                
            }
           

         

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var comboBox = GetTemplateChild("PART_ComboBox") as ComboBox;
            if (comboBox != null)
            {
                comboBox.KeyDown += ComboBox_KeyDown;
            }
        }

        // Regular Properties with Notification
        private bool needsOpen = true;
        public bool NeedsOpen
        {
            get => needsOpen;
            set => SetProperty(ref needsOpen, value, nameof(NeedsOpen));
        }

        private ObservableCollection<Geoapify.NET.Address> addresses = new ObservableCollection<Geoapify.NET.Address>();
        public ObservableCollection<Geoapify.NET.Address> Addresses
        {
            get => addresses;
            set => SetProperty(ref addresses, value, nameof(Addresses));
        }

        private string query;
        public string Query
        {
            get => query;
            set => SetProperty(ref query, value, nameof(Query));
        }

        private bool isBusy =false;
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value, nameof(IsBusy));
        }

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName ?? GetCallerName());
            return true;
        }

        private string GetCallerName([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            return propertyName;
        }
    }

}

namespace Geoapify.NET
{

    public class AutocompleteHelper
    {

        public static async Task<Address[]> GetAddressAsync(string address, string apiKey = "")
        {

            using (var httpClient = new HttpClient())
            {
                var url = $"https://api.geoapify.com/v1/geocode/autocomplete?&text={Uri.EscapeDataString(address)}&apiKey={Uri.EscapeDataString(apiKey)}";

                var response = await httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                var ret = JsonSerializer.Deserialize<Root>(content);

                return ret?.features?.Select(x => x.Address).ToArray() ?? new Address[0];
            }
        }


    }

    public class Datasource
    {
        public string sourcename { get; set; }
        public string attribution { get; set; }
        public string license { get; set; }
    }

    public class Feature
    {
        public string type { get; set; }
        public Geometry geometry { get; set; }
        [JsonPropertyName("properties")]
        public Address Address { get; set; }


    }

    public class Geometry
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class Parsed
    {
        public string housenumber { get; set; }
        public string street { get; set; }
        public string expected_type { get; set; }
    }

    public class Address : INotifyPropertyChanged
    {
        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        // All other properties are normal auto-properties (no notification)
        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; }

        [JsonPropertyName("housenumber")]
        public string HouseNumber { get; set; }

        [JsonPropertyName("street")]
        public string Street { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("county")]
        public string County { get; set; }

        [JsonPropertyName("datasource")]
        public Datasource Datasource { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("suburb")]
        public string Suburb { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("state_code")]
        public string StateCode { get; set; }

        [JsonPropertyName("formatted")]
        public string Formatted { get; set; }

        [JsonPropertyName("address_line1")]
        public string AddressLine1 { get; set; }

        [JsonPropertyName("address_line2")]
        public string AddressLine2 { get; set; }

        [JsonPropertyName("timezone")]
        public Timezone Timezone { get; set; }

        [JsonPropertyName("result_type")]
        public string ResultType { get; set; }

        [JsonPropertyName("rank")]
        public Rank Rank { get; set; }

        [JsonPropertyName("place_id")]
        public string PlaceId { get; set; }

        [JsonPropertyName("postcode")]
        public string PostCode { get; set; }

        public void Reset()
        {
            // Optional: you can add logic here if you want to reset fields
        }

        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(Formatted) ? Formatted : base.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } }

        public class Query
        {
            public string text { get; set; }
            public Parsed parsed { get; set; }
        }

        public class Rank
        {
            public double confidence { get; set; }
            public string match_type { get; set; }
        }

        public class Root
        {
            public string type { get; set; }
            public List<Feature> features { get; set; }
            public Query query { get; set; }
        }

        public class Timezone
        {
            public string name { get; set; }
            public string offset_STD { get; set; }
            public int offset_STD_seconds { get; set; }
            public string offset_DST { get; set; }
            public int offset_DST_seconds { get; set; }
            public string abbreviation_STD { get; set; }
            public string abbreviation_DST { get; set; }
        }


    
}
