using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

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

        public MainWindow()
        {
            InitializeComponent();
            InitializeGUI();
            LoadPizzaMenu();
        }

        private void LoadPizzaMenu()
        {
            try
            {
                // Percorso del file CSV (nella cartella dell'applicazione)
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "menu-pizze.csv");

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
            this.Width = 800;
            this.Height = 600;
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

            // Create header with logo
            StackPanel headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Create logo text
            TextBlock logoText = new TextBlock
            {
                Text = "MARE E MONTI",
                FontSize = 48,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204)), // Blue color similar to the logo
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Center
            };

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

            // Create a border for the logo
            Border logoBorder = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 102, 204)),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(15),
                Margin = new Thickness(10),
                Child = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Children = { logoText, logoTagline, fishDecoration }
                }
            };

            headerPanel.Children.Add(logoBorder);

            // Set the header in the grid
            Grid.SetRow(headerPanel, 0);
            mainGrid.Children.Add(headerPanel);

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
        }

        // Event handler for random button click
        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            // Select a random pizza
            int randomIndex = random.Next(pizzaMenu.Count);
            selectedPizza = pizzaMenu[randomIndex];

            // Update the UI with the selected pizza
            TextBlock pizzaNameText = (TextBlock)this.FindName("PizzaNameText");
            TextBlock pizzaIngredientsText = (TextBlock)this.FindName("PizzaIngredientsText");
            TextBlock pizzaPriceText = (TextBlock)this.FindName("PizzaPriceText");

            // Adjust font size for long pizza names
            if (selectedPizza.Name.Length > 25)
            {
                pizzaNameText.FontSize = 20;
            }
            else if (selectedPizza.Name.Length > 15)
            {
                pizzaNameText.FontSize = 22;
            }
            else
            {
                pizzaNameText.FontSize = 24;
            }

            // Create animations for text update
            DoubleAnimation fadeOut = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            DoubleAnimation fadeIn = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            fadeOut.Completed += (s, args) =>
            {
                // Update text content
                pizzaNameText.Text = selectedPizza.Name;
                pizzaIngredientsText.Text = selectedPizza.Ingredients;
                pizzaPriceText.Text = $"€ {selectedPizza.Price:F2}";

                // Start fade in animation
                pizzaNameText.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                pizzaIngredientsText.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                pizzaPriceText.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            };

            // Start fade out animation
            pizzaNameText.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            pizzaIngredientsText.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            pizzaPriceText.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        // Event handler for menu button click
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Create and show the menu window
            MenuWindow menuWindow = new MenuWindow(pizzaMenu);
            menuWindow.Owner = this;
            menuWindow.ShowDialog();
        }
    }
}