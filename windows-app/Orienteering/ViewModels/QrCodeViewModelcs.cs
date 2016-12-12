using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Orienteering.Helpers.Commands;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using DAL;
using DAL.Models;
using Orienteering.Helpers;
using System.Windows;

namespace Orienteering.ViewModels
{
    /// <summary>
    /// View model dla widoku kodów QR
    /// zarówno dla listy kodów jak i dla nowego kodu.
    /// Klasa QrCodeViewModel jest singletonem,
    /// co oznacza, że w aplikacji może być utworzona tylko jedna jej instacja
    /// </summary>
    public class QrCodeViewModel : ViewModelBase
    {
        #region Singleton members

        private static QrCodeViewModel instance;

        private QrCodeViewModel()
        {
            model = new QRCodeModel(DBHelper.ConnectionString);
            _controlPoints = new ObservableCollection<ControlPoint>(model.GetAllControlPoints());

            ControlPointModes = new List<string> { "Punkt kontrolny", "Punkt startowy", "Punkt końcowy" };
            SelectedControlPointMode = "Punkt kontrolny";

            AddNewControlPoint = new RelayCommand(() =>
            {
                if (!String.IsNullOrEmpty(Text) && (!String.IsNullOrEmpty(Name)))
                {
                    int temp;

                    if (!int.TryParse(Text, out temp) && SelectedControlPointMode == "Punkt kontrolny")
                    {
                        MessageBox.Show("wartość pola 'Text' musi być liczbą całkowitą");
                    }
                    else
                    {
                        if (Text.Length < 20)
                        {
                            ControlPoint cp = new ControlPoint();

                            if (SelectedControlPointMode == "Punkt kontrolny") { cp.Text = @"CH/" + Text; }
                            else if (SelectedControlPointMode == "Punkt startowy") { cp.Text = @"ST/" + Text; }
                            else { cp.Text = @"FI/" + Text; }
                            cp.Name = Name;
                            cp.Info = Info;
                            cp.IsActive = true;

                            if (!model.IsAnotherCodeWithSameText(cp.Text))
                            {
                                ControlPoints.Add(cp);
                                model.AddNewControlPoint(cp);
                                if (SelectedControlPointMode == "Punkt kontrolny") RouteViewModel.GetInstance().AllControlPoints.Add(cp);
                                SelectedControlPoint = cp;

                                Text = "";
                                Name = "";
                                Info = "";

                                CodeView = null;
                            }
                            else
                            {
                                MessageBox.Show("w bazie znajduje się już punkt kontrolny o tej samej zakodowanej informacji");
                            }
                        }
                        else
                        {
                            MessageBox.Show("długość zakodowanej informacji nie może przekraczać 20 znaków");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("należy uzupełnić wymagane pola");
                }
            });
            CreateNewControlPoint = new RelayCommand(() =>
            {
                Text = "";
                Name = "";
                Info = "";
            });
            DeleteControlPoint = new RelayCommand(() =>
            {
                model.DeleteControlPoint(SelectedControlPoint);
                if (SelectedControlPoint.Text.StartsWith(@"CH/"))
                    RouteViewModel.GetInstance().AllControlPoints.Remove(
                        RouteViewModel.GetInstance().AllControlPoints.FirstOrDefault(cp => cp.ID == SelectedControlPoint.ID));
                ControlPoints.Remove(SelectedControlPoint);
                CodePreview = null;
            });
            SaveQrCodeToFile = new RelayCommand(() =>
            {
                if (SelectedControlPoint != null)
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.DefaultExt = ".png";
                    sfd.Filter = "Images (.png)|*.png";

                    if (sfd.ShowDialog() == true)
                    {
                        currentlyDisplayedCode.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            });
        }

        public static QrCodeViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new QrCodeViewModel();
            }
            return instance;
        }

        #endregion

        #region Private Fileds

        private Bitmap currentlyDisplayedCode;

        private QRCodeModel model;

        #endregion

        #region Fields and Properties

        private string _text;
        /// <summary>
        /// tekst kodowany w kodzie QR
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                generateQrCode(_text);
                OnPropertyChanged("Text");
            }
        }

        private string _name;
        /// <summary>
        /// nazwa nowo dodawanego kodu QR
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private string _info;
        /// <summary>
        /// dodatkowe informacje nt. nowo dodawanego kodu, opis
        /// </summary>
        public string Info
        {
            get { return _info; }
            set
            {
                _info = value;
                OnPropertyChanged("Info");
            }
        }

