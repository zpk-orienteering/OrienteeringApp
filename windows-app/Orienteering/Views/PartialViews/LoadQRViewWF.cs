using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZXing;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Threading;
using DAL.Models;
using DAL;
using Orienteering.ViewModels;
using Orienteering.Helpers;

namespace Orienteering.View
{
    public partial class LoadQRViewWF : Form
    {
        private readonly CameraDevices camDevices;
        private Bitmap currentBitmapForDecoding;
        private readonly Thread decodingThread;
        private Result currentResult;
        private readonly Pen resultRectPen;
        private String currentDecodedString;
        private String latestDecodedString;
        private List<int> currentPlayerCPVisited;
        private Player currentPlayer;
        private ContestViewModel vm;

        private RouteModel routeModel;

        private PlayerDataParser playerDataParser = null;

        public delegate void AllPlayerDataLoaded(object sender);
        public event AllPlayerDataLoaded OnAllPlayerDataLoaded;

        public delegate void PlayerDataLoaded(object sender, Player proceedPlayer);
        public event PlayerDataLoaded OnPlayerDataLoaded;

        public delegate void CodeLoaded(object sender);
        public event CodeLoaded OnCodeLoaded;

        public LoadQRViewWF(ContestViewModel aVm)
        {
            InitializeComponent();
            vm = aVm;
            this.camDevices = new CameraDevices();
            this.decodingThread = new Thread(new ThreadStart(this.DecodeBarcode));
            this.decodingThread.Start();
            this.pictureBox1.Paint += new PaintEventHandler(this.pictureBox1_Paint);
            this.cmbDevice.SelectedIndexChanged += new EventHandler(this.cmbDevice_SelectedIndexChanged);
            this.resultRectPen = new Pen(Color.Green, 10f);
            currentPlayerCPVisited = new List<int>();

            routeModel = new RouteModel(DBHelper.ConnectionString);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.LoadDevicesToCombobox();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (e.Cancel)
                return;
            this.decodingThread.Abort();
            if (this.camDevices.Current == null)
                return;
            this.camDevices.Current.NewFrame -= new NewFrameEventHandler(this.Current_NewFrame);
            if (!this.camDevices.Current.IsRunning)
                return;
            this.camDevices.Current.SignalToStop();
        }

        private void LoadDevicesToCombobox()
        {
            this.cmbDevice.Items.Clear();
            for (int index = 0; index < this.camDevices.Devices.Count; ++index)
                this.cmbDevice.Items.Add((object)new Device()
                {
                    Index = index,
                    Name = this.camDevices.Devices[index].Name
                });
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (this.currentResult == null || this.currentResult.ResultPoints == null || this.currentResult.ResultPoints.Length <= 0)
                return;
            ResultPoint[] resultPoints = this.currentResult.ResultPoints;
            Rectangle rect = new Rectangle((int)resultPoints[0].X, (int)resultPoints[0].Y, 1, 1);
            foreach (ResultPoint resultPoint in resultPoints)
            {
                if ((double)resultPoint.X < (double)rect.Left)
                    rect = new Rectangle((int)resultPoint.X, rect.Y, rect.Width + rect.X - (int)resultPoint.X, rect.Height);
                if ((double)resultPoint.X > (double)rect.Right)
                    rect = new Rectangle(rect.X, rect.Y, rect.Width + (int)resultPoint.X - rect.X, rect.Height);
                if ((double)resultPoint.Y < (double)rect.Top)
                    rect = new Rectangle(rect.X, (int)resultPoint.Y, rect.Width, rect.Height + rect.Y - (int)resultPoint.Y);
                if ((double)resultPoint.Y > (double)rect.Bottom)
                    rect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height + (int)resultPoint.Y - rect.Y);
            }
            using (Graphics graphics = this.pictureBox1.CreateGraphics())
                graphics.DrawRectangle(this.resultRectPen, rect);
        }

