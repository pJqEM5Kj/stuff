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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PokerHelper;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //@!mov todo:
        // deep refactor

        private const int ActivationTimeout = 50; //ms
        private const int enemyPlayerCount_tb_SelectTime = 300; //ms
        private const int DefaultSimulatedGameCount = 50000;
        private const int DefaultEnemyPlayersCount = 1;
        private const int DefaultTimeLimit = 1700; //ms
        private const int ClipboardMonitorSleep = 100; //ms
        private const int FileMonitorSleep = 600; //ms
        private const int ProgressWatcherSleep = 100; //ms
        private const string AppVersionFormatString = "app v{0}";
        private const string LibVersionFormatString = "lib v{0}";
        private const char ExternalInput_StartSimulation = 'q';
        private const char ExternalInput_StopSimulation = 'w';
        private const char ExternalInput_GetCardsFromLogFile = 'a';
        private const string LogFilePath = @"C:\Program Files\PokerStars\PokerStars.log.0";

        private Dictionary<int, Tuple<double, double>> HoleCardsStatistic = new Dictionary<int, Tuple<double, double>>()
        {
            { 1, Tuple.Create(29.148, 84.815) },
            { 2, Tuple.Create(18.32635, 73.20935) },
            { 3, Tuple.Create(12.9407, 63.6387) },
            { 4, Tuple.Create(10.05565, 55.839) },
            { 5, Tuple.Create(8.20165, 49.1143) },
            { 6, Tuple.Create(7.0137, 43.33665) },
            { 7, Tuple.Create(6.205, 38.608) },
            { 8, Tuple.Create(4.503, 34.407) },
            { 9, Tuple.Create(4.941, 30.676) },
        };

        private readonly Logger Logger = new Logger();

        private KeyInterceptor keyHooker;
        private System.Timers.Timer enemyPlayerCount_tb_TextChanged_Timer;
        private Dictionary<Card, BitmapImage> CardImages;
        private Random rnd = new Random(Guid.NewGuid().GetHashCode());
        private Control lastFocusedElement;
        private bool ignore_cards_tb_TextChanged = false;

        private long logFileSize;
        private string clipboardTxt;

        private bool Calculating;
        private CancellationTokenSource CancelTokenSource;
        private ExperimentParameters ExpParams = null;
        private Task CalculationTask;


        public MainWindow()
        {
            InitializeComponent();

            topMost_ckb.Checked += topMost_ckb_Checked;
            topMost_ckb.Unchecked += topMost_ckb_Unchecked;

            enemyPlayerCount_tb.KeyDown += enemyPlayerCount_tb_KeyDown;
            parallelismLevel_tb.KeyDown += parallelismLevel_tb_KeyDown;
            simulatedGamesCount_tb.KeyDown += simulatedGamesCount_tb_KeyDown;
            cards_tb.KeyDown += cards_tb_KeyDown;
            cards_tb.PreviewKeyDown += cards_tb_PreviewKeyDown;
            timeLimit_tb.KeyDown += timeLimit_tb_KeyDown;

            runSimulation_btn.Click += runSimulation_btn_Click;

            enemyPlayerCount_tb.GotKeyboardFocus += enemyPlayerCount_tb_GotKeyboardFocus;
            enemyPlayerCount_tb.TextChanged += enemyPlayerCount_tb_TextChanged;

            enemyPlayerCount_tb_TextChanged_Timer = new System.Timers.Timer(enemyPlayerCount_tb_SelectTime);
            enemyPlayerCount_tb_TextChanged_Timer.Elapsed += enemyPlayerCount_tb_TextChanged_Timer_Elapsed;

            cards_tb.TextChanged += cards_tb_TextChanged;

            Loaded += MainWindow_Loaded;
            Deactivated += MainWindow_Deactivated;
            KeyDown += MainWindow_KeyDown;

            Closed += MainWindow_Closed;

            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }


        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            DisposeKeyHooker();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            DisposeKeyHooker();
        }

        private void cards_tb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            bool shiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (shiftDown && (e.Key == Key.Delete || e.Key == Key.Back))
            {
                cards_tb.Clear();
            }

            if (shiftDown && e.Key == Key.Enter)
            {
                RandomCards();
                e.Handled = true;
            }
        }

        private void timeLimit_tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartSimulation();
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            CancellationTokenSource cts = CancelTokenSource;
            if (Calculating && e.Key == Key.Escape)
            {
                cts.Cancel();
            }
        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            bool reactivateWindow = false;

            if (Keyboard.IsKeyToggled(Key.CapsLock))
            {
                reactivateWindow = true;
            }

            if (reactivateWindow && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            {
                reactivateWindow = false;
            }

            if (reactivateWindow)
            {
                Dispatcher.BeginInvoke(new Action(ActivateWnd));
            }
        }

        private void ActivateWnd()
        {
            Thread.Sleep(ActivationTimeout);
            if (!Activate())
            {
                Dispatcher.BeginInvoke(new Action(ActivateWnd));
            }
        }

        private void cards_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ignore_cards_tb_TextChanged)
            {
                return;
            }
            ignore_cards_tb_TextChanged = true;

            string ss = null;
            int caretIndx = cards_tb.CaretIndex;
            for (int i = 0; i < cards_tb.Text.Length; i++)
            {
                ss += (i % 2 == 0) ? char.ToUpper(cards_tb.Text[i]) : char.ToLower(cards_tb.Text[i]);
            }
            cards_tb.Text = ss;
            cards_tb.CaretIndex = caretIndx;
            ignore_cards_tb_TextChanged = false;

            try
            {
                Card[] cards = GetCards(cards_tb.Text);
                DisplayCards(cards);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);

                card1_img.Source = null;
                card2_img.Source = null;
                card3_img.Source = null;
                card4_img.Source = null;
                card5_img.Source = null;
                card6_img.Source = null;
                card7_img.Source = null;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetVersions();

            cards_tb.Focus();

            parallelismLevel_tb.Text = ParamHelper.GetParallelLevel().ToString();
            simulatedGamesCount_tb.Text = DefaultSimulatedGameCount.ToString();
            enemyPlayerCount_tb.Text = DefaultEnemyPlayersCount.ToString();
            timeLimit_ckb.IsChecked = true;
            timeLimit_tb.Text = Math.Round(DefaultTimeLimit / 1000d, 2).ToString(CultureInfo.InvariantCulture);

            CardImages = BuildCardImages();

            cards_tb.Text = string.Join(string.Empty, GetRandomHand().Select(x => CardUtils.ConvertToString(x)));

            StartMonitorLogFile();
            StartMonitorClipboard();

            keyHooker = new KeyInterceptor();
            keyHooker.SetHook(KeyboardHook);
        }

        private void KeyboardHook(int key)
        {
            try
            {
                Dispatcher.BeginInvoke(new Action(
                    () =>
                    {
                        char ch = (char)key;
                        try
                        {
                            KeyboardHookProcessing(ch);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex);
                        }
                    }));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void KeyboardHookProcessing(char ch)
        {
            if (IsActive)
            {
                return;
            }

            if (!(watchKeys_ckb.IsChecked ?? false))
            {
                return;
            }

            ch = char.ToLower(ch);

            int i;
            bool b = int.TryParse(ch.ToString(), out i);

            if (b)
            {
                enemyPlayerCount_tb.Text = i.ToString();
                return;
            }

            if (ch == ExternalInput_StartSimulation)
            {
                StartSimulation();
                return;
            }

            if (ch == ExternalInput_StopSimulation)
            {
                CancelTokenSource.Cancel();
                return;
            }

            if (ch == ExternalInput_GetCardsFromLogFile)
            {
                Task.Factory.StartNew(
                    () =>
                    {
                        try
                        {
                            LogFile_Changed();
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex, "Get cards on request failed.");
                        }
                    });

                return;
            }
        }

        private void enemyPlayerCount_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            enemyPlayerCount_tb_TextChanged_Timer.Stop();
            enemyPlayerCount_tb_TextChanged_Timer.Start();
        }

        private void enemyPlayerCount_tb_TextChanged_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            enemyPlayerCount_tb_TextChanged_Timer.Stop();

            Dispatcher.Invoke(new Action(
                () =>
                {
                    enemyPlayerCount_tb.SelectAll();

                    if (enemyPlayerCount_tb.IsFocused
                        && !string.IsNullOrWhiteSpace(enemyPlayerCount_tb.Text))
                    {
                        enemyPlayerCount_tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    }
                }));
        }

        private void enemyPlayerCount_tb_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            enemyPlayerCount_tb.SelectAll();
        }

        private void runSimulation_btn_Click(object sender, RoutedEventArgs e)
        {
            StartSimulation();
        }

        private void cards_tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartSimulation();
            }
        }

        private void simulatedGamesCount_tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartSimulation();
            }
        }

        private void parallelismLevel_tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartSimulation();
            }
        }

        private void enemyPlayerCount_tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StartSimulation();
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

        private void DisposeKeyHooker()
        {
            if (keyHooker == null)
            {
                return;
            }

            keyHooker.RemoveHook();
            keyHooker = null;
        }

        private void RandomCards()
        {
            cards_tb.Text = string.Join(string.Empty, GetRandomHand().Select(x => CardUtils.ConvertToString(x)));
        }

        private bool IsWatchCards()
        {
            return DispatcherCall(
                () =>
                {
                    return watchCards_ckb.IsChecked ?? false;
                });
        }

        private bool IsCalcLogCardsImmediately()
        {
            return DispatcherCall(
                () =>
                {
                    return calcLogCardsNow_ckb.IsChecked ?? false;
                });
        }

        private T DispatcherCall<T>(Func<T> f)
        {
            Code.RequireNotNull(f);

            if (Thread.CurrentThread.ManagedThreadId != Dispatcher.Thread.ManagedThreadId)
            {
                T res = default(T);
                Dispatcher.Invoke(new Action(
                    () =>
                    {
                        res = f();
                    }));
                return res;
            }

            return f();
        }

        private void SetVersions()
        {
            Version applicationVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Version coreLibVersion = Assembly.GetAssembly(typeof(PokerHelper.PokerStatisticCalc)).GetName().Version;

            appVersion_lbl.Content = AppVersionFormatString.FormatStr(applicationVersion.ToString());
            libVersion_lbl.Content = LibVersionFormatString.FormatStr(coreLibVersion.ToString());
        }

        private void StartMonitorClipboard()
        {
            Task.Factory.StartNew(
                () =>
                {
                    while (true)
                    {
                        Thread.Sleep(ClipboardMonitorSleep);

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
            Clipboard.GetText();
        }

        private void ClipboardMonitor()
        {
            if (!IsWatchCards())
            {
                return;
            }

            string s = null;

            Dispatcher.Invoke(new Action(
                () =>
                {
                    s = Clipboard.GetText();
                }));

            if (s == clipboardTxt)
            {
                return;
            }

            clipboardTxt = s;

            if (string.IsNullOrEmpty(clipboardTxt))
            {
                return;
            }

            var cards = new LinkedList<char>();

            bool b = false;
            for (int i = 0; i < clipboardTxt.Length; i++)
            {
                if (clipboardTxt[i] == '[')
                {
                    b = true;
                    continue;
                }
                else if (clipboardTxt[i] == ']')
                {
                    break;
                }

                if (b)
                {
                    cards.AddLast(clipboardTxt[i]);
                }
            }

            string cardsstr = new string(cards.ToArray());

            if (string.IsNullOrWhiteSpace(cardsstr))
            {
                return;
            }

            cardsstr = cardsstr.Replace(" ", string.Empty);

            try
            {
                // try parse
                GetCards(cardsstr);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return;
            }

            CancellationTokenSource cts = CancelTokenSource;
            Task calculationTask = CalculationTask;
            if (Calculating)
            {
                cts.Cancel();

                calculationTask.ContinueWith(
                    (Task ancestor) =>
                    {
                        Dispatcher.BeginInvoke(new Action(
                            () =>
                            {
                                cards_tb.Text += cardsstr;
                                StartSimulation();
                            }));
                    });
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(
                    () =>
                    {
                        cards_tb.Text += cardsstr;
                        StartSimulation();
                    }));
            }
        }

        private void StartMonitorLogFile()
        {
            Task.Factory.StartNew(LogFileMonitor);
        }

        private void LogFileMonitor()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(FileMonitorSleep);

                    if (Calculating)
                    {
                        continue;
                    }

                    if (!IsWatchCards())
                    {
                        continue;
                    }

                    var fi = new FileInfo(LogFilePath);
                    if (!fi.Exists)
                    {
                        continue;
                    }

                    if (fi.Length == logFileSize)
                    {
                        continue;
                    }

                    logFileSize = fi.Length;

                    Task.Factory.StartNew(
                        () =>
                        {
                            try
                            {
                                LogFile_Changed();
                            }
                            catch (Exception ex2)
                            {
                                Logger.LogException(ex2, "LogFile_Changed handler failed.");
                            }
                        });
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, "Log file processing failed.");
                }
            }
        }

        private void LogFile_Changed()
        {
            if (!File.Exists(LogFilePath))
            {
                return;
            }

            string str = null;
            try
            {
                using (var fs = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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

                Dispatcher.Invoke(new Action(
                    () =>
                    {
                        ProcessParsedFromLogFileCards(cards.Item1, cards.Item2);
                    }));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private Tuple<Card, Card> ParseLast2CardsFromLogFile(string s)
        {
            if (string.IsNullOrEmpty(s))
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

        private void ProcessParsedFromLogFileCards(Card c1, Card c2)
        {
            bool distinct = true;

            if (!string.IsNullOrEmpty(cards_tb.Text))
            {
                Card[] cards = GetCards(cards_tb.Text);
                if (cards.Length > 1)
                {
                    distinct = !IsEquals(new Card[] { c1, c2, }, cards.Take(2).ToArray());
                }
            }

            if (!distinct)
            {
                return;
            }

            string str = CardUtils.ConvertToString(c2) + CardUtils.ConvertToString(c1);

            Action exec =
                () =>
                {
                    cards_tb.Text = str;
                    if (IsCalcLogCardsImmediately())
                    {
                        StartSimulation();
                    }
                };

            CancellationTokenSource cts = CancelTokenSource;
            Task calculationTask = CalculationTask;
            if (Calculating)
            {
                cts.Cancel();

                calculationTask.ContinueWith(
                    (Task ancestor) =>
                    {
                        Dispatcher.BeginInvoke(exec);
                    });
            }
            else
            {
                exec();
            }
        }

        private bool IsEquals(Card[] cards1, Card[] cards2)
        {
            List<string> l1 = cards1.Select(x => CardUtils.ConvertToString(x)).ToList();
            l1.Sort();

            List<string> l2 = cards2.Select(x => CardUtils.ConvertToString(x)).ToList();
            l2.Sort();

            return l1.SequenceEqual(l2);
        }

        private void SimulationStartedShow()
        {
            lastFocusedElement = Keyboard.FocusedElement as Control;

            mainGrid.IsEnabled = false;
            waiter.Visibility = System.Windows.Visibility.Visible;

            progress_bar.Visibility = System.Windows.Visibility.Visible;
            progress_bar.Value = 0;
        }

        private void SimulationFinishedShow()
        {
            mainGrid.IsEnabled = true;
            waiter.Visibility = System.Windows.Visibility.Hidden;
            progress_bar.Visibility = System.Windows.Visibility.Collapsed;

            if (lastFocusedElement != null)
            {
                lastFocusedElement.Focus();
                lastFocusedElement = null;
            }
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
                Image cardImgCtrl = TryGetByIndx(cardImgCtrls, i, null);
                Card card = TryGetByIndx(cards, i, null);

                if (cardImgCtrl == null)
                {
                    continue;
                }

                if (card != null)
                {
                    cardImgCtrl.Source = CardImages[card];
                }
                else
                {
                    cardImgCtrl.Source = null;
                }
            }
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

        private Card[] GetAllCards()
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

        private void StartSimulation()
        {
            CalculationTask = null;
            ExpParams = null;

            try
            {
                ExpParams = GetParameters();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                result1_tb.Text = "Parameters are not valid:" + Environment.NewLine + ex.ToString();
                result2_tb.Text = string.Empty;
                result3_tb.Text = string.Empty;
                return;
            }

            SimulationStartedShow();

            TimeSpan calcTime = TimeSpan.Zero;
            Statistic stat = null;

            Calculating = true;
            CancelTokenSource = new CancellationTokenSource();
            ExpParams.CancelToken = CancelTokenSource.Token;

            progress_bar.Minimum = 0;
            progress_bar.Maximum = ExpParams.GameNumber;

            CalculationTask = Task.Factory.StartNew(
               () =>
               {
                   var psc = new PokerStatisticCalc();
                   var sw = Stopwatch.StartNew();
                   stat = psc.RunExperiment(ExpParams);
                   sw.Stop();
                   calcTime = sw.Elapsed;
               },
               CancelTokenSource.Token);

            CalculationTask.ContinueWith(
                (Task ancestor) =>
                {
                    Dispatcher.Invoke(new Action(
                        () =>
                        {
                            SimulationFinished(ancestor.Exception, ancestor.IsCanceled, stat, calcTime);
                        }));
                });

            Task.Factory.StartNew(
                () =>
                {
                    while (!CalculationTask.IsCompleted)
                    {
                        Thread.Sleep(ProgressWatcherSleep);

                        ExperimentParameters expParams = ExpParams;
                        if (expParams == null)
                        {
                            continue;
                        }

                        int val = expParams.SimulatedGamesCount;

                        Dispatcher.BeginInvoke(new Action(
                            () =>
                            {
                                progress_bar.Value = val;
                            }));
                    }
                });
        }

        private void SimulationFinished(Exception ex, bool cancelled, Statistic stat, TimeSpan calcTime)
        {
            Calculating = false;
            CancelTokenSource = null;

            SimulationFinishedShow();

            if (ex != null)
            {
                Logger.LogException(ex);
                result1_tb.Text = "Error occured:" + Environment.NewLine + ex.ToString();
                result2_tb.Text = string.Empty;
                result3_tb.Text = string.Empty;
                return;
            }

            if (cancelled)
            {
                result1_tb.Text = "Cancelled.";
                result2_tb.Text = string.Empty;
                result3_tb.Text = string.Empty;
                return;
            }

            ShowStatistic(stat, calcTime);
        }

        private ExperimentParameters GetParameters()
        {
            var param = new ExperimentParameters();

            if (!string.IsNullOrWhiteSpace(simulatedGamesCount_tb.Text))
            {
                param.GameNumber = int.Parse(simulatedGamesCount_tb.Text);
            }
            else
            {
                param.GameNumber = DefaultSimulatedGameCount;
                simulatedGamesCount_tb.Text = param.GameNumber.ToString();
            }

            if (!string.IsNullOrWhiteSpace(parallelismLevel_tb.Text))
            {
                param.ParallelLevel = int.Parse(parallelismLevel_tb.Text);
            }
            else
            {
                param.ParallelLevel = ParamHelper.GetParallelLevel();
                parallelismLevel_tb.Text = param.ParallelLevel.ToString();
            }

            if (!string.IsNullOrWhiteSpace(enemyPlayerCount_tb.Text))
            {
                param.EnemyPlayersCount = int.Parse(enemyPlayerCount_tb.Text);
            }
            else
            {
                param.EnemyPlayersCount = DefaultEnemyPlayersCount;
                enemyPlayerCount_tb.Text = param.EnemyPlayersCount.ToString();
            }

            if (timeLimit_ckb.IsChecked ?? false)
            {
                if (!string.IsNullOrWhiteSpace(timeLimit_tb.Text))
                {
                    double seconds = double.Parse(timeLimit_tb.Text, CultureInfo.InvariantCulture);
                    param.TimeLimit = TimeSpan.FromSeconds(seconds);
                }
                else
                {
                    param.TimeLimit = TimeSpan.FromMilliseconds(DefaultTimeLimit);
                    timeLimit_tb.Text = Math.Round(param.TimeLimit.Value.Milliseconds / 1000d, 2).ToString(CultureInfo.InvariantCulture);
                }
            }
            else
            {
                param.TimeLimit = null;
            }

            if (string.IsNullOrWhiteSpace(cards_tb.Text))
            {
                RandomCards();
            }

            Card[] cards = GetCards(cards_tb.Text);

            param.PlayerCard1 = cards[0];
            param.PlayerCard2 = cards[1];

            param.Flop1 = TryGetByIndx(cards, 2);
            param.Flop2 = TryGetByIndx(cards, 3);
            param.Flop3 = TryGetByIndx(cards, 4);
            param.Turn = TryGetByIndx(cards, 5);
            param.River = TryGetByIndx(cards, 6);

            return param;
        }

        private Card[] GetCards(string s)
        {
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

        private void ShowStatistic(Statistic stat, TimeSpan calcTime)
        {
            string s = null;

            double win_percent = 100 * stat.Win / (double)stat.GameNumber;

            s += "Wins:".PadRight(8) + "{0:0.####}%".FormatStr(win_percent);
            s += Environment.NewLine;
            s += "Draws:".PadRight(8) + "{0:0.####}%".FormatStr(100 * stat.Draw / (double)stat.GameNumber);
            s += Environment.NewLine;
            s += "Loses:".PadRight(8) + "{0:0.####}%".FormatStr(100 * stat.Lose / (double)stat.GameNumber);

            if (ExpParams.Flop1 == null && HoleCardsStatistic.ContainsKey(ExpParams.EnemyPlayersCount))
            {
                s += Environment.NewLine;
                s += Environment.NewLine;

                double min = HoleCardsStatistic[ExpParams.EnemyPlayersCount].Item1;
                double max = HoleCardsStatistic[ExpParams.EnemyPlayersCount].Item2;
                win_percent = Math.Max(win_percent, min);
                win_percent = Math.Min(win_percent, max);
                double d = 100 * (win_percent - min) / (max - min);

                s += "Hole cards value: " + "{0:0.####}%".FormatStr(d);
            }

            s += Environment.NewLine;
            s += Environment.NewLine;
            s += "Games:".PadRight(8) + stat.GameNumber.ToString();
            s += Environment.NewLine;
            s += "Enemy:".PadRight(8) + ExpParams.EnemyPlayersCount.ToString();
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

        private Card[] GetRandomHand()
        {
            Card[] cards = GetAllCards();
            Shuffle(cards);

            return cards.Take(rnd.Next(2, 8)).ToArray();
        }

        private void Shuffle(Card[] cards)
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

        private T TryGetByIndx<T>(IList<T> collection, int indx, T defaultValue = default(T))
        {
            if (collection.Count > indx)
            {
                return collection[indx];
            }

            return defaultValue;
        }
    }
}
