using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        // Carrello
        //private Dictionary<Pizza, int> _cart = new Dictionary<Pizza, int>();
        private TextBlock _cartTotalText;
        private TextBlock _cartItemCountText;
        private Border _cartSummaryBorder;
        private StackPanel _cartItemsPanel;
        private Expander _cartExpander;

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

            Loaded += MyWindow_Loaded;

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
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Cart summary

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
                Margin = new Thickness(0, 0, 0, 5),
                Foreground = new SolidColorBrush(Color.FromRgb(220, 50, 50)) // Rosso come nel logo
            };

            _sortComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 5),
                Padding = new Thickness(5),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 102, 204)), // Blu come nel logo
                BorderThickness = new Thickness(1),
                Background = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("Arial"),
                FontWeight = FontWeights.Bold
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
                Margin = new Thickness(0, 0, 0, 5),
                Foreground = new SolidColorBrush(Color.FromRgb(220, 50, 50)) // Rosso come nel logo
            };

            _priceRangeComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 5),
                Padding = new Thickness(5),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 102, 204)), // Blu come nel logo
                BorderThickness = new Thickness(1),
                Background = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("Arial"),
                FontWeight = FontWeights.Bold
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
                Margin = new Thickness(0, 0, 0, 5),
                Foreground = new SolidColorBrush(Color.FromRgb(220, 50, 50)) // Rosso come nel logo
            };

            _typeComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 0, 5),
                Padding = new Thickness(5),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 102, 204)), // Blu come nel logo
                BorderThickness = new Thickness(1),
                Background = new SolidColorBrush(Colors.White),
                FontFamily = new FontFamily("Arial"),
                FontWeight = FontWeights.Bold
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
                Text = "Ricerca:",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5),
                Foreground = new SolidColorBrush(Color.FromRgb(220, 50, 50)) // Rosso come nel logo
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
                Text = "Opzioni ricerca:",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5),
                Foreground = new SolidColorBrush(Color.FromRgb(220, 50, 50)) // Rosso come nel logo
            };

            _excludeIngredientsCheckBox = new CheckBox
            {
                Content = "Escludi gli ingredienti",
                Margin = new Thickness(0, 5, 0, 0),
                Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204)), // Blu come nel logo
                FontFamily = new FontFamily("Arial")
            };
            _excludeIngredientsCheckBox.Checked += FilterAndSort;
            _excludeIngredientsCheckBox.Unchecked += FilterAndSort;

            Button resetFiltersButton = new Button
            {
                Content = "Reimposta filtri",
                Margin = new Thickness(0, 10, 0, 0),
                Padding = new Thickness(5),
                Background = new SolidColorBrush(Color.FromRgb(0, 102, 204)), // Blu come nel logo
                Foreground = new SolidColorBrush(Colors.White),
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Arial"),
                BorderThickness = new Thickness(0)
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

            // Create footer with close button and pizza count
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

            // Create cart summary section
            CreateCartSummary(mainGrid);

            // Set the content of the window
            this.Content = mainGrid;
        }

        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.Cart.Count > 0)
            {
                // Show cart if it was hidden
                if (_cartSummaryBorder.Visibility == Visibility.Collapsed)
                {
                    _cartSummaryBorder.Visibility = Visibility.Visible;
                }

                // Expand the cart when cart is not empty
                if (App.Cart.Count > 0)
                {
                    _cartExpander.IsExpanded = true;
                }

                UpdateCartUI();
            }
        }

        private void CreateCartSummary(Grid mainGrid)
        {
            // Create cart summary border
            _cartSummaryBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(245, 245, 255)),
                BorderThickness = new Thickness(0, 1, 0, 0),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 102, 204)),
                Padding = new Thickness(5)
                // Rimosso MaxHeight per evitare limitazioni
            };

            _cartExpander = new Expander
            {
                IsExpanded = false,
                Padding = new Thickness(5)
            };

            // Header for the expander
            StackPanel expanderHeader = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            Image cartIcon = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Images/icons8-cart-32.png")),
                Width = 29,
                Height = 29,
                Margin = new Thickness(0, 0, 5, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            _cartItemCountText = new TextBlock
            {
                Text = "Carrello (0 pizze)",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204))
            };

            _cartTotalText = new TextBlock
            {
                Text = "Totale: € 0.00",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromRgb(220, 50, 50))
            };

            expanderHeader.Children.Add(cartIcon);
            expanderHeader.Children.Add(_cartItemCountText);
            expanderHeader.Children.Add(_cartTotalText);

            _cartExpander.Header = expanderHeader;

            // Create ScrollViewer for cart items
            ScrollViewer cartScrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                MaxHeight = 200  // Limita l'altezza solo dello ScrollViewer, non dell'intero border
            };

            _cartItemsPanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            cartScrollViewer.Content = _cartItemsPanel;
            _cartExpander.Content = cartScrollViewer;

            // Button Panel for cart actions - Ora separato dal contenuto espandibile
            StackPanel cartButtonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 5, 5, 0)
            };

            Button clearCartButton = new Button
            {
                Content = "Svuota Carrello",
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(5, 0, 0, 0),
                Background = new SolidColorBrush(Color.FromRgb(220, 50, 50)),
                Foreground = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(0)
            };
            clearCartButton.Click += ClearCart;

            cartButtonPanel.Children.Add(clearCartButton);

            // Stack panel for everything - Il pulsante è ora fuori dall'expander
            StackPanel cartContainer = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            cartContainer.Children.Add(_cartExpander);
            cartContainer.Children.Add(cartButtonPanel);  // Aggiunto dopo l'expander

            _cartSummaryBorder.Child = cartContainer;
            Grid.SetRow(_cartSummaryBorder, 4);
            mainGrid.Children.Add(_cartSummaryBorder);

            // Initially hide cart if empty
            _cartSummaryBorder.Visibility = Visibility.Collapsed;
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
                else if (selectedType == "Calzone")
                {
                    _filteredPizzas = _filteredPizzas.Where(p => p.Name.ToLower().Contains(selectedType.ToLower()) ||
                                                              p.Ingredients.ToLower().Contains(selectedType.ToLower())).ToList();
                }
                else if (selectedType == "Pizza Bianca")
                {
                    _filteredPizzas = _filteredPizzas.Where(p => p.Name.ToLower().Contains(selectedType.ToLower()) ||
                                                              p.Ingredients.ToLower().Contains(selectedType.ToLower())
                                                              || p.Ingredients.ToLower().Contains("bianca")).ToList();
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
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0, 102, 204)), // Blu come nel logo
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
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = new SolidColorBrush(Color.FromRgb(220, 50, 50)), // Rosso come nel logo
                };

                // Pizza ingredients
                TextBlock ingredientsText = new TextBlock
                {
                    Text = pizza.Ingredients,
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 5),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextAlignment = TextAlignment.Center,
                    Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204)) // Blu come nel logo
                };

                // Pizza price
                TextBlock priceText = new TextBlock
                {
                    Text = $"€ {pizza.Price:F2}",
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204)) // Blu come nel logo
                };

                // Add cart button with icon
                Button addToCartButton = CreateCartButton(pizza);

                // Add elements to the stack panel
                pizzaDetails.Children.Add(nameText);
                pizzaDetails.Children.Add(ingredientsText);
                pizzaDetails.Children.Add(priceText);
                pizzaDetails.Children.Add(addToCartButton);

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

        private Button CreateCartButton(Pizza pizza)
        {
            // Create a StackPanel for the button content
            StackPanel buttonContent = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Create the cart icon
            Image cartIcon = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Images/icons8-cart-32.png")),
                Width = 22,
                Height = 22,
                Margin = new Thickness(0, 0, 5, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            // Create the text for the button
            TextBlock buttonText = new TextBlock
            {
                Text = "",
                Foreground = new SolidColorBrush(Colors.White),
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 12
            };

            buttonContent.Children.Add(cartIcon);
            buttonContent.Children.Add(buttonText);

            // Create the button
            Button addToCartButton = new Button
            {
                Content = buttonContent,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(0, 8, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            addToCartButton.Click += (sender, e) => AddToCart(pizza);

            return addToCartButton;
        }

        private void AddToCart(Pizza pizza)
        {
            
            // Add pizza to cart or increment its quantity
            if (App.Cart.ContainsKey(pizza))
            {
                App.Cart[pizza]++;
            }
            else
            {
                App.Cart.Add(pizza, 1);
            }

            // Show cart if it was hidden
            if (_cartSummaryBorder.Visibility == Visibility.Collapsed)
            {
                _cartSummaryBorder.Visibility = Visibility.Visible;
            }

            // Expand the cart expander if this is the first item
            if (App.Cart.Count == 1)
            {
                _cartExpander.IsExpanded = true;
            }

            // Update the cart UI
            UpdateCartUI();

            // Show feedback
            //ShowAddToCartFeedback(pizza);
        }

        //private void ShowAddToCartFeedback(Pizza pizza)
        //{
        //    // Create a popup window or toast notification
        //    Window feedbackWindow = new Window
        //    {
        //        Width = 250,
        //        Height = 100,
        //        WindowStyle = WindowStyle.None,
        //        AllowsTransparency = true,
        //        Background = new SolidColorBrush(Colors.Transparent),
        //        Topmost = true,
        //        ResizeMode = ResizeMode.NoResize,
        //        ShowInTaskbar = false,
        //        Owner = this
        //    };

        //    // Create border with rounded corners
        //    Border feedbackBorder = new Border
        //    {
        //        Background = new SolidColorBrush(Color.FromArgb(230, 0, 102, 204)), // Semi-transparent blue
        //        CornerRadius = new CornerRadius(8),
        //        Padding = new Thickness(15)
        //    };

        //    // Create content
        //    StackPanel feedbackContent = new StackPanel
        //    {
        //        Orientation = Orientation.Vertical
        //    };

        //    TextBlock messageText = new TextBlock
        //    {
        //        Text = $"{pizza.Name}",
        //        Foreground = new SolidColorBrush(Colors.White),
        //        FontWeight = FontWeights.Bold,
        //        TextWrapping = TextWrapping.Wrap,
        //        HorizontalAlignment = HorizontalAlignment.Center
        //    };

        //    TextBlock addedText = new TextBlock
        //    {
        //        Text = "Aggiunta al carrello!",
        //        Foreground = new SolidColorBrush(Colors.White),
        //        HorizontalAlignment = HorizontalAlignment.Center,
        //        Margin = new Thickness(0, 5, 0, 0)
        //    };

        //    feedbackContent.Children.Add(messageText);
        //    feedbackContent.Children.Add(addedText);
        //    feedbackBorder.Child = feedbackContent;
        //    feedbackWindow.Content = feedbackBorder;

        //    // Position the feedback window in the bottom right
        //    feedbackWindow.WindowStartupLocation = WindowStartupLocation.Manual;
        //    feedbackWindow.Left = this.Left + this.Width - feedbackWindow.Width - 200;
        //    feedbackWindow.Top = this.Top + this.Height - feedbackWindow.Height - 200;

        //    // Show the feedback window
        //    feedbackWindow.Show();

        //    // Close the feedback window after 1.5 seconds
        //    System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        //    timer.Interval = TimeSpan.FromSeconds(1.0);
        //    timer.Tick += (sender, e) =>
        //    {
        //        feedbackWindow.Close();
        //        timer.Stop();
        //    };
        //    timer.Start();
        //}

        private void UpdateCartUI()
        {
            _cartItemsPanel.Children.Clear();

            // Calculate total price
            double totalPrice = 0;
            int totalItems = 0;

            foreach (var item in App.Cart)
            {
                Pizza pizza = item.Key;
                int quantity = item.Value;
                double itemTotal = (double)(pizza.Price * quantity);
                totalPrice += itemTotal;
                totalItems += quantity;

                // Create item row
                Border itemBorder = new Border
                {
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220)),
                    Padding = new Thickness(5)
                };

                Grid itemGrid = new Grid();
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(65) });
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
                itemGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(25) });

                // Pizza name
                TextBlock nameText = new TextBlock
                {
                    Text = pizza.Name,
                    FontWeight = FontWeights.Bold,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204))
                };
                Grid.SetColumn(nameText, 0);

                // Quantity controls
                StackPanel quantityPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                Button decrementButton = new Button
                {
                    Content = "-",
                    Width = 20,
                    Height = 20,
                    Padding = new Thickness(0),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Background = new SolidColorBrush(Color.FromRgb(240, 240, 240))
                };
                decrementButton.Click += (sender, e) => DecrementQuantity(pizza);

                TextBlock quantityText = new TextBlock
                {
                    Text = quantity.ToString(),
                    Width = 20,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(2, 0, 2, 0)
                };

                Button incrementButton = new Button
                {
                    Content = "+",
                    Width = 20,
                    Height = 20,
                    Padding = new Thickness(0),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Background = new SolidColorBrush(Color.FromRgb(240, 240, 240))
                };
                incrementButton.Click += (sender, e) => AddToCart(pizza);

                quantityPanel.Children.Add(decrementButton);
                quantityPanel.Children.Add(quantityText);
                quantityPanel.Children.Add(incrementButton);
                Grid.SetColumn(quantityPanel, 1);

                // Item total price
                TextBlock priceText = new TextBlock
                {
                    Text = $"€ {itemTotal:F2}",
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Right,
                    Margin = new Thickness(5, 0, 5, 0),
                    Foreground = new SolidColorBrush(Color.FromRgb(220, 50, 50))
                };
                Grid.SetColumn(priceText, 2);

                // Remove button
                Button removeButton = new Button
                {
                    Content = "✕",
                    Width = 20,
                    Height = 20,
                    Padding = new Thickness(0),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Background = new SolidColorBrush(Color.FromRgb(220, 50, 50)),
                    Foreground = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(0)
                };
                removeButton.Click += (sender, e) => RemoveFromCart(pizza);
                Grid.SetColumn(removeButton, 3);

                itemGrid.Children.Add(nameText);
                itemGrid.Children.Add(quantityPanel);
                itemGrid.Children.Add(priceText);
                itemGrid.Children.Add(removeButton);

                itemBorder.Child = itemGrid;
                _cartItemsPanel.Children.Add(itemBorder);
            }

            // Update summary info
            _cartItemCountText.Text = $"Carrello ({totalItems} {(totalItems == 1 ? "pizza" : "pizze")})";
            _cartTotalText.Text = $"Totale: € {totalPrice:F2}";


            //Update Main window cart
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.UpdateCartCounter();
        }

        private void DecrementQuantity(Pizza pizza)
        {
            if (App.Cart.ContainsKey(pizza))
            {
                App.Cart[pizza]--;

                // Remove item if quantity is 0
                if (App.Cart[pizza] <= 0)
                {
                    App.Cart.Remove(pizza);
                }

                // If cart is empty, hide it
                if (App.Cart.Count == 0)
                {
                    _cartSummaryBorder.Visibility = Visibility.Collapsed;
                }

                // Update the cart UI
                UpdateCartUI();
            }
        }

        private void RemoveFromCart(Pizza pizza)
        {
            if (App.Cart.ContainsKey(pizza))
            {
                App.Cart.Remove(pizza);

                // If cart is empty, hide it
                if (App.Cart.Count == 0)
                {
                    _cartSummaryBorder.Visibility = Visibility.Collapsed;
                }

                // Update the cart UI
                UpdateCartUI();
            }
        }

        private void ClearCart(object sender, RoutedEventArgs e)
        {
            // Clear the cart
            App.Cart.Clear();

            // Hide the cart summary
            _cartSummaryBorder.Visibility = Visibility.Collapsed;

            // Update the cart UI
            UpdateCartUI();
        }
    }
}