using System.Windows;
using static System.Decimal;

namespace Curs2
{
  /// <summary>
  /// Логика взаимодействия для AddItem.xaml
  /// </summary>
  public partial class AddItem : Window
  {
    public Loot? NewLoot;

    public AddItem()
    {
      InitializeComponent();
      NewLoot = null;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
      try
      {
        if (string.IsNullOrEmpty(NameBox.Text) || string.IsNullOrEmpty(WeightBox.Text) ||
            string.IsNullOrEmpty(ValBox.Text)) return;
        var tryParse = TryParse(ValBox.Text, out var val);
        var tryPare1 = TryParse(WeightBox.Text, out var weight);
        if(tryParse == false || tryPare1 == false)
          return;
        NewLoot = new Loot(NameBox.Text, val, weight);
        Close();
      }
      catch
      {
        MessageBox.Show("Введены неверные значения");
      }
    }
  }
}