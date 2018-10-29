using System;
using System.Linq;
using System.Windows.Media;
using System.Device.Location;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TakeMeThere
{

    public class ViewModel : INotifyPropertyChanged
    {
        
        public  string DefaultColor_Hex = "99A4C400";
        public  string SelectedColor_Hex = "99FA6800";
        public  string TargetColor_Hex = "99D80073";
        
        /*
        public string DefaultColor_Hex = "FFA4C400";
        public string SelectedColor_Hex = "FFFA6800";
        public string TargetColor_Hex = "FFD80073";
        */

        //AppDB DB = new AppDB();
        //ViewModel PushPinViewModel = new ViewModel();

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ObservableCollection<PushPinModel> _pushPins;
        public ObservableCollection<PushPinModel> PushPins
        {
            get { return _pushPins; }
            set
            {
                if (_pushPins != value)
                {
                    _pushPins = value;
                    OnPropertyChanged("PushPins");
                }
            }
        }

        public ViewModel()
        {
            this.PushPins = new ObservableCollection<PushPinModel>();
        }

        public PushPinModel GetTarget()
        {
            var items = from item in this.PushPins
                        where item.Target == true
                        select item;

            if (items.Count() != 0)
            {
                return items.First();
            }
            else
            {
                /*
                PushPinModel item = new PushPinModel
                {
                    Name = "---",
                    Location = new GeoCoordinate()
                    
                };
                 */

                return null;
            }
        }
        public PushPinModel GetSelcted()
        {
            var items = from item in this.PushPins
                        where item.Selected == true
                        select item;

            if (items.Count() != 0)
            {
                return items.First();
            }
            else
            {
                return null;
            }
        }
        public void SetSelected(PushPinModel item)
        {
            this.ClearSelected();
            if (item == null)
                return;
            item.Selected = true;
            item.Color = new SolidColorBrush(Utility.GetColorFromHexString(SelectedColor_Hex));
            item.ListColor = new SolidColorBrush(Utility.ConvertToFullOpacityColor(item.Color.Color));
        }
        public void SetTarget(PushPinModel item)
        {
            if (item == null)
                return;
            this.ClearTarget();
            item.Target = true;
            item.Color = new SolidColorBrush(Utility.GetColorFromHexString(TargetColor_Hex));
        }
        public void ClearTarget()
        {
            var prev_targetPin = this.GetTarget();
            if (prev_targetPin != null)
            {
                prev_targetPin.Target = false;
                prev_targetPin.Color = new SolidColorBrush(Utility.GetColorFromHexString(DefaultColor_Hex));
                prev_targetPin.ListColor = new SolidColorBrush(Utility.ConvertToFullOpacityColor(prev_targetPin.Color.Color));
            }
        }
        public void ClearSelected()
        {
            var prev_selectedPin = this.GetSelcted();
            if (prev_selectedPin != null)
            {
                prev_selectedPin.Selected = false;
                if (prev_selectedPin.Target == true)
                {
                    prev_selectedPin.Color = new SolidColorBrush(Utility.GetColorFromHexString(TargetColor_Hex));
                    prev_selectedPin.ListColor = new SolidColorBrush(Utility.ConvertToFullOpacityColor(prev_selectedPin.Color.Color));
                }
                else
                {
                    prev_selectedPin.Color = new SolidColorBrush(Utility.GetColorFromHexString(DefaultColor_Hex));
                    prev_selectedPin.ListColor = new SolidColorBrush(Utility.ConvertToFullOpacityColor(prev_selectedPin.Color.Color));
                }
            }
        }
        public PushPinModel GetPinByLocation(GeoCoordinate location)
        {
            var items = from item in this.PushPins
                        where item.Location==location
                        select item;

            if (items.Count() != 0)
            {
                return items.First();
            }
            else
            {
                return null;
            }
        }

        public PushPinModel GetPinByTimeStamp(DateTime timeStamp)
        {
            var items = from item in this.PushPins
                        where item.CreateDate == timeStamp
                        select item;

            if (items.Count() != 0)
            {
                return items.First();
            }
            else
            {
                return null;
            }
        }


    }


    public class PushPinModel : INotifyPropertyChanged
    {
        //public PushPinModel() { ; }
        private GeoCoordinate _location;
        private string _name;
        private System.Windows.Visibility _visibility;
        private bool _isenabled;
        private bool _target;
        private bool _selected;
        private SolidColorBrush _color;
        private string _note;
        private double _listItemOpacity;
        private string _distanceString;
        private string _etaString;
        private DateTime _createDate;
        private SolidColorBrush _listColor;
        private string _address;
        private string _phoneNum;



        public double Distance{get;set;}//ソーティング用。
        public string ArrowURI { get; set; }
        public string CircleURI { get; set; }


        public PushPinModel()
        {
            this.Location = new GeoCoordinate();
            this.PhoneNum = "";
            this.Address = "";
            this.Name = "";
            
        }

        public string PhoneNum
        {
            get
            {
                return _phoneNum;
            }
            set
            {
                if (_phoneNum != value)
                {
                    _phoneNum = value;
                    //OnPropertyChanged("PhoneNum");
                }
            }
        }

        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                if (_address != value)
                {
                    _address = value;
                    //OnPropertyChanged("Address");
                }
            }
        }

        public SolidColorBrush ListColor
        {
            get
            {
                return _listColor;
            }
            set
            {
                if (_listColor != value)
                {
                    _listColor = value;
                    OnPropertyChanged("ListColor");
                }
            }
        }

        public DateTime CreateDate
        {
            get
            {
                return _createDate;
            }
            set
            {
                if (_createDate != value)
                {
                    _createDate = value;
                    //OnPropertyChanged("CreateDate");
                }
            }
        }

        public double ListItemOpacity
        {
            get
            {
                return _listItemOpacity;
            }
            set
            {
                if (_listItemOpacity != value)
                {
                    _listItemOpacity = value;
                    //OnPropertyChanged("ListItemOpacity");
                }
            }
        }
        public string DistanceString
        {
            get
            {
                return _distanceString;
            }
            set
            {
                if (_distanceString != value)
                {
                    _distanceString = value;
                    OnPropertyChanged("DistanceString");
                }
            }
        }
        public string ETAString
        {
            get
            {
                return _etaString;
            }
            set
            {
                if (_etaString != value)
                {
                    _etaString = value;
                    OnPropertyChanged("ETAString");
                }
            }
        }

        public string Note
        {
            get
            {
                return _note;
            }
            set
            {
                if (_note != value)
                {
                    _note = value;
                    //OnPropertyChanged("Note");
                }
            }
        }

        public SolidColorBrush Color
        {
            get
            {
                return _color;
            }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged("Color");
                }
            }
        }

        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    OnPropertyChanged("Selected");
                }
            }
        }

        public bool Target
        {
            get
            {
                return _target;
            }
            set
            {
                if (_target != value)
                {
                    _target = value;
                    OnPropertyChanged("Target");
                }
            }
        }

        public bool IsEnabled
        {
            get 
            {
                return _isenabled;
            }
            set
            {
                if (_isenabled != value)
                {
                    _isenabled = value;
                    //OnPropertyChanged("IsEnable");
                }
            }
        }

        public System.Windows.Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                if (_visibility != value)
                {
                    _visibility = value;
                    //OnPropertyChanged("Visibility");
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public GeoCoordinate Location
        {
            get { return _location; }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    OnPropertyChanged("Location");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }



}