        private void DecodeBarcode()
        {
            BarcodeReader barcodeReader = new BarcodeReader();
            while (true)
            {
                if (this.currentBitmapForDecoding != null)
                {
                    Result result = barcodeReader.Decode(this.currentBitmapForDecoding);
                    if (result != null)
                        this.Invoke((Delegate)new Action<Result>(this.ShowResult), new object[1]
                        {
                          (object) result
                        });
                    this.currentBitmapForDecoding.Dispose();
                    this.currentBitmapForDecoding = (Bitmap)null;
                }
                Thread.Sleep(200);
            }
        }

        private void ShowResult(Result result)
        {
            currentResult = result;
            currentDecodedString = result.Text;

            if ((!String.IsNullOrEmpty(result.Text)) && (!currentDecodedString.Equals(latestDecodedString)))
            {
                if (playerDataParser == null)
                {
                    try
                    {
                        playerDataParser = new PlayerDataParser();
                        playerDataParser.LoadInitialPlayerData(currentDecodedString);
                        this.statusLabel.Text = "Wczytano dane uczestnika o numerze startowym "
                            + playerDataParser.Identifier.ToString()
                            + "(" + playerDataParser.LastProceedSegment.ToString() + "/" + playerDataParser.SegmentsNumber.ToString() + ")";
                    }
                    catch (PlayerDataParseException pdp_ex)
                    {
                        this.statusLabel.Text = pdp_ex.Message;
                    }
                    catch (Exception ex)
                    {
                        this.statusLabel.Text = "wystąpił nieznany błąd";
                    }
                }
                else
                {
                    if (playerDataParser.LastProceedSegment < playerDataParser.SegmentsNumber - 1)
                    {
                        try
                        {
                            playerDataParser.LoadPlayerData(currentDecodedString);
                            this.statusLabel.Text = "Wczytano dane uczestnika o numerze startowym "
                            + playerDataParser.Identifier.ToString()
                            + "(" + playerDataParser.LastProceedSegment.ToString() + "/" + playerDataParser.SegmentsNumber.ToString() + ")";
                        }
                        catch (PlayerDataParseException pdp_ex)
                        {
                            this.statusLabel.Text = pdp_ex.Message;
                        }
                        catch (Exception ex)
                        {
                            this.statusLabel.Text = "wystąpił nieznany błąd";
                        }
                    }
                    else if (playerDataParser.LastProceedSegment == playerDataParser.SegmentsNumber - 1)
                    {
                        try
                        {
                            playerDataParser.LoadFinishPlayerData(currentDecodedString);
                            this.statusLabel.Text = "Wczytano dane uczestnika o numerze startowym "
                            + playerDataParser.Identifier.ToString()
                            + "(" + playerDataParser.LastProceedSegment.ToString() + "/" + playerDataParser.SegmentsNumber.ToString() + ")";
                        }
                        catch (PlayerDataParseException pdp_ex)
                        {
                            this.statusLabel.Text = pdp_ex.Message;
                        }
                        catch (Exception ex)
                        {
                            this.statusLabel.Text = "wystąpił nieznany błąd";
                        }
                    }
                    else
                    {
                        this.statusLabel.Text = "Niezgodna liczba segmentów";
                    }
                }

                if (playerDataParser.LastProceedSegment == playerDataParser.SegmentsNumber)
                {
                    ContestModel model = new ContestModel(DBHelper.ConnectionString);
                    playerDataParser.CalculateResults(model.GetContestControlPoints(vm.Contest).ToList());

                    Player proceedPlayer = vm.Players.Where(p => p.Identifier == playerDataParser.Identifier).FirstOrDefault();
                    proceedPlayer.CheetingFlag = playerDataParser.Cheater;
                    proceedPlayer.ElapsedTime = playerDataParser.ElapsedTime;
                    proceedPlayer.StartTime = playerDataParser.StartTime;
                    proceedPlayer.FinishTime = playerDataParser.EndTime;
                    proceedPlayer.Status = playerDataParser.Status;

                    OnPlayerDataLoaded(this, proceedPlayer);

                    playerDataParser = null;
                }

                latestDecodedString = result.Text;
                OnCodeLoaded(this);
            }

            foreach (Player p in vm.Players)
            {
                if (p.Status == RunStatus.InProgress || p.ElapsedTime == null)
                    return;
            }

            OnAllPlayerDataLoaded(this);
        }

