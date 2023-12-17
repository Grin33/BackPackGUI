using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curs2
{
  public class Loot
  {
    private string name { get; set; }
    private decimal value { get; set; } //decimal потому что он точнее
    private decimal weight { get; set; }

    public Loot(string name, decimal value, decimal weight)
    {
      this.value = value;
      this.weight = weight;
      this.name = name;
    }
    public string Name 
    { 
      get => name;
      set => name = value;
    }
    public decimal Value 
    { 
      get => value;
      set => this.value = value;
    }
    public decimal Weight 
    { 
      get => weight;
      set => weight = value;
    }
    public override string ToString() //Переопределенный метод для вывода
    {
      return $"{name} value: {value} weight: {weight}";
    }
  }
}
