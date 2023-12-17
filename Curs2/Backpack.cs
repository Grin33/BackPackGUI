using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Curs2
{
  public class Backpack
  {
    static object locker = new();
    static object locker1 = new();
    public List<Loot>? Most_Valuable { get; private set; } = null;
    public decimal max_weight { get; set; }
    public decimal best_value { get; set; } = 0;
    public decimal final_weight { get; set; }

    public Backpack(decimal max_weight)
    {
      this.max_weight = max_weight;
    }
    #region Parallel

    public bool Check_Parallel(ref List<Loot> thread_loots, ref List<Loot> thread_best)
    {
      decimal iteration_weight = 0;
      decimal iteration_value = 0;
      foreach (var oneloot in thread_loots)
      {
        iteration_value += oneloot.Value;
        iteration_weight += oneloot.Weight;
      }

      var thread_best_value = thread_best.Sum(v => v.Value);

      if (iteration_weight <= max_weight)
      {
        if (iteration_value > thread_best_value)
        {
          thread_best = new List<Loot>(thread_loots);
          return true;
        }
        else
          return true;
      }
      else
        return false;
    }

    public void nested_shuffle(ref List<Loot> loots, ref List<Loot> thread_temp, List<Loot> iter_loots, Loot i)
    {
      var v = loots.IndexOf(i) + 1;
      for (var n = v; n < loots.Count; n++)
      {
        var iter_new_loots = new List<Loot>(iter_loots) { loots[n] };
        //Check_Parallel(ref iter_new_loots, ref thread_temp);
        if (Check_Parallel(ref iter_new_loots, ref thread_temp))
          nested_shuffle(ref loots, ref thread_temp, iter_new_loots, loots[n]);

      }

    }

    public void Parallel_shuffle(List<Loot> loots)
    {
      Parallel.ForEach<Loot, List<Loot>>(
        loots,
        () => new List<Loot>(),
        (i, loop, thread_temp) =>
        {
          var iter_loots = new List<Loot>();
          iter_loots.Add(i);

          //Check_Parallel(ref iter_loots, ref thread_temp);
          if (Check_Parallel(ref iter_loots, ref thread_temp))
            nested_shuffle(ref loots, ref thread_temp, iter_loots, i);
          return thread_temp; //передается в следующую итерацию одного потока
        },
        (thread_final) =>
        {
          decimal thread_best_value = 0;
          decimal thread_best_weight = 0;
          foreach (var oneloot in thread_final)
          {
            thread_best_value += oneloot.Value;
            thread_best_weight += oneloot.Weight;
          }

          lock (locker)
          {
            if (best_value >= thread_best_value) return;
            Most_Valuable = thread_final;
            final_weight = thread_best_weight;
            best_value = thread_best_value;
          }
        }
      );
    }

    #endregion Parallel

  }
}