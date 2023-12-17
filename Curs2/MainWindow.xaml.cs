using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using static System.Decimal;

namespace Curs2
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private List<Loot> Loots { get; set; } = new();

    public MainWindow()
    {
      InitializeComponent();
      InitLoots();
    }

    private void InitLoots()
    {
      Loots = new List<Loot>
      {
        new Loot("1", 2000m, 30m), new Loot("2", 2500m, 10m), new Loot("3", 4000m, 40m), new Loot("4", 2100m, 35m),
        new Loot("5", 4000m, 45m), new Loot("6", 3000m, 40m), new Loot("7", 2500m, 40m), new Loot("8", 2900m, 15m),
        new Loot("9", 3000m, 16m), new Loot("10", 3000m, 15.5m), new Loot("11", 2700m, 11m), new Loot("12", 3500m, 39m)
      };
      LootGrid.ItemsSource = Loots;
    }

    private void ClearData_Click(object sender, RoutedEventArgs e)
    {
      Loots.Clear();
      LootGrid.Items.Refresh();
    }

    private void AddMenuClick(object sender, RoutedEventArgs e)
    {
      var newWindow = new AddItem();
      newWindow.Show();
      newWindow.Closed += NewWindow_Closed;
    }

    private void NewWindow_Closed(object? sender, EventArgs e)
    {
      var window = sender as AddItem;
      var newLoot = window?.NewLoot;
      if (newLoot == null) return;
      Loots.Add(newLoot);
      LootGrid.Items.Refresh();
    }

    private void DeleteItemClick(object sender, RoutedEventArgs e)
    {
      var selectedLoot = LootGrid.SelectedItem;
      if (selectedLoot is not Loot loot) return;
      Loots.Remove(Loots.First(k => k.Value == loot.Value && loot.Name == k.Name && loot.Weight == k.Weight));
      LootGrid.Items.Refresh();
    }

    //Рассчет
    private async void Button_Click(object sender, RoutedEventArgs e)
    {
      decimal cap = 0;
      try
      {
        if (!string.IsNullOrEmpty(CapBox.Text))
        {
          var res = TryParse(CapBox.Text, out cap);
          if(res == false)
            return;
        }
      }
      catch
      {
        MessageBox.Show("Неверно указана вместимость");
        return;
      }

      var capacity = new BackpackCap(cap);
      var back = new Backpack(capacity.Capacity);
      var loots = new List<Loot>(this.Loots);
      LoadingRing.Visibility = Visibility.Visible;
      await Task.Run(() => back.Parallel_shuffle(loots));
      LoadingRing.Visibility = Visibility.Hidden;
      ShowAns(back);
    }

    private void ShowAns(Backpack back)
    {
      if (back.Most_Valuable == null)
      {
        MessageBox.Show("Нет ответа");
        return;
      }

      FinLootGrid.ItemsSource = back.Most_Valuable;
      FinLootGrid.Items.Refresh();
    }

    //save
    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
      var fileName = string.Empty;
      var sd = new SaveFileDialog
      {
        DefaultExt = "json"
      };
      if (sd.ShowDialog() == true)
      {
        fileName = sd.FileName;
      }

      if (fileName == string.Empty)
        return;
      var fs = File.Open(fileName, FileMode.Create);

      var json = JsonSerializer.Serialize(this.Loots);
      var info = new UTF8Encoding(true).GetBytes(json);
      fs.Write(info, 0, info.Length);
      fs.Close();
    }

    //open
    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
      var fd = new OpenFileDialog
      {
        Filter = "*.json|*.json"
      };
      if (fd.ShowDialog() == false)
      {
        MessageBox.Show("Выбран неверный файл", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      var file = fd.FileName;
      FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read);
      var json = JsonSerializer.Deserialize(fs, typeof(List<Loot>));
      var list = json as List<Loot>;
      if (list == null)
      {
        this.Loots = new List<Loot>();
        this.LootGrid.Items.Refresh();
        return;
      }

      this.Loots = list;
      this.LootGrid.Items.Refresh();
      fs.Close();
    }

    //help
    private void Button_Click_3(object sender, RoutedEventArgs e)
    {

      MessageBox.Show(
        "Программа для решения задачи о рюкзаке \nАвтор: Поздеев Д.А. \n\nИнструкция: " +
        "\n\n Кнопка \"Сохранить\" Сохраняет введеные данные в формате json" +
        "\n\n Кнопка \"Открыть\" Позволяет открыть сохраненный файл с введеными файлами" +
        "\n\n Кнопка \"Рассчитать\" Рассчитывает и выводит на экран результат работы программы" +
        "\n\n Кнопка \"Очистить\" Очищает все введеные элементы" +
        "\n\n Вместимость рюкзака вводить в поле рядом с одноименным полем" +
        "\n\n Добавление/удаление введеных данных происходит через контекстное меню. Для его вызова щелкните ПКМ по таблице с входными данными");
    }
  }
}