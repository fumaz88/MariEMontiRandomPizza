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
using System.Windows.Shapes;

namespace MariEMontiRandomPizza
{
    /// <summary>
    /// Class to represent a menu window that displays all pizzas
    /// </summary>
    public partial class MenuWindow : Window
    {
        public MenuWindow(List<Pizza> pizzaMenu)
        {
            this.Title = "Menu Completo - Mare e Monti";
            this.Width = 1400;
            this.Height = 800;

            try
            {
                this.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/MariEMontiIcon.ico"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossibile caricare l'icona: " + ex.Message);
            }

            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.Background = new SolidColorBrush(Colors.White);
            this.ResizeMode = ResizeMode.CanMinimize;
            this.ResizeMode = ResizeMode.CanResize;

            // Create the main container
            Grid mainGrid = new Grid();
            mainGrid.Background = new SolidColorBrush(Color.FromRgb(240, 245, 255)); // Light blue background

            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) }); // Header
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Content
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) }); // Footer

            // Create header
            Border headerBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0, 102, 204)), // Blue color
                BorderThickness = new Thickness(0)
            };

            TextBlock headerText = new TextBlock
            {
                Text = "LE NOSTRE PIZZE",
                FontSize = 28,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            headerBorder.Child = headerText;
            Grid.SetRow(headerBorder, 0);
            mainGrid.Children.Add(headerBorder);

            // Create a ScrollViewer to contain the pizza list
            ScrollViewer scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Margin = new Thickness(10)
            };

            // Create a WrapPanel to display pizzas in a grid-like fashion
            WrapPanel pizzaPanel = new WrapPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };


            foreach (var pizza in pizzaMenu.OrderBy(p => p.Name))
            {
                // Create a border for each pizza
                Border pizzaBorder = new Border
                {
                    BorderBrush = new SolidColorBrush(Color.FromRgb(220, 50, 50)), // Red color
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(5),
                    Padding = new Thickness(8),
                    Width = 200,
                    Background = new SolidColorBrush(Colors.White)
                };

                // Create a stack panel for pizza details
                StackPanel pizzaDetails = new StackPanel
                {
                    Orientation = Orientation.Vertical
                };

                // Pizza name
                TextBlock nameText = new TextBlock
                {
                    Text = pizza.Name,
                    FontWeight = FontWeights.Bold,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 5),
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                // Pizza ingredients
                TextBlock ingredientsText = new TextBlock
                {
                    Text = pizza.Ingredients,
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 5),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextAlignment = TextAlignment.Center
                };

                // Pizza price
                TextBlock priceText = new TextBlock
                {
                    Text = $"€ {pizza.Price:F2}",
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(220, 50, 50)), // Red color
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                // Add elements to the stack panel
                pizzaDetails.Children.Add(nameText);
                pizzaDetails.Children.Add(ingredientsText);
                pizzaDetails.Children.Add(priceText);

                // Add stack panel to the border
                pizzaBorder.Child = pizzaDetails;

                // Add border to the wrap panel
                pizzaPanel.Children.Add(pizzaBorder);
            }

            // Add wrap panel to the scroll viewer
            scrollViewer.Content = pizzaPanel;

            // Add scroll viewer to the grid
            Grid.SetRow(scrollViewer, 1);
            mainGrid.Children.Add(scrollViewer);

            // Create footer with close button
            Border footerBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                BorderThickness = new Thickness(0, 1, 0, 0),
                BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200))
            };

            Button closeButton = new Button
            {
                Content = "CHIUDI",
                Width = 120,
                Height = 30,
                Margin = new Thickness(10),
                Background = new SolidColorBrush(Color.FromRgb(220, 50, 50)), // Red color
                Foreground = new SolidColorBrush(Colors.White),
                FontWeight = FontWeights.Bold
            };

            closeButton.Click += (sender, e) => this.Close();

            StackPanel footerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            footerPanel.Children.Add(closeButton);
            footerBorder.Child = footerPanel;

            Grid.SetRow(footerBorder, 2);
            mainGrid.Children.Add(footerBorder);

            // Set the content of the window
            this.Content = mainGrid;
        }
    }
}
