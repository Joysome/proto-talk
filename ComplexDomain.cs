#nullable enable

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace ComplexDomain {
    public enum OutcomeResult {
        Unknown = 0,
        Win = 1,
        Lose = 2,
        Void = 3,
        Canceled = 4
    }

    public abstract record Outcome {
        public record Unknown() : Outcome;
        public record Empty() : Outcome;
        public record Priced(decimal Price) : Outcome;
        public record PricedWithProb(decimal Price, float Prob) : Outcome;
        public record Resulted(OutcomeResult Result) : Outcome;
        static public readonly Outcome UnknownValue = new Unknown();
        static public readonly Outcome EmptyValue = new Empty();
    }

    public record Winner3Way(Outcome Win1, Outcome Draw, Outcome Win2);
    public record Handicap(decimal Value, Outcome Win1, Outcome Win2);
    public record Total(decimal Value, Outcome Over, Outcome Under);
    public record Score(int S1, int S2);
    public record ScoreOutcome(Score Score, Outcome Outcome);
    public record CorrectScore(IReadOnlyList<ScoreOutcome> Scores);

    public static class IdHelper {
        private static string BuildId(System.Action<System.Text.StringBuilder> f){
            var txt = new System.Text.StringBuilder();
            txt.Append('(');
            f(txt);
            txt.Append(')');
            return txt.ToString();
        }

        public static string GetId(this Score v)
            => BuildId(txt => txt.Append(v.S1).Append(',').Append(v.S2));

    }

    public static class IdxHelper {
        public static Outcome GetOutcome(this Winner3Way x, string idx)
            => idx switch {
                "(1)" => x.Win1,
                "(2)" => x.Draw,
                "(3)" => x.Win2,
                _ => Outcome.UnknownValue
            };
        public static (string,Outcome)[] GetOutcomes(this Winner3Way x)
            => new[]{("(1)", x.Win1), ("(2)", x.Draw), ("(3)", x.Win2)};
        public static Winner3Way With(this Winner3Way x, IEnumerable<(string,Outcome)> pairs){
            var (win1, draw, win2) = (Outcome.UnknownValue, Outcome.UnknownValue, Outcome.UnknownValue);
            foreach(var (k, v) in pairs){
                switch (k){
                    case "(1)": win1 = v; break;
                    case "(2)": draw = v; break;
                    case "(3)": win2 = v; break;
                }
            }
            return x with {
                Win1 = win1 != Outcome.UnknownValue ? win1 : x.Win1,
                Draw = draw != Outcome.UnknownValue ? draw : x.Draw,
                Win2 = win2 != Outcome.UnknownValue ? win2 : x.Win2
            };
        }
        public static Outcome GetOutcome(this CorrectScore x, Score idx)
            => x.Scores.FirstOrDefault(v => v.Score == idx)?.Outcome??Outcome.UnknownValue;
        public static (Score,Outcome)[] GetOutcomes(this CorrectScore x, string idx)
            => x.Scores.Select(v => (v.Score, v.Outcome)).ToArray();
        public static CorrectScore With(this CorrectScore x, IEnumerable<(Score,Outcome)> pairs){
            var dict = pairs.ToDictionary(pair => pair.Item1, pair => pair.Item2);
            return x with {
                Scores = x.Scores.Select(v => dict.TryGetValue(v.Score, out var o) ? v with {Outcome = o} : v).ToList().AsReadOnly()
            };
        }

    }
}