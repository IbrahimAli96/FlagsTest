using Caliburn.Micro;
using FlagsTest.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace FlagsTest.ViewModels
{
    public class MainViewModel : Screen
    {
        private BindableCollection<string> _filteredCities = new BindableCollection<string>();

        public BindableCollection<string> FilteredCities
        {
            get { return _filteredCities; }
            set
            {
                _filteredCities = value;
                NotifyOfPropertyChange(() => FilteredCities);
            }
        }

        private string _selectedCity;

        public string SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                _selectedCity = value;
                NotifyOfPropertyChange(() => SelectedCity);
                if(SelectedCity != null)
                {
                    GetWeatherDataAsync(SelectedCity);
                }
            }
        }

        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }
            set 
            { 
                _searchText = value;
                FilterCities();
            }
        }

        private WeatherModel _weatherModel;

        public WeatherModel WeatherModel
        {
            get { return _weatherModel; }
            set 
            {
                _weatherModel = value;
                NotifyOfPropertyChange(() => WeatherModel);
                NotifyOfPropertyChange(() => Temperature);
                NotifyOfPropertyChange(() => WeatherDescription);
                NotifyOfPropertyChange(() => WeatherIcon);
            }
        }

        public string Temperature
        {
            get
            {
                if (WeatherModel?.Current != null)
                {
                    return WeatherModel?.Current?.Temperature + "°C";
                }
                else
                {
                    return string.Empty;
                }                
            }
        }

        public string WeatherDescription
        {
            get 
            {
                if(WeatherModel?.Current != null)
                {
                    return string.Join(", ", WeatherModel?.Current?.Weather_Descriptions) ?? string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public BitmapImage WeatherIcon
        {
            get
            {
                if (WeatherModel?.Current != null)
                {
                    if (WeatherModel.Current.Weather_Icons != null)
                    {
                        return new BitmapImage(new System.Uri(WeatherModel.Current.Weather_Icons[0], UriKind.RelativeOrAbsolute)) ?? null;

                    }
                }

                return null;
            }
        }

        private string _currentDateTime;

        public string CurrentDateTime
        {
            get { return _currentDateTime; }
            set
            {
                if (_currentDateTime != value)
                {
                    _currentDateTime = value;
                    NotifyOfPropertyChange(() => CurrentDateTime);
                }
            }
        }

        private HttpClient _httpClient { get; set; }
        private List<string> _allCities = new List<string>();
        private DispatcherTimer _timer;

        //Update Here
        private string _citiesApiKey = "Ibrahim.ali96";
        private string _weatherApiKey = "dc56193aa155358c940caa53ca21fb72";

        public MainViewModel()
        {
            try
            {
                _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            GetLocationDataAsync();

            CurrentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CurrentDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void FilterCities()
        {
            if (_allCities.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    FilteredCities = new BindableCollection<string>(_allCities.Where(x => x.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
                }
                else
                {
                    FilteredCities = new BindableCollection<string>(_allCities);
                }
            }
        }

        private async void GetLocationDataAsync()
        {
            try
            {
                string uri = "http://api.geonames.org/searchJSON?name=&featureClass=P&maxRows=1000&orderby=population&username=" + _citiesApiKey;

                using (HttpResponseMessage response = await _httpClient.GetAsync(uri))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();

                        JObject data = JObject.Parse(json);

                        JArray cities = (JArray)data["geonames"];

                        foreach (var city in cities)
                        {
                            string cityName = (string)city["name"];
                            _allCities.Add(cityName);
                        }

                        _allCities = _allCities.OrderBy(x => x).ToList();

                        FilteredCities = new BindableCollection<string>(_allCities);
                    }
                    else
                    {
                        MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private async void GetWeatherDataAsync(string location)
        {
            try
            {
                string uri = "http://api.weatherstack.com/current?access_key=" + _weatherApiKey + "&query=" + location;

                using (HttpResponseMessage response = await _httpClient.GetAsync(uri))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        WeatherModel = await response.Content.ReadAsAsync<WeatherModel>();
                    }
                    else
                    {
                        MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
