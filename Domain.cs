#nullable enable

namespace Domain {
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
}