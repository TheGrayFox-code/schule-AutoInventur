using MySql.Data.MySqlClient;

using System.Windows;

namespace CarInventory
{
    public partial class MainWindow : Window
    {
        private string connectionString = "Server=localhost;Port=3306;Database=CarInventory;User=root;Password=;";

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var cars = new List<Car>();

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Cars";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cars.Add(new Car
                                {
                                    Id = reader.GetInt32("Id"),
                                    Brand = reader.GetString("Brand"),
                                    Model = reader.GetString("Model"),
                                    Year = reader.GetInt32("Year"),
                                    Kilometers = reader.GetInt32("Kilometers"),
                                    Price = reader.GetDecimal("Price")
                                });
                            }
                        }
                    }
                }

                CarList.ItemsSource = cars;
                CarList.DisplayMemberPath = "DisplayName";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Daten: {ex.Message}");
            }
        }

        private void CarList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CarList.SelectedItem is Car selectedCar)
            {
                BrandTextBox.Text = selectedCar.Brand;
                ModelTextBox.Text = selectedCar.Model;
                YearTextBox.Text = selectedCar.Year.ToString();
                KmTextBox.Text = selectedCar.Kilometers.ToString();
                PriceTextBox.Text = selectedCar.Price.ToString("F2");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Button1: Speichert ein Auto aus den bestehenden Textboxen in die Datenbank
            string brand = BrandTextBox.Text;
            string model = ModelTextBox.Text;
            if (int.TryParse(YearTextBox.Text, out int year) &&
                int.TryParse(KmTextBox.Text, out int kilometers) &&
                decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                try
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "INSERT INTO Cars (Brand, Model, Year, Kilometers, Price) VALUES (@Brand, @Model, @Year, @Kilometers, @Price);";
                        using (var cmd = new MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@Brand", brand);
                            cmd.Parameters.AddWithValue("@Model", model);
                            cmd.Parameters.AddWithValue("@Year", year);
                            cmd.Parameters.AddWithValue("@Kilometers", kilometers);
                            cmd.Parameters.AddWithValue("@Price", price);
                            cmd.ExecuteNonQuery();
                            
                        }
                    }

                    MessageBox.Show("Auto erfolgreich gespeichert!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Speichern des Autos: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Bitte gültige Werte in allen Feldern eingeben.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Button2: Löscht das ausgewählte Auto aus der Datenbank
            if (CarList.SelectedItem is Car selectedCar)
            {
                try
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM Cars WHERE Id = @Id";
                        using (var cmd = new MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@Id", selectedCar.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Auto erfolgreich gelöscht!", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Löschen des Autos: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie ein Auto aus der Liste aus.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    public class Car
    {
        public string DisplayName => $"{Brand} {Model}, BJ. {Year}";

        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Kilometers { get; set; }
        public decimal Price { get; set; }
    }
}
