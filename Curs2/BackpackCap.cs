using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curs2
{
  class BackpackCap
  {
    private decimal capacity { get; set; }

    public decimal Capacity => capacity;

    public BackpackCap(decimal capacity)
    {
      this.capacity = capacity;
    }

    public BackpackCap(string Capacity)
    {
      this.capacity = Capacity switch
      {
        null => throw new ArgumentNullException(nameof(Capacity)),
        "default" or "Default" => 100,
        _ => this.capacity
      };
    }

  }
}