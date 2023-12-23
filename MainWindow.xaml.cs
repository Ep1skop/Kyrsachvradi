using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;

namespace Koursach_Tri_v_Ryad
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int gameDurationInSeconds = 5;
        private int remainingTimeInSeconds;
        private DispatcherTimer gameTimer;

        int bSize = 60;
        BitmapImage[] typedpic = new BitmapImage[]
        {
            new BitmapImage(new Uri(@"pack://application:,,,/imgs/0.png", UriKind.Absolute)),
            new BitmapImage(new Uri(@"pack://application:,,,/imgs/1.png", UriKind.Absolute)),
            new BitmapImage(new Uri(@"pack://application:,,,/imgs/2.png", UriKind.Absolute)),
            new BitmapImage(new Uri(@"pack://application:,,,/imgs/3.png", UriKind.Absolute)),
            new BitmapImage(new Uri(@"pack://application:,,,/imgs/4.png", UriKind.Absolute)),
            new BitmapImage(new Uri(@"pack://application:,,,/imgs/5.png", UriKind.Absolute)),
        };

        Element[,] elfield = new Element[w, w]; 
        GameLogic GameLog; 

        const int w = 8; 
        const int nulltipe = -99; 
        const int missscore = 5 * ((w - 2) * 3 * w * 2); 

        JsonSohr j= new JsonSohr();

        Player p;
        List<Player> ratelist = new List<Player>();

        public MainWindow()
        {
            
            ratelist.Clear();

            InitializeComponent();

            unigrid.Rows = w;
            unigrid.Columns = w;

            unigrid.Width = w * (bSize + 4);
            unigrid.Height = w * (bSize + 4);

            unigrid.Margin = new Thickness(5, 5, 5, 5);

            GameLog = new GameLogic(elfield);
            remainingTimeInSeconds = gameDurationInSeconds;
            TimeLabel.Content = FormatTime(remainingTimeInSeconds);
            InitializeGameTimer();


        }
        private void InitializeGameTimer()
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += GameTimer_Tick;
        }
        private void StartGame()
        {
            gameTimer.Start();

        }
        private void EndGame()
        {
            MessageBox.Show("Конец");

            string playerName = Convert.ToString(PlayerName.Content);
            int playerScore = Convert.ToInt32(totalscore.Content);

            Player currentPlayer = new Player(playerName, playerScore);

            ratelist.Add(currentPlayer);

            var sortedPlayers = from r in ratelist
                                orderby r.score descending
                                select r;


            Rate.Items.Clear();

            foreach (Player p in sortedPlayers)
            {
                Rate.Items.Add(p.name + ":     " + p.score);
            }

            PlayerName.Content = "";

        }
        private string FormatTime(int timeInSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(timeInSeconds);
            return string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
        }
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            remainingTimeInSeconds--;
            TimeLabel.Content = FormatTime(remainingTimeInSeconds);

            if (remainingTimeInSeconds <= 0)
            {
                gameTimer.Stop();
                EndGame();
            }
        }
       
        private void Falled(object sender, EventArgs args)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                Update();
            });
        }

 
        void Update()
        {
            for (int i = 0; i < w; i++)
                for (int j = 0; j < w; j++)
                {
                    StackPanel stack = new StackPanel();

                    int typeel = elfield[i, j].typeofpic;
                    if (typeel != nulltipe)
                    {
                        BitmapImage image = typedpic[typeel];
                        stack = getPanel(image);

                    }

                    elfield[i, j].b.Content = stack;
                }
        }
        StackPanel getPanel(BitmapImage picture)
        {
            StackPanel stackPanel = new StackPanel();
            Image image = new Image();
            image.Source = picture;
            stackPanel.Children.Add(image);
            stackPanel.Margin = new Thickness(1);

            return stackPanel;
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            int index = (int)(((Button)sender).Tag);

            int i = index % w;
            int j = index / w;

            GameLog.moveCell(i, j);

            totalscore.Content = Convert.ToString(GameLog.getScore() - missscore);
            Update();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            string name = Convert.ToString(PlayerName.Content);

            if (name != "Ваше имя: ")
            {
                ClearGrid();
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        elfield[i, j] = new Element(nulltipe, i + j * w);
                        StackPanel stackPanel = new StackPanel();
                        stackPanel.Margin = new Thickness(1);
                        elfield[i, j].b.Click += Btn_Click;
                        unigrid.Children.Add(elfield[i, j].b);
                    }
                }

                GameLog.GameSetScore(0); 
                GameLog.Falled += Falled;
                Update();
                GameLog.StartFall();

                TimeLabel.Content = FormatTime(gameDurationInSeconds); 
                remainingTimeInSeconds = gameDurationInSeconds; 
                StartGame();
            }
            else
            {
                MessageBox.Show("Введите имя");
            }
        }

        private void NameChange_Click(object sender, RoutedEventArgs e)
        {
            bool proverka = true;
            AddName win2 = new AddName();
            if (win2.ShowDialog() == true)
            {

                foreach (Player pl in ratelist)
                    if (win2.Name.Text == pl.name)
                    {
                        MessageBox.Show("Имя занято");

                        proverka = false;
                    }
                if (proverka == true)
                {
                    GameLog.GameSetScore(0);
                    p = new Player(win2.Name.Text, 0);
                    PlayerName.Content = "Ваше имя: " + p.getName();
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Rate.Items.Clear();
            p.setScore(Convert.ToInt32(totalscore.Content));

            ratelist.Add(p);

            var sortedPlayers = from r in ratelist
                                orderby r.score descending
                                select r;

            foreach (Player p in sortedPlayers)
                Rate.Items.Add(p.name + ":     " + p.score);

            PlayerName.Content = "";
            totalscore.Content = "";
            j.SaveFile(ratelist);

        }

        private void Load_Click_1(object sender, RoutedEventArgs e)
        {
            Rate.Items.Clear();
            ratelist = j.LoadFile();

            var sortedPlayers = from r in ratelist
                                orderby r.score descending
                                select r;

            foreach (Player p in sortedPlayers)
                Rate.Items.Add(p.name + ":     " + p.score);
        }
        private void ClearGrid()
        {
            unigrid.Children.Clear();
        }


        
    }
}
