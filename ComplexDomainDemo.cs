using System;
using System.Linq;
using ComplexDomain;

namespace pg_pg {
    public static class ComplexDomainDemo{
        public static void Run(){
            var s1 = new Score(5,0);
            Console.WriteLine($"ID: {s1.GetId()}");

            var m1 = new Winner3Way(
                new Outcome.Priced(1.85m),
                new Outcome.Priced(3.3m),
                new Outcome.Priced(2.0m));

            var outcomes = m1.GetOutcomes();
            Console.WriteLine($"Outcomes: {string.Join(",", outcomes)}");

            var newOutcomes = outcomes.Select(pair => (pair.Item1,
                pair.Item2 switch{
                    Outcome.Priced c => c with {Price = c.Price * 1.1m},
                    Outcome.PricedWithProb c => c with {Price = c.Price * 1.1m},
                    _ => pair.Item2
                }));

            var m2 = m1.With(newOutcomes);

            Console.WriteLine($"M1: {m1}");
            Console.WriteLine($"M2: {m2}");
        }
    }
}