using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MariEMontiRandomPizza
{
    /// <summary>
    /// Class to represent a menu window that displays all pizzas with filtering and sorting options
    /// </summary>
    public partial class MenuWindow : Window
    {
        private List<Pizza> _allPizzas;
        private List<Pizza> _filteredPizzas;
        private WrapPanel _pizzaPanel;
        private ComboBox _sortComboBox;
        private ComboBox _priceRangeComboBox;
        private ComboBox _typeComboBox;
        private TextBox _searchIngredientTextBox;
        private CheckBox _excludeIngredientsCheckBox;

        public MenuWindow(List<Pizza> pizzaMenu)
        {
            _allPizzas = pizzaMenu;
            _filteredPizzas = new List<Pizza>(_allPizzas);

            this.Title = "Menu Completo - Mare e Monti";
            this.Width = 1400;
            this.Height = 900;

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
            this.ResizeMode = ResizeMode.CanResize;

            // Create the main container
            Grid mainGrid = new Grid();
            mainGrid.Background = new SolidColorBrush(Color.FromRgb(240, 245, 255)); // Light blue background

            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) }); // Header
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(120) }); // Filter options
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

            // Create filter options section
            Border filterBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(250, 250, 250)),
                BorderThickness = new Thickness(0, 1, 0, 1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                Padding = new Thickness(10)
            };

            Grid filterGrid = new Grid();
            filterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            filterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            filterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            filterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            filterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Sorting options
            StackPanel sortPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };

            TextBlock sortLabel = new TextBlock
            {
                Text = "Ordina per:",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            };

            _sortComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 5)
            };
            _sortComboBox.Items.Add("Nome (A-Z)");
            _sortComboBox.Items.Add("Nome (Z-A)");
            _sortComboBox.Items.Add("Prezzo (crescente)");
            _sortComboBox.Items.Add("Prezzo (decrescente)");
            _sortComboBox.SelectedIndex = 0;
            _sortComboBox.SelectionChanged += FilterAndSort;

            sortPanel.Children.Add(sortLabel);
            sortPanel.Children.Add(_sortComboBox);

            Grid.SetColumn(sortPanel, 0);
            filterGrid.Children.Add(sortPanel);

            // Price range filter
            StackPanel pricePanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };

            TextBlock priceLabel = new TextBlock
            {
                Text = "Fascia di prezzo:",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            };

            _priceRangeComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 5)
            };
            _priceRangeComboBox.Items.Add("Tutti i prezzi");
            _priceRangeComboBox.Items.Add("Fino a €5");
            _priceRangeComboBox.Items.Add("€5 - €8");
            _priceRangeComboBox.Items.Add("€8 - €12");
            _priceRangeComboBox.Items.Add("Più di €12");
            _priceRangeComboBox.SelectedIndex = 0;
            _priceRangeComboBox.SelectionChanged += FilterAndSort;

            pricePanel.Children.Add(priceLabel);
            pricePanel.Children.Add(_priceRangeComboBox);

            Grid.SetColumn(pricePanel, 1);
            filterGrid.Children.Add(pricePanel);

            // Type filter
            StackPanel typePanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };

            TextBlock typeLabel = new TextBlock
            {
                Text = "Tipo di pizza:",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            };

            _typeComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 5)
            };
            _typeComboBox.Items.Add("Tutti i tipi");
            _typeComboBox.Items.Add("Pizza");
            _typeComboBox.Items.Add("Calzone");
            _typeComboBox.Items.Add("Pizza Bianca");
            _typeComboBox.SelectedIndex = 0;
            _typeComboBox.SelectionChanged += FilterAndSort;

            typePanel.Children.Add(typeLabel);
            typePanel.Children.Add(_typeComboBox);

            Grid.SetColumn(typePanel, 2);
            filterGrid.Children.Add(typePanel);

            // Ingredient filter
            StackPanel ingredientPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };

            TextBlock ingredientLabel = new TextBlock
            {
                Text = "Filtra per ingredienti:",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            };

            _searchIngredientTextBox = new TextBox
            {
                Margin = new Thickness(0, 0, 0, 5),
                Padding = new Thickness(5),
                Text = ""
            };
            _searchIngredientTextBox.TextChanged += FilterAndSort;

            ingredientPanel.Children.Add(ingredientLabel);
            ingredientPanel.Children.Add(_searchIngredientTextBox);

            Grid.SetColumn(ingredientPanel, 3);
            filterGrid.Children.Add(ingredientPanel);

            // Exclude/Include Ingredients Option
            StackPanel excludePanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };

            TextBlock excludeLabel = new TextBlock
            {
                Text = "Opzioni ingredienti:",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            };

            _excludeIngredientsCheckBox = new CheckBox
            {
                Content = "Escludi gli ingredienti",
                Margin = new Thickness(0, 5, 0, 0)
            };
            _excludeIngredientsCheckBox.Checked += FilterAndSort;
            _excludeIngredientsCheckBox.Unchecked += FilterAndSort;

            Button resetFiltersButton = new Button
            {
                Content = "Reimposta filtri",
                Margin = new Thickness(0, 10, 0, 0),
                Padding = new Thickness(5),
                Background = new SolidColorBrush(Color.FromRgb(0, 102, 204)),
                Foreground = new SolidColorBrush(Colors.White)
            };
            resetFiltersButton.Click += ResetFilters;

            excludePanel.Children.Add(excludeLabel);
            excludePanel.Children.Add(_excludeIngredientsCheckBox);
            excludePanel.Children.Add(resetFiltersButton);

            Grid.SetColumn(excludePanel, 4);
            filterGrid.Children.Add(excludePanel);

            filterBorder.Child = filterGrid;
            Grid.SetRow(filterBorder, 1);
            mainGrid.Children.Add(filterBorder);

            // Create a ScrollViewer to contain the pizza list
            ScrollViewer scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Margin = new Thickness(10)
            };

            // Create a WrapPanel to display pizzas in a grid-like fashion
            _pizzaPanel = new WrapPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Display initial pizzas
            UpdatePizzaDisplay();

            // Add wrap panel to the scroll viewer
            scrollViewer.Content = _pizzaPanel;

            // Add scroll viewer to the grid
            Grid.SetRow(scrollViewer, 2);
            mainGrid.Children.Add(scrollViewer);

            // Create footer with close button
            Border footerBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                BorderThickness = new Thickness(0, 1, 0, 0),
                BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 200))
            };

            StackPanel footerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            _pizzaCountText = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10),
                FontWeight = FontWeights.Bold,
                Text = $"Pizze visualizzate: {_filteredPizzas.Count} / {_allPizzas.Count}"
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

            footerPanel.Children.Add(_pizzaCountText);
            footerPanel.Children.Add(closeButton);
            footerBorder.Child = footerPanel;

            Grid.SetRow(footerBorder, 3);
            mainGrid.Children.Add(footerBorder);

            // Set the content of the window
            this.Content = mainGrid;
        }

        private void ResetFilters(object sender, RoutedEventArgs e)
        {
            // Reset all filters to default state
            _sortComboBox.SelectedIndex = 0;
            _priceRangeComboBox.SelectedIndex = 0;
            _typeComboBox.SelectedIndex = 0;
            _searchIngredientTextBox.Text = "";
            _excludeIngredientsCheckBox.IsChecked = false;

            // Apply the reset filters
            FilterAndSort(null, null);
        }

        private void FilterAndSort(object sender, RoutedEventArgs e)
        {
            // Apply all filters
            _filteredPizzas = new List<Pizza>(_allPizzas);

            // Apply price range filter
            switch (_priceRangeComboBox.SelectedIndex)
            {
                case 1: // Fino a €5
                    _filteredPizzas = _filteredPizzas.Where(p => p.Price <= 5).ToList();
                    break;
                case 2: // €5 - €8
                    _filteredPizzas = _filteredPizzas.Where(p => p.Price > 5 && p.Price <= 8).ToList();
                    break;
                case 3: // €8 - €12
                    _filteredPizzas = _filteredPizzas.Where(p => p.Price > 8 && p.Price <= 12).ToList();
                    break;
                case 4: // Più di €12
                    _filteredPizzas = _filteredPizzas.Where(p => p.Price > 12).ToList();
                    break;
            }

            // Apply type filter
            if (_typeComboBox.SelectedIndex > 0)
            {
                string selectedType = (string)_typeComboBox.SelectedItem;
                if (selectedType == "Pizza")
                {
                    _filteredPizzas = _filteredPizzas.Where(p => !p.Name.ToLower().Contains("calzone") && !p.Name.ToLower().Contains("bianca")).ToList();
                }
                else
                {
                    _filteredPizzas = _filteredPizzas.Where(p => p.Name.ToLower().Contains(selectedType.ToLower()) ||
                                                              p.Ingredients.ToLower().Contains(selectedType.ToLower())).ToList();
                }
            }

            // Apply ingredient filter
            string ingredientFilter = _searchIngredientTextBox.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(ingredientFilter))
            {
                bool excludeIngredients = _excludeIngredientsCheckBox.IsChecked ?? false;

                // Split the filter text to allow multiple ingredients (comma-separated)
                string[] ingredients = ingredientFilter.Split(',').Select(i => i.Trim()).Where(i => !string.IsNullOrEmpty(i)).ToArray();

                if (excludeIngredients)
                {
                    // Exclude pizzas with specified ingredients
                    foreach (string ingredient in ingredients)
                    {
                        _filteredPizzas = _filteredPizzas.Where(p =>
                            !p.Ingredients.ToLower().Contains(ingredient) &&
                            !p.Name.ToLower().Contains(ingredient)).ToList();
                    }
                }
                else
                {
                    // Include pizzas with specified ingredients
                    List<Pizza> result = new List<Pizza>();

                    // If any ingredient matches in ingredients or name, include the pizza
                    foreach (string ingredient in ingredients)
                    {
                        result.AddRange(_filteredPizzas.Where(p =>
                            p.Ingredients.ToLower().Contains(ingredient) ||
                            p.Name.ToLower().Contains(ingredient)));
                    }

                    _filteredPizzas = result.Distinct().ToList();
                }
            }

            // Apply sorting
            switch (_sortComboBox.SelectedIndex)
            {
                case 0: // Nome (A-Z)
                    _filteredPizzas = _filteredPizzas.OrderBy(p => p.Name).ToList();
                    break;
                case 1: // Nome (Z-A)
                    _filteredPizzas = _filteredPizzas.OrderByDescending(p => p.Name).ToList();
                    break;
                case 2: // Prezzo (crescente)
                    _filteredPizzas = _filteredPizzas.OrderBy(p => p.Price).ToList();
                    break;
                case 3: // Prezzo (decrescente)
                    _filteredPizzas = _filteredPizzas.OrderByDescending(p => p.Price).ToList();
                    break;
            }

            // Update the display
            UpdatePizzaDisplay();
        }

        private TextBlock _pizzaCountText;

        private void UpdatePizzaDisplay()
        {
            // Clear the current display
            _pizzaPanel.Children.Clear();

            foreach (var pizza in _filteredPizzas)
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
                _pizzaPanel.Children.Add(pizzaBorder);
            }

            // Update the pizza count text in the footer
            if (_pizzaCountText != null)
            {
                _pizzaCountText.Text = $"Pizze visualizzate: {_filteredPizzas.Count} / {_allPizzas.Count}";
            }
        }
    }
}