        private ImageSource _codeView;
        /// <summary>
        /// podgląd wygenerowanego kodu QR
        /// </summary>
        public ImageSource CodeView
        {
            get { return _codeView; }
            set
            {
                _codeView = value;
                OnPropertyChanged("CodeView");
            }
        }

        private ImageSource _codePreview;
        /// <summary>
        /// podgląd aktualnie wybranego z listy kodu QR
        /// </summary>
        public ImageSource CodePreview
        {
            get { return _codePreview; }
            set
            {
                _codePreview = value;
                OnPropertyChanged("CodePreview");
            }
        }

        private ControlPoint _selectedControlPoint;
        /// <summary>
        /// aktualnie zaznaczony na liscie punkt kontrolny
        /// </summary>
        public ControlPoint SelectedControlPoint
        {
            get { return _selectedControlPoint; }
            set
            {
                _selectedControlPoint = value;

                if (value != null)
                {
                    if (_selectedControlPoint.Text != null) { generateQrCodePreview(_selectedControlPoint.Text); }
                }

                OnPropertyChanged("SelectedControlPoint");
            }
        }

        private ObservableCollection<ControlPoint> _controlPoints;
        /// <summary>
        /// lista punktów kontrolnych i przyporządkowanych im kodów QR
        /// dostępna dla użytkownika
        /// </summary>
        public ObservableCollection<ControlPoint> ControlPoints
        {
            get { return _controlPoints; }
            set
            {
                _controlPoints = value;
            }
        }

        /// <summary>
        /// Lista dostępnych typów punktów kontrolnych
        /// </summary>
        public List<string> ControlPointModes { get; set; }

        /// <summary>
        /// Wybrany tryb punktu kontrolnego
        /// </summary>
        public string SelectedControlPointMode { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// komenda dodaje nowy punkt kontrolny do listy
        /// na podstawie danych wpisanych przez użytkownika
        /// w odpowiednich polach
        /// </summary>
        public ICommand AddNewControlPoint { get; private set; }
        /// <summary>
        /// komenda daje możliwość użytkownikowi wygenerowania nowego kodu QR
        /// </summary>
        public ICommand CreateNewControlPoint { get; private set; }
        /// <summary>
        /// komenda usuwa zaznaczony punkt kontrolny z listy
        /// </summary>
        public ICommand DeleteControlPoint { get; private set; }
        /// <summary>
        /// komenda zapisuje wygenerowany kod QR do pliku w formacie .png
        /// </summary>
        public ICommand SaveQrCodeToFile { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Metoda generująca kod QR na podstawie przekazanego tekstu
        /// </summary>
        /// <param name="aText">tekst na podstawie którego zostanie wygenerowany kod QR</param>
        private void generateQrCode(string aText)
        {
            if (aText == "")
            {
                CodeView = null;
                return;
            }

            IBarcodeWriter writer = new BarcodeWriter { Format = BarcodeFormat.QR_CODE };
            Bitmap bmp;
            var result = writer.Write(aText);
            bmp = new Bitmap(result);
            CodeView = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                                                                               bmp.GetHbitmap(),
                                                                                               IntPtr.Zero,
                                                                                               System.Windows.Int32Rect.Empty,
                                                                                               BitmapSizeOptions.FromWidthAndHeight(bmp.Width, bmp.Height));
        }

        /// <summary>
        /// Metoda generująca podgląd kodu QR dla aktualnie zaznaczonego na liście punktu kontrolnego 
        /// </summary>
        /// <param name="aText"></param>
        private void generateQrCodePreview(string aText)
        {
            if (aText == "")
            {
                CodeView = null;
                return;
            }

            IBarcodeWriter writer = new BarcodeWriter { Format = BarcodeFormat.QR_CODE };
            Bitmap bmp;
            var result = writer.Write(aText);
            bmp = new Bitmap(result);
            currentlyDisplayedCode = new Bitmap(result);
            CodePreview = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                                                                               bmp.GetHbitmap(),
                                                                                               IntPtr.Zero,
                                                                                               System.Windows.Int32Rect.Empty,
                                                                                               BitmapSizeOptions.FromWidthAndHeight(bmp.Width, bmp.Height));
        }

        #endregion
    }
}
