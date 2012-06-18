using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using PokerHelper;

namespace WpfApplication1
{
    //@!mov todo:
    // deep refactor
    // validity show

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal MainWindowPr Presenter { get; set; }

        //
        internal const int ActivationTimeout = 50; //ms
        internal const int enemyPlayerCount_tb_SelectTime = 300; //ms

        //
        private System.Timers.Timer _enemyPlayerCount_tb_TextChanged_Timer;
        private Control _lastFocusedElement;
        private int _ignore_cards_tb_TextChanged = 0;


        public MainWindow()
        {
            InitializeComponent();

            //
            Loaded += MainWindow_Loaded;
            KeyDown += MainWindow_KeyDown;
            Closed += MainWindow_Closed;
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;

            //
            topMost_ckb.Checked += topMost_ckb_Checked;
            topMost_ckb.Unchecked += topMost_ckb_Unchecked;

            parallelismLevel_tb.KeyDown += parallelismLevel_tb_KeyDown;

            simulatedGamesCount_tb.KeyDown += simulatedGamesCount_tb_KeyDown;

            timeLimit_tb.KeyDown += timeLimit_tb_KeyDown;

            runSimulation_btn.Click += runSimulation_btn_Click;

            enemyPlayerCount_tb.KeyDown += enemyPlayerCount_tb_KeyDown;
            enemyPlayerCount_tb.GotKeyboardFocus += enemyPlayerCount_tb_GotKeyboardFocus;
            enemyPlayerCount_tb.TextChanged += enemyPlayerCount_tb_TextChanged;

            _enemyPlayerCount_tb_TextChanged_Timer = new System.Timers.Timer(enemyPlayerCount_tb_SelectTime);
            _enemyPlayerCount_tb_TextChanged_Timer.Elapsed += enemyPlayerCount_tb_TextChanged_Timer_Elapsed;

            cards_tb.TextChanged += cards_tb_TextChanged;
            cards_tb.KeyDown += cards_tb_KeyDown;
            cards_tb.PreviewKeyDown += cards_tb_PreviewKeyDown;
        }


        #region MainView Events

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Presenter.View_Loaded();