        private void ShowFrame(Bitmap frame)
        {
            if (this.pictureBox1.Width < frame.Width)
                this.pictureBox1.Width = frame.Width;
            if (this.pictureBox1.Height < frame.Height)
                this.pictureBox1.Height = frame.Height;
            this.pictureBox1.Image = (Image)frame;
        }

        private void Current_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (this.IsDisposed)
                return;
            try
            {
                if (this.currentBitmapForDecoding == null)
                    this.currentBitmapForDecoding = (Bitmap)((Image)eventArgs.Frame).Clone();
                this.Invoke((Delegate)new Action<Bitmap>(this.ShowFrame), new object[1]
                {
                  ((Image) eventArgs.Frame).Clone()
                });
            }
            catch (ObjectDisposedException ex)
            {
            }
        }

        private void cmbDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.camDevices.Current != null)
            {
                this.camDevices.Current.NewFrame -= new NewFrameEventHandler(this.Current_NewFrame);
                if (this.camDevices.Current.IsRunning)
                    this.camDevices.Current.SignalToStop();
            }
            this.camDevices.SelectCamera(((Device)this.cmbDevice.SelectedItem).Index);
            this.camDevices.Current.NewFrame += new NewFrameEventHandler(this.Current_NewFrame);
            this.camDevices.Current.Start();
        }
    }

    internal struct Device
    {
        public int Index;
        public string Name;

        public override string ToString()
        {
            return this.Name;
        }
    }

    internal class CameraDevices
    {
        public FilterInfoCollection Devices { get; private set; }

        public VideoCaptureDevice Current { get; private set; }

        public CameraDevices()
        {
            this.Devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        }

        public void SelectCamera(int index)
        {
            if (index >= this.Devices.Count)
                throw new ArgumentOutOfRangeException("index");
            this.Current = new VideoCaptureDevice(this.Devices[index].MonikerString);
        }
    }

    /// <summary>
    /// Klasa reprezentująca zrzut danych z aplikacji mobilnej
    /// uczestnika biegu po jego zakończeniu
    /// </summary>
    internal class PlayerDataParser
    {
        private int identifier;

        public int Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        }

        private int cheater;

        public bool Cheater
        {
            get { return cheater == 0 ? false : true; }
            set { cheater = value ? 1 : 0; }
        }

        private DateTime startTime;

        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        private DateTime endTime;

        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        private TimeSpan elapsedTime;

        public TimeSpan ElapsedTime
        {
            get { return elapsedTime; }
            set { elapsedTime = value; }
        }

        private List<int> controlPoints = new List<int>();

        public List<int> ControlPoints
        {
            get { return controlPoints; }
            set { controlPoints = value; }
        }

        private RunStatus status;

        public RunStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        private int lastProceedSegment;

        public int LastProceedSegment
        {
            get { return lastProceedSegment; }
            set { lastProceedSegment = value; }
        }

        private int segmentsNumber;

        public int SegmentsNumber
        {
            get { return segmentsNumber; }
            set { segmentsNumber = value; }
        }

        public void CalculateResults(List<MMControlPointContest> aContestControlPoints)
        {
            aContestControlPoints.Sort((cp1, cp2) => { return cp1.Order.CompareTo(cp2.Order); });

            if (aContestControlPoints.Count() != ControlPoints.Count)
            {
                Status = RunStatus.WrongPath;
                return;
            }

            for (int i = 0; i < controlPoints.Count; i++)
            {
                if (controlPoints[i] != aContestControlPoints[i].Ident)
                {
                    Status = RunStatus.WrongPath;
                    return;
                }
            }

            Status = RunStatus.Correct;
        }

        public void LoadInitialPlayerData(string aData)
        {
            int hours;
            int minutes;
            int secounds;

            string[] elements = aData.Split('|');

            if (elements.Length < 5) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }

            string[] codeSegment = elements[0].Split('/');
            if (codeSegment.Length != 2) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }
            if (!int.TryParse(codeSegment[0], out lastProceedSegment)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }
            if (!int.TryParse(codeSegment[1], out segmentsNumber)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }

            if (!int.TryParse(elements[1], out identifier)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }
            if (!int.TryParse(elements[2], out cheater)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }
            if (!int.TryParse(elements[3].Substring(0, 2), out hours)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }
            if (!int.TryParse(elements[3].Substring(2, 2), out minutes)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }
            if (!int.TryParse(elements[3].Substring(4, 2), out secounds)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }

            startTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hours, minutes, secounds);

            for (int i = 4; i < (lastProceedSegment == segmentsNumber ? elements.Length - 2 : elements.Length - 1); i++)
            {
                string[] temp = elements[i].Split('-');
                if (temp.Length != 2) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }

                int tempControlPointID;
                if (!int.TryParse(temp[0], out tempControlPointID)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }
                controlPoints.Add(tempControlPointID);
            }

            if (lastProceedSegment == segmentsNumber)
            {
                if (!int.TryParse(elements[elements.Length - 2].Substring(0, 2), out hours)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }
                if (!int.TryParse(elements[elements.Length - 2].Substring(2, 2), out minutes)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }
                if (!int.TryParse(elements[elements.Length - 2].Substring(4, 2), out secounds)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }

                endTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hours, minutes, secounds);
                elapsedTime = endTime - startTime;
            }
        }

        public void LoadPlayerData(string aData)
        {
            string[] elements = aData.Split('|');

            string[] codeSegment = elements[0].Split('/');
            if (codeSegment.Length != 2) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }
            if (!int.TryParse(codeSegment[0], out lastProceedSegment)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }
            if (!int.TryParse(codeSegment[1], out segmentsNumber)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }

            for (int i = 1; i < elements.Length - 1; i++)
            {
                string[] temp = elements[i].Split('-');
                if (temp.Length != 2) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }

                int tempControlPointID;
                if (!int.TryParse(temp[0], out tempControlPointID)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }
                controlPoints.Add(tempControlPointID);
            }
        }

        public void LoadFinishPlayerData(string aData)
        {
            int hours;
            int minutes;
            int secounds;

            string[] elements = aData.Split('|');

            string[] codeSegment = elements[0].Split('/');
            if (codeSegment.Length != 2) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }
            if (!int.TryParse(codeSegment[0], out lastProceedSegment)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }
            if (!int.TryParse(codeSegment[1], out segmentsNumber)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }

            for (int i = 1; i < elements.Length - 2; i++)
            {
                string[] temp = elements[i].Split('-');
                if (temp.Length != 2) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }

                int tempControlPointID;
                if (!int.TryParse(temp[0], out tempControlPointID)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }
                controlPoints.Add(tempControlPointID);
            }

            if (lastProceedSegment == segmentsNumber)
            {
                if (!int.TryParse(elements[elements.Length - 2].Substring(0, 2), out hours)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }
                if (!int.TryParse(elements[elements.Length - 2].Substring(2, 2), out minutes)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }
                if (!int.TryParse(elements[elements.Length - 2].Substring(4, 2), out secounds)) { throw new PlayerDataParseException("Nieprawidłowy format danych"); }

                endTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hours, minutes, secounds);
                elapsedTime = endTime - startTime;
            }
        }
    }

    internal class PlayerDataParseException : Exception
    {
        public PlayerDataParseException() { }

        public PlayerDataParseException(string message) : base(message) { }
    }
}
