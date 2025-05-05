using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MariEMontiRandomPizza
{
    /// <summary>
    /// Interaction logic for PizzaManagerWindow.xaml
    /// </summary>
    public partial class PizzaManagerWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Pizza> _pizzaMenu;
        private Pizza _selectedPizza;
        private string _searchText = "";
        private string _csvFilePath;
        private bool _isEditing = false;
        private bool _hasUnsavedChanges = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Pizza> PizzaMenu
        {
            get { return _pizzaMenu; }
            set
            {
                _pizzaMenu = value;
                OnPropertyChanged(nameof(PizzaMenu));
            }
        }

        public Pizza SelectedPizza
        {
            get { return _selectedPizza; }
            set
            {
                _selectedPizza = value;
                OnPropertyChanged(nameof(SelectedPizza));
                
                // Aggiorna i campi di input quando una pizza viene selezionata
                if (_selectedPizza != null && !_isEditing)
                {
                    PizzaName = _selectedPizza.Name;
                    PizzaIngredients = _selectedPizza.Ingredients;
                    PizzaPrice = _selectedPizza.Price.ToString("0.00");
                }
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterPizzaList();
            }
        }

        public string PizzaName { get; set; }
        public string PizzaIngredients { get; set; }
        public string PizzaPrice { get; set; }

        public PizzaManagerWindow(List<Pizza> pizzaMenu, string csvFilePath)
        {
            InitializeComponent();
            DataContext = this;
            _csvFilePath = csvFilePath;

            // Crea una copia del menu per evitare modifiche dirette
            PizzaMenu = new ObservableCollection<Pizza>(pizzaMenu.Select(p => new Pizza
            {
                Name = p.Name,
                Ingredients = p.Ingredients,
                Price = p.Price
            }));

            CreateUI();
        }

        private void CreateUI()
        {
            this.Title = "Mare e Monti - Gestione Menu";
            this.Width = 900;
            this.Height = 600;
            this.Background = new SolidColorBrush(Colors.White);
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Main Grid
            Grid mainGrid = new Grid();
            mainGrid.Margin = new Thickness(20);

            // Define rows
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) }); // Header
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Content
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) }); // Footer

            // Header with title
            Border headerBorder = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 102, 204)),
                BorderThickness = new Thickness(0, 0, 0, 2),
                Padding = new Thickness(10, 5, 10, 5)
            };

            StackPanel headerPanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            TextBlock titleText = new TextBlock
            {
                Text = "GESTIONE MENU PIZZE",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204)),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 5)
            };

            // Aggiunta del campo di ricerca
            Grid searchGrid = new Grid();
            searchGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            searchGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            TextBlock searchLabel = new TextBlock
            {
                Text = "Cerca pizza:",
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(searchLabel, 0);

            TextBox searchBox = new TextBox
            {
                FontSize = 16,
                Padding = new Thickness(5),
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            searchBox.SetBinding(TextBox.TextProperty, new Binding("SearchText") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            Grid.SetColumn(searchBox, 1);

            searchGrid.Children.Add(searchLabel);
            searchGrid.Children.Add(searchBox);

            headerPanel.Children.Add(titleText);
            headerPanel.Children.Add(searchGrid);
            headerBorder.Child = headerPanel;
            Grid.SetRow(headerBorder, 0);
            mainGrid.Children.Add(headerBorder);

            // Content Grid with list and editing form
            Grid contentGrid = new Grid();
            contentGrid.Margin = new Thickness(0, 10, 0, 0);

            contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // List
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10) }); // Spacer
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Edit form

            // Pizza List
            Border listBorder = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(220, 50, 50)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5)
            };

            Grid listGrid = new Grid();
            listGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) }); // List header
            listGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // List content

            // List header
            Border listHeaderBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(220, 50, 50)),
                Padding = new Thickness(10, 5, 10, 5)
            };

            TextBlock listHeaderText = new TextBlock
            {
                Text = "ELENCO PIZZE",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            listHeaderBorder.Child = listHeaderText;
            Grid.SetRow(listHeaderBorder, 0);
            listGrid.Children.Add(listHeaderBorder);

            // List view
            ListView pizzaListView = new ListView
            {
                Margin = new Thickness(5),
                BorderThickness = new Thickness(0)
            };

            // Set binding for the list view
            pizzaListView.SetBinding(ListView.ItemsSourceProperty, new Binding("PizzaMenu"));
            pizzaListView.SetBinding(ListView.SelectedItemProperty, new Binding("SelectedPizza") { Mode = BindingMode.TwoWay });

            // Define the ListView's template
            GridView gridView = new GridView();
            
            // Define column for name
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Nome",
                DisplayMemberBinding = new Binding("Name"),
                Width = 250
            });

            // Define column for price
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Prezzo",
                DisplayMemberBinding = new Binding("PriceFormatted"),
                Width = 100
            });

            pizzaListView.View = gridView;
            Grid.SetRow(pizzaListView, 1);
            listGrid.Children.Add(pizzaListView);

            listBorder.Child = listGrid;
            Grid.SetColumn(listBorder, 0);
            contentGrid.Children.Add(listBorder);

            // Edit Form
            Border formBorder = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 102, 204)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5)
            };

            Grid formGrid = new Grid();
            formGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) }); // Form header
            formGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Form content

            // Form header
            Border formHeaderBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(0, 102, 204)),
                Padding = new Thickness(10, 5, 10, 5)
            };

            TextBlock formHeaderText = new TextBlock
            {
                Text = "DETTAGLI PIZZA",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            formHeaderBorder.Child = formHeaderText;
            Grid.SetRow(formHeaderBorder, 0);
            formGrid.Children.Add(formHeaderBorder);

            // Form fields
            StackPanel formPanel = new StackPanel
            {
                Margin = new Thickness(15),
                VerticalAlignment = VerticalAlignment.Top
            };

            // Nome pizza
            TextBlock nameLabel = new TextBlock
            {
                Text = "Nome Pizza:",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 5)
            };
            formPanel.Children.Add(nameLabel);

            TextBox nameTextBox = new TextBox
            {
                FontSize = 16,
                Padding = new Thickness(5),
                Margin = new Thickness(0, 0, 0, 15)
            };
            nameTextBox.SetBinding(TextBox.TextProperty, new Binding("PizzaName") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            nameTextBox.TextChanged += (s, e) => { _hasUnsavedChanges = true; };
            formPanel.Children.Add(nameTextBox);

            // Ingredienti
            TextBlock ingredientsLabel = new TextBlock
            {
                Text = "Ingredienti:",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 5)
            };
            formPanel.Children.Add(ingredientsLabel);

            TextBox ingredientsTextBox = new TextBox
            {
                FontSize = 16,
                Padding = new Thickness(5),
                Margin = new Thickness(0, 0, 0, 15),
                MinHeight = 80,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true
            };
            ingredientsTextBox.SetBinding(TextBox.TextProperty, new Binding("PizzaIngredients") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            ingredientsTextBox.TextChanged += (s, e) => { _hasUnsavedChanges = true; };
            formPanel.Children.Add(ingredientsTextBox);

            // Prezzo
            TextBlock priceLabel = new TextBlock
            {
                Text = "Prezzo (€):",
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 5)
            };
            formPanel.Children.Add(priceLabel);

            TextBox priceTextBox = new TextBox
            {
                FontSize = 16,
                Padding = new Thickness(5),
                Margin = new Thickness(0, 0, 0, 15),
                MaxWidth = 150,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            priceTextBox.SetBinding(TextBox.TextProperty, new Binding("PizzaPrice") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            priceTextBox.TextChanged += (s, e) => { _hasUnsavedChanges = true; };
            formPanel.Children.Add(priceTextBox);

            // Buttons for edit operations
            StackPanel buttonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 20, 0, 0)
            };

            Button newButton = new Button
            {
                Content = "NUOVA PIZZA",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromRgb(0, 102, 204)),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(15, 8, 15, 8),
                Margin = new Thickness(0, 0, 10, 0)
            };
            newButton.Click += NewButton_Click;
            buttonsPanel.Children.Add(newButton);

            Button saveButton = new Button
            {
                Content = "SALVA",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromRgb(50, 150, 50)),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(15, 8, 15, 8),
                Margin = new Thickness(0, 0, 10, 0)
            };
            saveButton.Click += SaveButton_Click;
            buttonsPanel.Children.Add(saveButton);

            Button deleteButton = new Button
            {
                Content = "ELIMINA",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromRgb(220, 50, 50)),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(15, 8, 15, 8)
            };
            deleteButton.Click += DeleteButton_Click;
            buttonsPanel.Children.Add(deleteButton);

            formPanel.Children.Add(buttonsPanel);
            Grid.SetRow(formPanel, 1);
            formGrid.Children.Add(formPanel);

            formBorder.Child = formGrid;
            Grid.SetColumn(formBorder, 2);
            contentGrid.Children.Add(formBorder);

            Grid.SetRow(contentGrid, 1);
            mainGrid.Children.Add(contentGrid);

            // Footer with save buttons
            Border footerBorder = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 102, 204)),
                BorderThickness = new Thickness(0, 2, 0, 0),
                Padding = new Thickness(10, 10, 10, 10)
            };

            StackPanel footerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            Button saveAllButton = new Button
            {
                Content = "SALVA MENU",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromRgb(0, 102, 204)),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(20, 8, 20, 8),
                Margin = new Thickness(0, 0, 10, 0)
            };
            saveAllButton.Click += SaveAllButton_Click;
            footerPanel.Children.Add(saveAllButton);

            Button closeButton = new Button
            {
                Content = "CHIUDI",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromRgb(150, 150, 150)),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(20, 8, 20, 8)
            };
            closeButton.Click += CloseButton_Click;
            footerPanel.Children.Add(closeButton);

            footerBorder.Child = footerPanel;
            Grid.SetRow(footerBorder, 2);
            mainGrid.Children.Add(footerBorder);

            // Set the content
            this.Content = mainGrid;
        }

        private void FilterPizzaList()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(PizzaMenu);
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                view.Filter = null;
            }
            else
            {
                view.Filter = (item) =>
                {
                    Pizza pizza = item as Pizza;
                    return pizza != null && 
                           (pizza.Name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            (pizza.Ingredients != null && pizza.Ingredients.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0));
                };
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            // Check for unsaved changes
            if (_hasUnsavedChanges && SelectedPizza != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Ci sono modifiche non salvate. Vuoi continuare?",
                    "Modifiche non salvate",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            // Clear selection and form for new pizza
            SelectedPizza = null;
            PizzaName = "";
            PizzaIngredients = "";
            PizzaPrice = "0.00";
            _isEditing = true;
            _hasUnsavedChanges = false;

            // Notifica le proprietà
            OnPropertyChanged(nameof(PizzaName));
            OnPropertyChanged(nameof(PizzaIngredients));
            OnPropertyChanged(nameof(PizzaPrice));
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(PizzaName))
            {
                ShowError("Il nome della pizza non può essere vuoto.");
                return;
            }

            if (!decimal.TryParse(PizzaPrice.Replace(',', '.'), out decimal price) || price <= 0)
            {
                ShowError("Il prezzo deve essere un numero valido maggiore di zero.");
                return;
            }

            // Animation for the button
            ApplyButtonClickAnimation(sender as Button);

            if (SelectedPizza == null)
            {
                // Add new pizza
                Pizza newPizza = new Pizza
                {
                    Name = PizzaName.Trim(),
                    Ingredients = PizzaIngredients?.Trim() ?? "",
                    Price = price
                };

                PizzaMenu.Add(newPizza);
                SelectedPizza = newPizza;

                // Show confirmation with animation
                ShowConfirmation("Pizza aggiunta con successo!");
            }
            else
            {
                // Update existing pizza
                SelectedPizza.Name = PizzaName.Trim();
                SelectedPizza.Ingredients = PizzaIngredients?.Trim() ?? "";
                SelectedPizza.Price = price;

                // Force refresh of the list view
                CollectionViewSource.GetDefaultView(PizzaMenu).Refresh();

                // Show confirmation with animation
                ShowConfirmation("Pizza aggiornata con successo!");
            }

            _isEditing = false;
            _hasUnsavedChanges = false;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if a pizza is selected
            if (SelectedPizza == null)
            {
                ShowError("Seleziona una pizza da eliminare.");
                return;
            }

            // Confirm deletion
            MessageBoxResult result = MessageBox.Show(
                $"Sei sicuro di voler eliminare la pizza '{SelectedPizza.Name}'?",
                "Conferma eliminazione",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Animation for the button
                ApplyButtonClickAnimation(sender as Button);

                // Remove the pizza
                PizzaMenu.Remove(SelectedPizza);
                
                // Clear form
                PizzaName = "";
                PizzaIngredients = "";
                PizzaPrice = "0.00";
                OnPropertyChanged(nameof(PizzaName));
                OnPropertyChanged(nameof(PizzaIngredients));
                OnPropertyChanged(nameof(PizzaPrice));
                
                SelectedPizza = null;
                _hasUnsavedChanges = false;

                // Show confirmation
                ShowConfirmation("Pizza eliminata con successo!");
            }
        }

        private void SaveAllButton_Click(object sender, RoutedEventArgs e)
        {
            SaveMenuToCsv();

            // Animation for the button
            ApplyButtonClickAnimation(sender as Button);

            // Show confirmation
            ShowConfirmation("Menu salvato con successo!");
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Check for unsaved changes
            if (_hasUnsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Ci sono modifiche non salvate. Vuoi salvare prima di chiudere?",
                    "Modifiche non salvate",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    SaveButton_Click(sender, e);
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            this.DialogResult = true;
            this.Close();
        }

        private void SaveMenuToCsv()
        {
            try
            {
                StringBuilder csv = new StringBuilder();
                csv.AppendLine("Pizza,Prezzo");

                foreach (Pizza pizza in PizzaMenu)
                {
                    // Format each line with quotes for the pizza name and ingredients
                    string pizzaData = $"\"{pizza.Name}\",€ {pizza.Price:F2}";
                    csv.AppendLine(pizzaData);
                }

                // Write to the CSV file
                File.WriteAllText(_csvFilePath, csv.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il salvataggio del menu: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowConfirmation(string message)
        {
            // Create a notification popup
            Border notificationBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(100, 200, 100)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(50, 150, 50)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(15, 10, 15, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, 20),
                Opacity = 0
            };

            TextBlock notificationText = new TextBlock
            {
                Text = message,
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Colors.White)
            };

            notificationBorder.Child = notificationText;

            // Add to the window
            Grid mainGrid = this.Content as Grid;
            if (mainGrid != null)
            {
                mainGrid.Children.Add(notificationBorder);
                Grid.SetRow(notificationBorder, 2);

                // Create animation
                DoubleAnimation fadeIn = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(300)
                };

                DoubleAnimation fadeOut = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    BeginTime = TimeSpan.FromSeconds(2),
                    Duration = TimeSpan.FromMilliseconds(300)
                };

                fadeOut.Completed += (s, args) =>
                {
                    mainGrid.Children.Remove(notificationBorder);
                };

                // Apply animations
                notificationBorder.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                notificationBorder.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            }
        }

        private void ApplyButtonClickAnimation(Button button)
        {
            if (button != null)
            {
                // Create a scale animation
                ScaleTransform scaleTransform = new ScaleTransform(1, 1);
                button.RenderTransform = scaleTransform;

                DoubleAnimation scaleDownX = new DoubleAnimation
                {
                    From = 1,
                    To = 0.95,
                    Duration = TimeSpan.FromMilliseconds(100),
                    AutoReverse = true
                };

                DoubleAnimation scaleDownY = new DoubleAnimation
                {
                    From = 1,
                    To = 0.95,
                    Duration = TimeSpan.FromMilliseconds(100),
                    AutoReverse = true
                };

                // Apply animations
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleDownX);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleDownY);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            // If dialog was closed via X button, check for unsaved changes
            if (!this.DialogResult.HasValue && _hasUnsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Ci sono modifiche non salvate. Vuoi salvare prima di chiudere?",
                    "Modifiche non salvate",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    SaveButton_Click(null, null);
                    e.Cancel = false;
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}