            InitialPreparations();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            Presenter.View_KeyDown(e);
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Presenter.View_Closed();
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Presenter.View_Dispatcher_UnhandledException(e);
        }

        #endregion


        #region Events

        private void cards_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_ignore_cards_tb_TextChanged > 0)
            {
                return;
            }

            Presenter.View_CardsInputChanged(cards_tb.Text);
        }

        private void cards_tb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool handled;
            Presenter.View_CardsActivity(e, out handled);
            if (handled)
            {
                e.Handled = true;
            }
        }

        private void enemyPlayerCount_tb_TextChanged_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _enemyPlayerCount_tb_TextChanged_Timer.Stop();

            Dispatcher.Invoke(new Action(
                () =>
                {
                    if (enemyPlayerCount_tb.IsFocused
                        && Presenter.EnemyPlayerCount_IsValid)
                    {
                        enemyPlayerCount_tb.SelectAll();
                        enemyPlayerCount_tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    }
                }));
        }

        private void timeLimit_tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (Presenter.KeyManager.IsDefaultStartSimulationKey(e))
            {
                Presenter.StartSimulation();
            }
        }

        private void enemyPlayerCount_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            _enemyPlayerCount_tb_TextChanged_Timer.Stop();
            _enemyPlayerCount_tb_TextChanged_Timer.Start();
        }

        private void enemyPlayerCount_tb_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            enemyPlayerCount_tb.SelectAll();
        }

        private void runSimulation_btn_Click(object sender, RoutedEventArgs e)
        {
            Presenter.StartSimulation();
        }

        private void cards_tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (Presenter.KeyManager.IsDefaultStartSimulationKey(e))
            {
                Presenter.StartSimulation();
            }
        }

        private void simulatedGamesCount_tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (Presenter.KeyManager.IsDefaultStartSimulationKey(e))
            {
                Presenter.StartSimulation();
            }
        }

        private void parallelismLevel_tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (Presenter.KeyManager.IsDefaultStartSimulationKey(e))
            {
                Presenter.StartSimulation();
            }
        }

        private void enemyPlayerCount_tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (Presenter.KeyManager.IsDefaultStartSimulationKey(e))
            {
                Presenter.StartSimulation();
            }
        }

        private void topMost_ckb_Unchecked(object sender, RoutedEventArgs e)
        {
            Topmost = false;
        }

        private void topMost_ckb_Checked(object sender, RoutedEventArgs e)
        {
            Topmost = true;
        }

        #endregion


        #region From Presenter

        internal void Pr_ParallelismLevel_Changed()
        {
            parallelismLevel_tb.Text = Presenter.ParallelismLevel_Str;
        }

        internal void Pr_SimulatedGamesCount_Changed()
        {
            simulatedGamesCount_tb.Text = Presenter.SimulatedGamesCount_Str;
        }

        internal void Pr_EnemyPlayerCount_Changed()
        {
            enemyPlayerCount_tb.Text = Presenter.EnemyPlayerCount_Str;
        }

        internal void Pr_CalculationTimeLimitEnabled_Changed()
        {
            timeLimit_ckb.IsChecked = Presenter.CalculationTimeLimitEnabled;
        }

        internal void Pr_CalculationTimeLimit_Changed()
        {
            timeLimit_tb.Text = Presenter.CalculationTimeLimit_Str;
        }

        internal void Pr_SimulationStarted()
        {
            _lastFocusedElement = Keyboard.FocusedElement as Control;

            mainGrid.IsEnabled = false;
            waiter.Visibility = Visibility.Visible;

            progress_bar.Visibility = Visibility.Visible;
            progress_bar.Value = 0;

            progress_bar.Minimum = 0;
            progress_bar.Maximum = Presenter.CalculationParameters.GameNumber;
        }

        internal void Pr_SimulationFinished()
        {
            mainGrid.IsEnabled = true;
            waiter.Visibility = Visibility.Hidden;
            progress_bar.Visibility = Visibility.Collapsed;

            if (_lastFocusedElement != null)
            {
                _lastFocusedElement.Focus();
                _lastFocusedElement = null;
            }

            switch (Presenter.CalculationResult)
            {
                case CalculationResult.Ok:
                    ShowCalculatedStatistic_Ok();
                    break;

                case CalculationResult.Cancelled:
                    result1_tb.Text = "Cancelled.";
                    result2_tb.Text = string.Empty;
                    result3_tb.Text = string.Empty;
                    break;

                case CalculationResult.Failed:
                    result1_tb.Text = "Error occured:" + Environment.NewLine + Presenter.CalculationError.ToString();
                    result2_tb.Text = string.Empty;
                    result3_tb.Text = string.Empty;
                    break;

                default:
                    throw Utility.GetUnknownEnumValueException(Presenter.CalculationResult);
            }
        }

        internal void Pr_ParametersAreNotValid(Exception ex)
        {
            result1_tb.Text = "Parameters are not valid:" + Environment.NewLine + ex.ToString();
            result2_tb.Text = string.Empty;
            result3_tb.Text = string.Empty;
        }

        internal void Pr_SetProgress(int value)
        {
            progress_bar.Value = value;
        }

        internal void Pr_CorrectCardsString(string val)
        {
            _ignore_cards_tb_TextChanged++;
            try
            {
                int caretIndx = cards_tb.CaretIndex;
                cards_tb.Text = val;
                cards_tb.CaretIndex = caretIndx;
            }
            finally
            {
                _ignore_cards_tb_TextChanged--;
            }
        }

        internal void Pr_CardsChanged()
        {
            _ignore_cards_tb_TextChanged++;
            try
            {
                cards_tb.Text = Presenter.Cards_Str;

                if (Presenter.Cards_IsValid)
                {
                    DisplayCards(Presenter.Cards);
                }
                else
                {
                    DisplayCards(null);
                }
            }
            finally
            {
                _ignore_cards_tb_TextChanged--;
            }
        }

        #endregion


        private void InitialPreparations()
        {
            cards_tb.Focus();

            appVersion_lbl.Content = MainWindowPr.AppVersionFormatString.FormatStr(Presenter.AppVersion.ToString());
            libVersion_lbl.Content = MainWindowPr.LibVersionFormatString.FormatStr(Presenter.CoreLibVersion.ToString());
        }

        private void ShowCalculatedStatistic_Ok()
        {
            Statistic stat = Presenter.CalculatedStatistic;
            CalculationParameters calcParameters = Presenter.CalculationParameters;
            TimeSpan calcTime = Presenter.CalculationTime;

            string s = null;

            double win_percent = 100 * stat.Win / (double)stat.GameNumber;

            s += "Wins:".PadRight(8) + "{0:0.####}%".FormatStr(win_percent);
            s += Environment.NewLine;
            s += "Draws:".PadRight(8) + "{0:0.####}%".FormatStr(100 * stat.Draw / (double)stat.GameNumber);
            s += Environment.NewLine;
            s += "Loses:".PadRight(8) + "{0:0.####}%".FormatStr(100 * stat.Lose / (double)stat.GameNumber);

            if (calcParameters.Flop1 == null && PokerStatisticCalc.HoleCardsStatistic.ContainsKey(calcParameters.EnemyPlayersCount))
            {
                s += Environment.NewLine;
                s += Environment.NewLine;

                double min = PokerStatisticCalc.HoleCardsStatistic[calcParameters.EnemyPlayersCount].Item1;
                double max = PokerStatisticCalc.HoleCardsStatistic[calcParameters.EnemyPlayersCount].Item2;
                win_percent = Math.Max(win_percent, min);
                win_percent = Math.Min(win_percent, max);
                double d = 100 * (win_percent - min) / (max - min);

                s += "Hole cards value: " + "{0:0.####}%".FormatStr(d);
            }

            s += Environment.NewLine;
            s += Environment.NewLine;
            s += "Games:".PadRight(8) + stat.GameNumber.ToString();
            s += Environment.NewLine;
            s += "Enemy:".PadRight(8) + calcParameters.EnemyPlayersCount.ToString();
            s += Environment.NewLine;
            s += "Simulation time: {0}".FormatStr(calcTime);
            s += Environment.NewLine;

            result1_tb.Text = s;

            s = null;
            s += "Player hands histogram:";
            s += Environment.NewLine;
            s += Environment.NewLine;

            Func<int, int, string> formatWinDrawLose =
                (int value, int gameCount) =>
                {
                    return (value < 1 ? "-" : (Math.Round(value * 100 / (double)gameCount, 4).ToString() + "%")).PadRight(8);
                };

            foreach (Statistic.HandStatistic hs in stat.PlayerHandsStat.OrderBy(x => HandTypeConverter.GetHandValue(x.HandType)))
            {
                if (hs.Win == 0 && hs.Draw == 0 && hs.Lose == 0)
                {
                    continue;
                }

                s += "{0} {1}  |  {2}   {3}   {4}".FormatStr(
                    (HandTypeConverter.GetName(hs.HandType) + ":").PadRight(20),
                    formatWinDrawLose(hs.Win + hs.Draw + hs.Lose, stat.GameNumber),
                    formatWinDrawLose(hs.Win, stat.GameNumber),
                    formatWinDrawLose(hs.Draw, stat.GameNumber),
                    formatWinDrawLose(hs.Lose, stat.GameNumber)
                    );
                s += Environment.NewLine;
            }

            result2_tb.Text = s;

            s = null;
            s += "Enemy hands histogram:";
            s += Environment.NewLine;
            s += Environment.NewLine;
            s += "Wins: " + "{0:0.####}%".FormatStr(100 * stat.EnemyHandsStat.Select(x => x.Win).Sum() / (double)stat.GameNumber);
            s += "    ";
            s += "Draws: " + "{0:0.####}%".FormatStr(100 * stat.EnemyHandsStat.Select(x => x.Draw).Sum() / (double)stat.GameNumber);
            s += "    ";
            s += "Loses: " + "{0:0.####}%".FormatStr(100 * stat.EnemyHandsStat.Select(x => x.Lose).Sum() / (double)stat.GameNumber);
            s += Environment.NewLine;
            s += Environment.NewLine;

            foreach (Statistic.HandStatistic hs in stat.EnemyHandsStat.OrderBy(x => HandTypeConverter.GetHandValue(x.HandType)))
            {
                if (hs.Win == 0)
                {
                    continue;
                }

                s += "{0} {1}  |  {2}   {3}   {4}".FormatStr(
                    (HandTypeConverter.GetName(hs.HandType) + ":").PadRight(20),
                    formatWinDrawLose(hs.Win + hs.Draw + hs.Lose, stat.GameNumber),
                    formatWinDrawLose(hs.Win, stat.GameNumber),
                    formatWinDrawLose(hs.Draw, stat.GameNumber),
                    formatWinDrawLose(hs.Lose, stat.GameNumber)
                    );
                s += Environment.NewLine;
            }

            result3_tb.Text = s;
        }

        private void DisplayCards(Card[] cards)
        {
            var cardImgCtrls = new Image[]
            {
                card1_img,
                card2_img,
                card3_img,
                card4_img,
                card5_img,
                card6_img,
                card7_img,
            };

            for (int i = 0; i < Math.Max(cards.Length, cardImgCtrls.Length); i++)
            {
                Image cardImgCtrl = Utility.TryGetByIndx(cardImgCtrls, i, null);
                Card card = Utility.TryGetByIndx(cards, i, null);

                if (cardImgCtrl == null)
                {
                    continue;
                }

                if (card != null)
                {
                    cardImgCtrl.Source = Presenter.CardImages[card];
                }
                else
                {
                    cardImgCtrl.Source = null;
                }
            }
        }
    }

    class MainWindowPr
    {
        internal ApplicationCm Application { get; set; }

        //
        private MainWindow MainView;

        //
        internal readonly Logger Logger = new Logger();

        //
        internal const int ClipboardMonitorSleep = 100; //ms
        internal const int FileMonitorSleep = 600; //ms
        internal const int ProgressWatcherSleep = 100; //ms

        internal const string AppVersionFormatString = "app v{0}";
        internal const string LibVersionFormatString = "lib v{0}";

        internal const char ExternalInput_StartSimulation = 'q';
        internal const char ExternalInput_StopSimulation = 'w';
        internal const char ExternalInput_GetCardsFromLogFile = 'a';

        internal const string LogFilePath = @"C:\Program Files\PokerStars\PokerStars.log.0";

        //
        internal string ParallelismLevel_Str { get; private set; }
        internal bool ParallelismLevel_IsValid { get; private set; }
        internal int ParallelismLevel { get; private set; }

        internal string SimulatedGamesCount_Str { get; private set; }
        internal bool SimulatedGamesCount_IsValid { get; private set; }
        internal int SimulatedGamesCount { get; private set; }

        internal string EnemyPlayerCount_Str { get; private set; }
        internal bool EnemyPlayerCount_IsValid { get; private set; }
        internal int EnemyPlayerCount { get; private set; }

        internal bool CalculationTimeLimitEnabled { get; private set; }

        internal string CalculationTimeLimit_Str { get; private set; }
        internal bool CalculationTimeLimit_IsValid { get; private set; }
        internal double CalculationTimeLimit { get; private set; }

        internal string Cards_Str { get; private set; }
        internal bool Cards_IsValid { get; private set; }
        internal Card[] Cards { get; private set; }

        internal bool IsWatchCardsInClipboard { get; private set; }
        internal bool IsWatchGlobalKeys { get; private set; }
        internal bool IsCalcLogCardsImmediately { get; private set; }

        //
        internal Version AppVersion { get; private set; }
        internal Version CoreLibVersion { get; private set; }

        //
        internal MainWindowKeyManager KeyManager { get; private set; }
        internal Dictionary<Card, BitmapImage> CardImages { get; private set; }

        //
        internal bool Calculating { get; private set; }
        internal CalculationResult CalculationResult { get; private set; }
        internal Exception CalculationError { get; private set; }
        internal Statistic CalculatedStatistic { get; private set; }
        internal TimeSpan CalculationTime { get; private set; }
        internal CalculationParameters CalculationParameters { get; private set; }

        //
        private KeyInterceptor _globalKeyHooker;
        private Random _rnd = new Random(Guid.NewGuid().GetHashCode());

        private CancellationTokenSource _cancelTokenSource;
        private Task _calculationTask;

        private long? _logFileSize;
        private string _clipboardTxt;


        public void Start()
        {
            Init();

            MainView.ShowDialog();
        }

        private void Init()
        {
            MainView = new MainWindow();
            MainView.Presenter = this;
        }


        #region From View


        #region Main

        internal void View_Loaded()
        {
            SetVersions();

            KeyManager = new MainWindowKeyManager();

            SetParallelismLevel(ParamHelper.GetParallelismLevel());
            SetSimulatedGamesCount(ParamHelper.GetSimulatedGameCount());
            SetEnemyPlayerCount(ParamHelper.GetEnemyPlayersCount());
            SetCalculationTimeLimitEnabled(ParamHelper.GetCalculationTimeLimitEnabled());
            SetCalculationTimeLimit(ParamHelper.GetCalculationTimeLimit());

            CardImages = BuildCardImages();

            Cards = GetRandomHand(_rnd);

            StartMonitor_LogFile();
            StartMonitor_Clipboard();
            SetGlobalKeyHooker();
        }

        internal void View_KeyDown(KeyEventArgs e)
        {
            CancellationTokenSource cts = _cancelTokenSource;
            if (Calculating && e.Key == Key.Escape)
            {
                cts.Cancel();
            }
        }

        internal void View_Closed()
        {
            DisposeGlobalKeyHooker();
        }

        internal void View_Dispatcher_UnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            DisposeGlobalKeyHooker();
        }

        #endregion


        internal void View_CardsActivity(KeyEventArgs e, out bool handled)
        {
            handled = false;

            bool shiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (shiftDown && (e.Key == Key.Delete || e.Key == Key.Back))
            {
                SetCards(new Card[0]);
            }

            if (shiftDown && e.Key == Key.Enter)
            {
                SetCards(GetRandomHand(_rnd));
                handled = true;
            }
        }

        internal void View_CardsInputChanged(string val)
        {
            string correctedVal = null;

            if (!val.IsNullOrEmptyStr())
            {
                for (int i = 0; i < val.Length; i++)
                {
                    correctedVal += (i % 2 == 0) ? char.ToUpper(val[i]) : char.ToLower(val[i]);
                }
            }

            Cards_Str = correctedVal;
            MainView.Pr_CorrectCardsString(correctedVal);

            try
            {
                Card[] cards = ParseCards(correctedVal);
                Cards = cards;
                Cards_IsValid = true;
            }
            catch (Exception ex)
            {
                Cards_IsValid = false;

                Logger.LogException(ex);
            }

            MainView.Pr_CardsChanged();
        }

        #endregion

        //-----------------------------------------------------------

        private void SetVersions()
        {
            AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
            CoreLibVersion = Assembly.GetAssembly(typeof(PokerStatisticCalc)).GetName().Version;
        }

        private static Card[] GetAllCards()
        {
            CardSuit[] cardSuits = Enum.GetValues(typeof(CardSuit))
                .Cast<CardSuit>()
                .ToArray();

            CardValue[] cardValues = Enum.GetValues(typeof(CardValue))
                .Cast<CardValue>()
                .ToArray();

            var cards = new List<Card>();
            foreach (CardSuit cardSuit in cardSuits)
            {
                foreach (CardValue cardValue in cardValues)
                {
                    var card = new Card(cardSuit, cardValue);
                    cards.Add(card);
                }
            }

            return cards.ToArray();
        }

        private static Card[] GetRandomHand(Random rnd)
        {
            Card[] cards = GetAllCards();
            Shuffle(cards, rnd);

            return cards.Take(rnd.Next(2, 8)).ToArray();
        }

        private static void Shuffle(Card[] cards, Random rnd)
        {
            var rnds = new int[cards.Length];

            for (int i = cards.Length - 1; i > 0; i--)
            {
                rnds[i] = rnd.Next(0, i + 1);
            }

            for (int i = cards.Length - 1; i > 0; i--)
            {
                int rndIndx = rnds[i];

                if (rndIndx != i)
                {
                    Card tmp = cards[i];
                    cards[i] = cards[rndIndx];
                    cards[rndIndx] = tmp;
                }
            }
        }

        private void SetGlobalKeyHooker()
        {
            _globalKeyHooker = new KeyInterceptor();
            _globalKeyHooker.SetHook(GlobalKeyboardHook);
        }

        private void DisposeGlobalKeyHooker()
        {
            if (_globalKeyHooker == null)
            {
                return;
            }

            _globalKeyHooker.RemoveHook();
            _globalKeyHooker = null;
        }

        private Tuple<Card, Card> ParseLast2CardsFromLogFile(string s)
        {
            if (s.IsNullOrEmptyStr())
            {
                return null;
            }

            var l = new List<int>();

            for (int i = s.Length - 1; i >= 2; i--)
            {
                char c = s[i];
                if (s[i] == ':' && s[i - 1] == ':' && s[i - 2] == ':')
                {
                    l.Add(i);

                    if (l.Count > 1)
                    {
                        break;
                    }
                }
            }

            if (l.Count < 2)
            {
                return null;
            }

            int indx1 = l[l.Count - 2];
            int indx2 = l[l.Count - 1];

            string s1 = null;
            for (int i = indx1 + 1; i < indx1 + 10; i++)
            {
                char c = s[i];
                int ci = (int)c;
                if (ci == 10 || ci == 13)
                {
                    break;
                }
                s1 += c;
            }
            s1 = s1.Trim();

            string s2 = null;
            for (int i = indx2 + 1; i < indx2 + 10; i++)
            {
                char c = s[i];
                int ci = (int)c;
                if (ci == 10 || ci == 13)
                {
                    break;
                }
                s2 += c;
            }
            s2 = s2.Trim();

            Card c1 = ParseCardSpecial(s1);
            Card c2 = ParseCardSpecial(s2);

            return Tuple.Create(c1, c2);
        }

        private Card ParseCardSpecial(string s)
        {
            string value = new string(s.Take(s.Length - 1).ToArray());
            char suit = s[s.Length - 1];

            CardSuit cardSuit = CardUtils.ParseSuit(suit);

            CardValue cardValue;
            switch (value)
            {
                case "14":
                    cardValue = CardValue._Ace;
                    break;
                case "13":
                    cardValue = CardValue._King;
                    break;
                case "12":
                    cardValue = CardValue._Queen;
                    break;
                case "11":
                    cardValue = CardValue._Jack;
                    break;
                case "10":
                    cardValue = CardValue._10;
                    break;
                case "9":
                    cardValue = CardValue._9;
                    break;
                case "8":
                    cardValue = CardValue._8;
                    break;
                case "7":
                    cardValue = CardValue._7;
                    break;
                case "6":
                    cardValue = CardValue._6;
                    break;
                case "5":
                    cardValue = CardValue._5;
                    break;
                case "4":
                    cardValue = CardValue._4;
                    break;
                case "3":
                    cardValue = CardValue._3;
                    break;
                case "2":
                    cardValue = CardValue._2;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return new Card(cardSuit, cardValue);
        }

        private BitmapImage GetCardImage(System.Drawing.Bitmap cardImages, CardSuit cardSuit, CardValue cardValue)
        {
            int topx = 0;
            int topy = 0;

            switch (cardSuit)
            {
                case CardSuit.Spades:
                    topy = 98;
                    break;
                case CardSuit.Hearts:
                    topy = 196;
                    break;
                case CardSuit.Diamonds:
                    topy = 294;
                    break;
                case CardSuit.Clubs:
                    topy = 0;
                    break;
                default:
                    throw Utility.GetUnknownEnumValueException(cardSuit);
            }

            int cardValue_int;
            switch (cardValue)
            {
                case CardValue._2:
                    cardValue_int = 2;
                    break;
                case CardValue._3:
                    cardValue_int = 3;
                    break;
                case CardValue._4:
                    cardValue_int = 4;
                    break;
                case CardValue._5:
                    cardValue_int = 5;
                    break;
                case CardValue._6:
                    cardValue_int = 6;
                    break;
                case CardValue._7:
                    cardValue_int = 7;
                    break;
                case CardValue._8:
                    cardValue_int = 8;
                    break;
                case CardValue._9:
                    cardValue_int = 9;
                    break;
                case CardValue._10:
                    cardValue_int = 10;
                    break;
                case CardValue._Jack:
                    cardValue_int = 11;
                    break;
                case CardValue._Queen:
                    cardValue_int = 12;
                    break;
                case CardValue._King:
                    cardValue_int = 13;
                    break;
                case CardValue._Ace:
                    cardValue_int = 1;
                    break;
                default:
                    throw Utility.GetUnknownEnumValueException(cardValue);
            }

            topx = 73 * (cardValue_int - 1);

            var rect = new System.Drawing.Rectangle(topx, topy, 73, 97);
            System.Drawing.Bitmap cropped = cardImages.Clone(rect, cardImages.PixelFormat);

            var ms = new MemoryStream();
            cropped.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            cropped.Dispose();

            var bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            return bi;
        }

        private Dictionary<Card, BitmapImage> BuildCardImages()
        {
            Card[] cards = GetAllCards();

            System.Drawing.Bitmap cardImagesBitmap = Properties.Resources.cardfaces;

            var cardImages = new Dictionary<Card, BitmapImage>(CardComparer.Default);
            foreach (Card card in cards)
            {
                BitmapImage bi = GetCardImage(cardImagesBitmap, card.Suit, card.Value);
                cardImages[card] = bi;
            }

            return cardImages;
        }

        private static Card[] ParseCards(string s)
        {
            if (s.IsNullOrEmptyStr())
            {
                return null;
            }

            int count = s.Length;
            if (count % 2 == 1)
            {
                count--;
            }

            var cards = new List<Card>();

            for (int i = 0; i < count; i += 2)
            {
                cards.Add(CardUtils.ParseCard(s, i));
            }

            return cards.ToArray();
        }

        private static string ConvertCardsToStr(Card[] cards)
        {
            if (cards.IsNullOrEmpty())
            {
                return null;
            }

            return string.Join(string.Empty, cards.Select(x => CardUtils.ConvertToString(x)));
        }

        private void GlobalKeyboardHook(int key)
        {
            char ch = (char)key;
            try
            {
                GlobalKeyboardHookHandler(ch);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void GlobalKeyboardHookHandler(char ch)
        {
            if (MainView.IsActive)
            {
                return;
            }

            if (!IsWatchGlobalKeys)
            {
                return;
            }

            ch = char.ToLower(ch);

            int i;
            bool b = int.TryParse(ch.ToString(), out i);

            if (b)
            {
                EnemyPlayerCount = i;
                return;
            }

            if (ch == MainWindowPr.ExternalInput_StartSimulation)
            {
                StartSimulation();
                return;
            }

            if (ch == MainWindowPr.ExternalInput_StopSimulation)
            {
                _cancelTokenSource.Cancel();
                return;
            }

            if (ch == MainWindowPr.ExternalInput_GetCardsFromLogFile)
            {
                Task.Factory.StartNew(
                    () =>
                    {
                        try
                        {
                            HandleLogFileCards();
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                        }
                    });

                return;
            }
        }

        private void SimulationFinished(Exception ex, bool cancelled, Statistic stat, TimeSpan calcTime)
        {
            Calculating = false;
            _cancelTokenSource = null;

            CalculationTime = calcTime;
            CalculatedStatistic = stat;

            if (ex != null)
            {
                CalculatedStatistic = null;
                Logger.LogException(ex);
            }

            if (cancelled)
            {
                CalculatedStatistic = null;
            }

            MainView.Pr_SimulationFinished();
        }

        private static bool IsCardsNonOrderEqual(Card[] cards1, Card[] cards2)
        {
            var c1 = cards1.OrderBy(x => x, CardComparer.Default).ToArray();
            var c2 = cards2.OrderBy(x => x, CardComparer.Default).ToArray();
            return c1.SequenceEqual(c2);
        }

        private static void UseClipboardInSTA(Action action)
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                action();
            }
            else
            {
                var th = new Thread(() => { action(); });
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
                th.Join();
            }
        }

        public static T UseClipboardInSTA<T>(Func<T> func)
        {
            T res = default(T);

            UseClipboardInSTA(
                () =>
                {
                    res = func();
                });

            return res;
        }

        public void ExecOrExecAfterCancelIfCalculating(Action exec)
        {
            CancellationTokenSource cts = _cancelTokenSource;
            Task calculationTask = _calculationTask;
            if (Calculating)
            {
                cts.Cancel();

                calculationTask.ContinueWith(
                    (Task ancestor) =>
                    {
                        try
                        {
                            exec();
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                        }
                    });
            }
            else
            {
                exec();
            }
        }

        private void ProcessParsedFromLogFileCards(Card c1, Card c2)
        {
            bool distinct = true;

            if (!Cards.IsNullOrEmpty() && Cards.Length >= 2)
            {
                distinct = !IsCardsNonOrderEqual(new Card[] { c1, c2, }, Cards.Take(2).ToArray());
            }

            if (!distinct)
            {
                return;
            }

            ExecOrExecAfterCancelIfCalculating(
                () =>
                {
                    SetCards(new Card[] { c1, c2, });

                    if (IsCalcLogCardsImmediately)
                    {
                        StartSimulation();
                    }
                });
        }

        public void StartSimulation()
        {
            _calculationTask = null;
            CalculationParameters = null;

            try
            {
                CalculationParameters = GetParameters();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                MainView.Pr_ParametersAreNotValid(ex);
                return;
            }

            MainView.Pr_SimulationStarted();

            TimeSpan calcTime = TimeSpan.Zero;
            Statistic stat = null;

            Calculating = true;
            _cancelTokenSource = new CancellationTokenSource();
            CalculationParameters.CancelToken = _cancelTokenSource.Token;

            _calculationTask = Task.Factory.StartNew(
               () =>
               {
                   var psc = new PokerStatisticCalc();
                   var sw = Stopwatch.StartNew();
                   stat = psc.RunExperiment(CalculationParameters);
                   sw.Stop();
                   calcTime = sw.Elapsed;
               },
               _cancelTokenSource.Token);

            _calculationTask.ContinueWith(
                (Task ancestor) =>
                {
                    try
                    {
                        SimulationFinished(ancestor.Exception, ancestor.IsCanceled, stat, calcTime);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                });

            StartMonitor_ProgressWatcher();
        }


        #region Monitors

        //
        private void StartMonitor_LogFile()
        {
            Task.Factory.StartNew(
                () =>
                {
                    while (true)
                    {
                        Thread.Sleep(FileMonitorSleep);

                        try
                        {
                            LogFileMonitor();
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                        }
                    }
                });
        }

        private void LogFileMonitor()
        {
            if (Calculating)
            {
                return;
            }

            if (!IsWatchCardsInClipboard)
            {
                return;
            }

            var fi = new FileInfo(MainWindowPr.LogFilePath);
            if (!fi.Exists)
            {
                return;
            }

            if (_logFileSize == null || fi.Length == _logFileSize.Value)
            {
                return;
            }

            _logFileSize = fi.Length;

            Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        HandleLogFileCards();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                });
        }

        private void HandleLogFileCards()
        {
            if (!File.Exists(LogFilePath))
            {
                return;
            }

            string str = null;
            try
            {
                using (var fs = File.Open(MainWindowPr.LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    str = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            try
            {
                Tuple<Card, Card> cards = ParseLast2CardsFromLogFile(str);
                if (cards == null || cards.Item1 == null || cards.Item2 == null)
                {
                    return;
                }

                ProcessParsedFromLogFileCards(cards.Item1, cards.Item2);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        //
        private void StartMonitor_Clipboard()
        {
            Task.Factory.StartNew(
                () =>
                {
                    while (true)
                    {
                        Thread.Sleep(MainWindowPr.ClipboardMonitorSleep);

                        try
                        {
                            ClipboardMonitor();
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                        }
                    }
                });
        }

        private void ClipboardMonitor()
        {
            if (!IsWatchCardsInClipboard)
            {
                return;
            }

            string s = UseClipboardInSTA(() => Clipboard.GetText());

            if (s == _clipboardTxt)
            {
                return;
            }

            _clipboardTxt = s;

            if (_clipboardTxt.IsNullOrEmptyStr())
            {
                return;
            }

            var cards = new LinkedList<char>();

            bool b = false;
            for (int i = 0; i < _clipboardTxt.Length; i++)
            {
                if (_clipboardTxt[i] == '[')
                {
                    b = true;
                    continue;
                }
                else if (_clipboardTxt[i] == ']')
                {
                    break;
                }

                if (b)
                {
                    cards.AddLast(_clipboardTxt[i]);
                }
            }

            string cardsstr = new string(cards.ToArray());

            if (cardsstr.IsNullOrWhiteSpaceStr())
            {
                return;
            }

            cardsstr = cardsstr.Replace(" ", string.Empty);

            Card[] clipboardCards = null;
            try
            {
                clipboardCards = ParseCards(cardsstr);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return;
            }

            if (clipboardCards.IsNullOrEmpty())
            {
                return;
            }

            ExecOrExecAfterCancelIfCalculating(
                () =>
                {
                    SetCards(clipboardCards, true);
                    StartSimulation();
                });
        }

        //
        private void StartMonitor_ProgressWatcher()
        {
            Task.Factory.StartNew(
                () =>
                {
                    while (true)
                    {
                        Thread.Sleep(ProgressWatcherSleep);

                        bool finish;
                        try
                        {
                            ProgressWatcherMonitor(out finish);
                        }
                        catch (Exception ex)
                        {
                            finish = false;
                            Logger.LogException(ex);
                        }

                        if (finish)
                        {
                            break;
                        }
                    }
                });
        }

        private void ProgressWatcherMonitor(out bool finish)
        {
            finish = false;

            Task calcTask = _calculationTask;
            if (calcTask == null)
            {
                finish = true;
                return;
            }

            if (calcTask.IsCompleted)
            {
                finish = true;
                return;
            }

            CalculationParameters expParams = CalculationParameters;
            if (expParams == null)
            {
                finish = true;
                return;
            }

            int val = expParams.SimulatedGamesCount;
            MainView.Pr_SetProgress(val);
        }


        #endregion


        private CalculationParameters GetParameters()
        {
            var param = new CalculationParameters();

            if (!SimulatedGamesCount_IsValid)
            {
                SetSimulatedGamesCount(ParamHelper.GetSimulatedGameCount());
            }
            param.GameNumber = SimulatedGamesCount;

            if (!ParallelismLevel_IsValid)
            {
                SetParallelismLevel(ParamHelper.GetParallelismLevel());
            }
            param.ParallelLevel = ParallelismLevel;

            if (!EnemyPlayerCount_IsValid)
            {
                SetEnemyPlayerCount(ParamHelper.GetEnemyPlayersCount());
            }
            param.EnemyPlayersCount = EnemyPlayerCount;

            if (CalculationTimeLimitEnabled)
            {
                if (!CalculationTimeLimit_IsValid)
                {
                    SetCalculationTimeLimit(ParamHelper.GetCalculationTimeLimit());
                }
                param.TimeLimit = TimeSpan.FromMilliseconds(CalculationTimeLimit);
            }
            else
            {
                param.TimeLimit = null;
            }

            if (!Cards_IsValid)
            {
                SetCards(GetRandomHand(_rnd));
            }

            Card[] cards = Cards.ToArray();

            param.PlayerCard1 = cards[0];
            param.PlayerCard2 = cards[1];

            param.Flop1 = Utility.TryGetByIndx(cards, 2);
            param.Flop2 = Utility.TryGetByIndx(cards, 3);
            param.Flop3 = Utility.TryGetByIndx(cards, 4);
            param.Turn = Utility.TryGetByIndx(cards, 5);
            param.River = Utility.TryGetByIndx(cards, 6);

            return param;
        }

        private void SetCards(Card[] cards, bool add = false)
        {
            if (add)
            {
                if (!cards.IsNullOrEmpty())
                {
                    List<Card> res_cards;
                    if (!Cards.IsNullOrEmpty())
                    {
                        res_cards = Cards.ToList();
                    }
                    else
                    {
                        res_cards = new List<Card>();
                    }
                    res_cards.AddRange(cards);
                }
            }
            else
            {
                if (cards.IsNullOrEmpty())
                {
                    Cards = null;
                }
                else
                {
                    Cards = cards.ToArray();
                }
            }

            Cards_IsValid = true;
            Cards_Str = ConvertCardsToStr(Cards);
            MainView.Pr_CardsChanged();
        }

        private void SetSimulatedGamesCount(int val)
        {
            SimulatedGamesCount = val;
            SimulatedGamesCount_IsValid = true;
            SimulatedGamesCount_Str = val.ToString();
            MainView.Pr_SimulatedGamesCount_Changed();
        }

        private void SetParallelismLevel(int val)
        {
            ParallelismLevel = val;
            ParallelismLevel_IsValid = true;
            ParallelismLevel_Str = val.ToString();
            MainView.Pr_ParallelismLevel_Changed();
        }

        private void SetEnemyPlayerCount(int val)
        {
            EnemyPlayerCount = val;
            EnemyPlayerCount_IsValid = true;
            EnemyPlayerCount_Str = val.ToString();
            MainView.Pr_EnemyPlayerCount_Changed();
        }

        private void SetCalculationTimeLimit(double val)
        {
            CalculationTimeLimit = val;
            CalculationTimeLimit_IsValid = true;
            CalculationTimeLimit_Str = val.ToString(CultureInfo.InvariantCulture);
            MainView.Pr_CalculationTimeLimit_Changed();
        }

        private void SetCalculationTimeLimitEnabled(bool val)
        {
            CalculationTimeLimitEnabled = val;
            MainView.Pr_CalculationTimeLimitEnabled_Changed();
        }
    }
}