using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MariEMontiRandomPizza
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Pizza> pizzaMenu = new List<Pizza>();
        private Random random = new Random();
        private Pizza selectedPizza = null;
        private DispatcherTimer slotMachineTimer;
        private int slotMachineCounter = 0;
        private const int SLOT_MACHINE_ITERATIONS = 20; // Numero di iterazioni dello slot machine
        private const int CHARACTER_REVEAL_DELAY_MS = 50; // Ritardo tra la rivelazione di ogni carattere
        private SoundPlayer spinSoundPlayer; // Campo per mantenere il riferimento al player
        private SoundPlayer winSoundPlayer; // Campo per mantenere il riferimento al player

        public MainWindow()
        {
            InitializeComponent();
            InitializeResources();
            InitializeGUI();
            LoadPizzaMenu();
            InitializeSlotMachineTimer();
        }

        private void LoadPizzaMenu()
        {
            try
            {
                // Percorso del file CSV (nella cartella dell'applicazione)
                string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "menu-pizze.csv");

                // Caricare le pizze dal file CSV
                pizzaMenu = PizzaMenuLoader.LoadPizzasFromCsv(filePath);

                // Aggiornare l'interfaccia utente con il menu caricato
                //AggiornaInterfacciaMenu();

                //MessageBox.Show($"Menu caricato con successo! {pizzaMenu.Count} pizze trovate.", "Caricamento completato", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il caricamento del menu: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeGUI()
        {
            this.Width = 900;
            this.Height = 700;
            this.Title = "Mare e Monti - Pizzeria d'Asporto";

            try
            {
                this.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/MariEMontiIcon.ico"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossibile caricare l'icona: " + ex.Message);
            }

            this.Background = new SolidColorBrush(Colors.White);
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ResizeMode = ResizeMode.CanMinimize;

            // Main Grid
            Grid mainGrid = new Grid();

            // Background with light blue color similar to the logo
            mainGrid.Background = new SolidColorBrush(Color.FromRgb(230, 240, 255));

            // Define rows
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(150) }); // Header
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Content
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) }); // Footer


            // Create logo tagline
            TextBlock logoTagline = new TextBlock
            {
                Text = "PIZZERIA D'ASPORTO",
                FontSize = 20,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204)), // Blue color similar to the logo
                Margin = new Thickness(10, 40, 10, 10),
                VerticalAlignment = VerticalAlignment.Bottom
            };

            // Add decoration element similar to the fish in the logo
            Border fishDecoration = new Border
            {
                Width = 60,
                Height = 30,
                Background = new SolidColorBrush(Color.FromRgb(0, 102, 204)),
                CornerRadius = new CornerRadius(15),
                Margin = new Thickness(5)
            };


            // Create content area
            Grid contentGrid = new Grid();
            contentGrid.Margin = new Thickness(20);

            // Define columns for the content area
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Left side - Pizza selection
            Border selectionBorder = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(220, 50, 50)), // Red color similar to the phone number in the logo
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(10),
                Padding = new Thickness(10)
            };

            StackPanel selectionPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock titleText = new TextBlock
            {
                Text = "SELEZIONA UNA PIZZA RANDOM",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(220, 50, 50)), // Red color
                Margin = new Thickness(0, 0, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            Button randomButton = new Button
            {
                Content = "SCEGLI UNA PIZZA",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromRgb(220, 50, 50)), // Red color
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(15, 10, 15, 10),
                Margin = new Thickness(0, 0, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            Button menuButton = new Button
            {
                Content = "VISUALIZZA MENU COMPLETO",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromRgb(0, 102, 204)), // Blue color
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(15, 8, 15, 8),
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Add click events to the buttons
            randomButton.Click += RandomButton_Click;
            menuButton.Click += MenuButton_Click;

            selectionPanel.Children.Add(titleText);
            selectionPanel.Children.Add(randomButton);
            selectionPanel.Children.Add(menuButton);

            selectionBorder.Child = selectionPanel;
            Grid.SetColumn(selectionBorder, 0);
            contentGrid.Children.Add(selectionBorder);

            // Right side - Result display
            Border resultBorder = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 102, 204)), // Blue color
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(10),
                Padding = new Thickness(15),
                Width = 350
            };

            StackPanel resultPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock resultTitleText = new TextBlock
            {
                Text = "LA TUA PIZZA",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204)), // Blue color
                Margin = new Thickness(0, 0, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            TextBlock pizzaNameText = new TextBlock
            {
                Text = "",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                MaxWidth = 280,
                Name = "PizzaNameText"
            };

            TextBlock pizzaIngredientsText = new TextBlock
            {
                Text = "",
                FontSize = 16,
                Foreground = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Name = "PizzaIngredientsText"
            };

            TextBlock pizzaPriceText = new TextBlock
            {
                Text = "",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(220, 50, 50)), // Red color
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                Name = "PizzaPriceText"
            };

            resultPanel.Children.Add(resultTitleText);
            resultPanel.Children.Add(pizzaNameText);
            resultPanel.Children.Add(pizzaIngredientsText);
            resultPanel.Children.Add(pizzaPriceText);

            resultBorder.Child = resultPanel;
            Grid.SetColumn(resultBorder, 1);
            contentGrid.Children.Add(resultBorder);

            // Register the text blocks for later reference
            this.RegisterName("PizzaNameText", pizzaNameText);
            this.RegisterName("PizzaIngredientsText", pizzaIngredientsText);
            this.RegisterName("PizzaPriceText", pizzaPriceText);

            // Add content grid to main grid
            Grid.SetRow(contentGrid, 1);
            mainGrid.Children.Add(contentGrid);

            // Create footer
            TextBlock footerText = new TextBlock
            {
                Text = "Pizza integrale: € 2,00 in più - Vendita vino sfuso D.O.P. € 2,20/litro",
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(220, 50, 50)), // Red color
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetRow(footerText, 2);
            mainGrid.Children.Add(footerText);

            // Set the content of the window
            this.Content = mainGrid;

            CreateLogoHeader(mainGrid);
        }

        private void InitializeResources()
        {
            try
            {
                spinSoundPlayer = new SoundPlayer("Resources/classic-slot-machine.wav");
                spinSoundPlayer.Load();

                winSoundPlayer = new SoundPlayer("Resources/game_win_success.wav");
                winSoundPlayer.Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante il caricamento del suono: " + ex.Message);
            }
        }
        // Event handler for random button click
        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            PlaySpinSound();

            // Disabilita il bottone durante l'animazione
            Button button = (Button)sender;
            button.IsEnabled = false;

            // Pulisci tutti i campi all'inizio dello spin
            TextBlock pizzaNameText = (TextBlock)this.FindName("PizzaNameText");
            TextBlock pizzaIngredientsText = (TextBlock)this.FindName("PizzaIngredientsText");
            TextBlock pizzaPriceText = (TextBlock)this.FindName("PizzaPriceText");

            pizzaNameText.Text = "";
            pizzaIngredientsText.Text = "";
            pizzaPriceText.Text = "";

            // Select a random pizza
            int randomIndex = random.Next(pizzaMenu.Count);
            selectedPizza = pizzaMenu[randomIndex];

            // Avvia l'animazione dello slot machine
            slotMachineCounter = 0;
            slotMachineTimer.Start();


            // Aggiunge un effetto di lampeggiamento al bordo del risultato
            Border resultBorder = VisualTreeHelper.GetParent((DependencyObject)FindName("PizzaNameText")) as Border;
            if (resultBorder != null && resultBorder.Parent is StackPanel panel)
            {
                Border outerBorder = panel.Parent as Border;
                if (outerBorder != null)
                {
                    ColorAnimation borderAnimation = new ColorAnimation
                    {
                        From = Color.FromRgb(220, 50, 50),
                        To = Color.FromRgb(0, 102, 204),
                        Duration = TimeSpan.FromMilliseconds(500),
                        AutoReverse = true,
                        RepeatBehavior = new RepeatBehavior(4)
                    };

                    SolidColorBrush borderBrush = new SolidColorBrush();
                    outerBorder.BorderBrush = borderBrush;
                    borderBrush.BeginAnimation(SolidColorBrush.ColorProperty, borderAnimation);
                }
            }

            // Riabilita il bottone dopo un breve ritardo
            DispatcherTimer enableButtonTimer = new DispatcherTimer();
            enableButtonTimer.Interval = TimeSpan.FromSeconds(4);
            enableButtonTimer.Tick += (s, args) =>
            {
                button.IsEnabled = true;
                enableButtonTimer.Stop();
            };
            enableButtonTimer.Start();
        }

        private void InitializeSlotMachineTimer()
        {
            slotMachineTimer = new DispatcherTimer();
            slotMachineTimer.Interval = TimeSpan.FromMilliseconds(80); // Velocità di cambio delle pizze
            slotMachineTimer.Tick += SlotMachineTimer_Tick;
        }

        private void SlotMachineTimer_Tick(object sender, EventArgs e)
        {

            TextBlock pizzaNameText = (TextBlock)this.FindName("PizzaNameText");
            TextBlock pizzaIngredientsText = (TextBlock)this.FindName("PizzaIngredientsText");
            TextBlock pizzaPriceText = (TextBlock)this.FindName("PizzaPriceText");

            // Selezione casuale durante l'animazione
            int randomIndex = random.Next(pizzaMenu.Count);
            Pizza randomPizza = pizzaMenu[randomIndex];

            // Aggiorna i testi con la pizza casuale
            pizzaNameText.Text = randomPizza.Name;

            // Rallenteam l'animazione verso la fine
            if (slotMachineCounter > SLOT_MACHINE_ITERATIONS * 0.7)
            {
                slotMachineTimer.Interval = TimeSpan.FromMilliseconds(slotMachineTimer.Interval.TotalMilliseconds + 15);
            }

            // Verifica se terminiamo l'animazione dello slot machine
            if (slotMachineCounter >= SLOT_MACHINE_ITERATIONS)
            {
                slotMachineTimer.Stop();
                PlayWinSound();
                slotMachineCounter = 0;

                // Resetta l'intervallo per la prossima volta
                slotMachineTimer.Interval = TimeSpan.FromMilliseconds(80);

                // Esegui la dissolvenza con rivelazione carattere per carattere
                RevealTextCharByChar();
            }
            else
            {
                // Incrementa contatore per slot machine
                slotMachineCounter++;

                // Aggiungi effetto di rotazione
                DoubleAnimation rotateAnimation = new DoubleAnimation
                {
                    From = -2,
                    To = 2,
                    Duration = TimeSpan.FromMilliseconds(100),
                    AutoReverse = true
                };

                RotateTransform rotateTransform = new RotateTransform();
                pizzaNameText.RenderTransform = rotateTransform;
                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
            }
        }

        private async void RevealTextCharByChar()
        {
            TextBlock pizzaNameText = (TextBlock)this.FindName("PizzaNameText");
            TextBlock pizzaIngredientsText = (TextBlock)this.FindName("PizzaIngredientsText");
            TextBlock pizzaPriceText = (TextBlock)this.FindName("PizzaPriceText");

            string fullPizzaName = selectedPizza.Name;
            string fullIngredients = selectedPizza.Ingredients;
            string fullPrice = $"€ {selectedPizza.Price:F2}";

            // Nascondi tutto il testo all'inizio
            pizzaNameText.Text = "";
            pizzaIngredientsText.Text = "";
            pizzaPriceText.Text = "";

            // Applica un effetto di tremolio prima della rivelazione
            ApplyShakeEffect(pizzaNameText);

            // Rivela il nome della pizza carattere per carattere con un effetto di tremolio
            for (int i = 0; i < fullPizzaName.Length; i++)
            {
                pizzaNameText.Text = fullPizzaName.Substring(0, i + 1);
                await Task.Delay(CHARACTER_REVEAL_DELAY_MS);
            }

            // Applica un effetto highlight al nome completo
            ApplyHighlightEffect(pizzaNameText);

            // Rivela gli ingredienti con una dissolvenza
            await Task.Delay(200);
            DoubleAnimation fadeInIngredients = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(500)
            };
            pizzaIngredientsText.Text = fullIngredients;
            pizzaIngredientsText.Opacity = 0;
            pizzaIngredientsText.BeginAnimation(UIElement.OpacityProperty, fadeInIngredients);

            // Rivela il prezzo con un'animazione di crescita
            await Task.Delay(300);
            DoubleAnimation scaleUpPrice = new DoubleAnimation
            {
                From = 0.5,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            ScaleTransform scaleTransform = new ScaleTransform(1, 1);
            pizzaPriceText.RenderTransform = scaleTransform;
            pizzaPriceText.Text = fullPrice;
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleUpPrice);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleUpPrice);
        }

        private void ApplyShakeEffect(TextBlock textBlock)
        {
            // Crea un'animazione di tremolio
            DoubleAnimation shakeAnimation = new DoubleAnimation
            {
                From = -3,
                To = 3,
                Duration = TimeSpan.FromMilliseconds(50),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(5)
            };

            TranslateTransform translateTransform = new TranslateTransform();
            textBlock.RenderTransform = translateTransform;
            translateTransform.BeginAnimation(TranslateTransform.XProperty, shakeAnimation);
        }

        private void ApplyHighlightEffect(TextBlock textBlock)
        {
            // Crea un'animazione di evidenziazione
            ColorAnimation highlightAnimation = new ColorAnimation
            {
                From = Colors.Red,
                To = Colors.Black,
                Duration = TimeSpan.FromMilliseconds(500)
            };

            SolidColorBrush textBrush = new SolidColorBrush(Colors.Black);
            textBlock.Foreground = textBrush;
            textBrush.BeginAnimation(SolidColorBrush.ColorProperty, highlightAnimation);
        }

        // Event handler for menu button click
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Create and show the menu window
            MenuWindow menuWindow = new MenuWindow(pizzaMenu);
            menuWindow.Owner = this;
            menuWindow.ShowDialog();
        }

        private void CreateLogoHeader(Grid mainGrid)
        {
            // Pannello principale per l'header
            Grid headerGrid = new Grid();
            headerGrid.Margin = new Thickness(5);

            // Definiamo due colonne: una per il logo e una per le informazioni di contatto
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            //headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });


            // Creiamo il bordo per il logo Mare e Monti (parte sinistra)
            Border logoBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(220, 240, 255)), // Azzurro chiaro come nell'immagine
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 102, 204)), // Blu del bordo
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(5)
            };

            // Contenuto del logo (StackPanel verticale)
            StackPanel logoContent = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Creiamo il pannello orizzontale per il logo testuale e l'icona del pesce
            StackPanel logoTextPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10, 5, 10, 0)
            };

            // Testo "MARE E MONTI"
            TextBlock mareMontiText = new TextBlock
            {
                Text = "MARE E MONTI",
                FontSize = 40,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204)), // Blu come nell'immagine
                VerticalAlignment = VerticalAlignment.Center
            };
            logoTextPanel.Children.Add(mareMontiText);


            // Creiamo l'icona del pesce usando Shapes
            Canvas fishIcon = new Canvas
            {
                Width = 60,
                Height = 30,
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            // Corpo del pesce
            System.Windows.Shapes.Path fishBody = new System.Windows.Shapes.Path
            {
                Fill = new SolidColorBrush(Color.FromRgb(0, 102, 204)), // Blu come nell'immagine
                Data = Geometry.Parse("M 0,15 C 5,5 20,0 40,15 C 20,30 5,25 0,15 Z") // Forma semplificata di pesce
            };
            fishIcon.Children.Add(fishBody);

            // Occhio del pesce
            Ellipse fishEye = new Ellipse
            {
                Width = 4,
                Height = 4,
                Fill = new SolidColorBrush(Colors.White),
                Margin = new Thickness(10, 13, 0, 0)
            };
            fishIcon.Children.Add(fishEye);

            // Coda del pesce
            System.Windows.Shapes.Path fishTail = new System.Windows.Shapes.Path
            {
                Fill = new SolidColorBrush(Color.FromRgb(0, 102, 204)), // Blu come nell'immagine
                Data = Geometry.Parse("M 40,15 L 55,5 L 55,25 Z") // Forma triangolare della coda
            };
            fishIcon.Children.Add(fishTail);

            logoTextPanel.Children.Add(fishIcon);
            logoContent.Children.Add(logoTextPanel);

            // Testo "PIZZERIA D'ASPORTO"
            TextBlock pizzeriaText = new TextBlock
            {
                Text = "PIZZERIA D'ASPORTO",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204)), // Blu come nell'immagine
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 5)
            };
            logoContent.Children.Add(pizzeriaText);

            // Pannello orizzontale con sole + telefono
            StackPanel sunAndPhonePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 10)
            };

            // Sole
            Canvas sunIcon = new Canvas
            {
                Width = 30,
                Height = 30,
                Margin = new Thickness(0, 0, 10, 0) // Spazio tra sole e telefono
            };

            Ellipse sunCircle = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = new SolidColorBrush(Color.FromRgb(255, 60, 50)),
            };
            Canvas.SetLeft(sunCircle, 5);
            Canvas.SetTop(sunCircle, 5);
            sunIcon.Children.Add(sunCircle);

            // Raggi del sole
            for (int i = 0; i < 8; i++)
            {
                Line ray = new Line
                {
                    X1 = 15,
                    Y1 = 15,
                    X2 = 15 + 12 * Math.Cos(i * Math.PI / 4),
                    Y2 = 15 + 12 * Math.Sin(i * Math.PI / 4),
                    Stroke = new SolidColorBrush(Color.FromRgb(255, 60, 50)),
                    StrokeThickness = 2
                };
                sunIcon.Children.Add(ray);
            }
            sunAndPhonePanel.Children.Add(sunIcon);

            // Numero di telefono a destra del sole
            TextBlock phoneNumber = new TextBlock
            {
                Text = "Tel. 035 904800",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(220, 50, 50)),
                VerticalAlignment = VerticalAlignment.Center
            };
            sunAndPhonePanel.Children.Add(phoneNumber);

            // Aggiungiamo tutto al contenuto principale del logo
            logoContent.Children.Add(sunAndPhonePanel);
            logoBorder.Child = logoContent;


            Grid.SetColumn(logoBorder, 0);
            headerGrid.Children.Add(logoBorder);

            Grid.SetRow(headerGrid, 0);
            mainGrid.Children.Add(headerGrid);
        }

        private void PlayWinSound()
        {
            try
            {
                if (spinSoundPlayer != null)
                {
                    spinSoundPlayer.Stop();
                }

                // Inizializza il player per il suono di vittoria se necessario
                if (winSoundPlayer == null)
                {
                    winSoundPlayer = new SoundPlayer("Resources/game_win_success.wav");
                }

                winSoundPlayer.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la riproduzione del suono: {ex.Message}");
            }

        }

        private void PlaySpinSound()
        {
            try
            {
                // Inizializza il player per il suono di spin se necessario
                if (spinSoundPlayer == null)
                {
                    spinSoundPlayer = new SoundPlayer("Resources/classic-slot-machine.wav");
                }

                spinSoundPlayer.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la riproduzione del suono: {ex.Message}");

            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Pulizia delle risorse audio
            if (spinSoundPlayer != null)
            {
                spinSoundPlayer.Dispose();
                spinSoundPlayer = null;
            }

            if (winSoundPlayer != null)
            {
                winSoundPlayer.Dispose();
                winSoundPlayer = null;
            }
        }
    }
}