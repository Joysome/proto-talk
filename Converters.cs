using System;
using System.Text.Json;
using System.Collections.Generic;

namespace Domain.Converters {

    public static class Binary {
        private static long DecimalToLong(decimal v, int scale)
            => (long)(Decimal.Truncate(v * (decimal)Math.Pow(10, scale)));
        public static decimal LongToDecimal(long v, int scale)
            => v / (decimal)Math.Pow(10, scale);

        static Protobuf.OutcomeEmpty outcomeEmptyValue = new Protobuf.OutcomeEmpty();
        public static Protobuf.OutcomeEmpty FromDomain(this Outcome.Empty v)
            => outcomeEmptyValue;

        public static Protobuf.OutcomePriced FromDomain(this Outcome.Priced v)
            => new Protobuf.OutcomePriced() {
                Price = DecimalToLong(v.Price, 3)};

        public static Protobuf.OutcomePricedWithProb FromDomain(this Outcome.PricedWithProb v)
            => new Protobuf.OutcomePricedWithProb() {
                Price = DecimalToLong(v.Price, 3),
                Prob = v.Prob};

        public static Protobuf.OutcomeResulted FromDomain(this Outcome.Resulted v)
            => new Protobuf.OutcomeResulted() {
                Result = (Protobuf.OutcomeResult)v.Result};

        public static Outcome ToDomain(this Protobuf.OutcomeEmpty v)
            => Outcome.EmptyValue;

        public static Outcome ToDomain(this Protobuf.OutcomePriced v)
            => new Outcome.Priced (
                LongToDecimal(v.Price, 3));

        public static Outcome ToDomain(this Protobuf.OutcomePricedWithProb v)
            => new Outcome.PricedWithProb(
                LongToDecimal(v.Price, 3),
                v.Prob);

        public static Outcome ToDomain(this Protobuf.OutcomeResulted v)
            => new Outcome.Resulted((OutcomeResult)v.Result);

        public static Protobuf.Winner3Way FromDomain(this Winner3Way v){
            var target = new Protobuf.Winner3Way();
            switch (v.Win1) {
                case Outcome.Empty c:
                    target.Win1Empty = c.FromDomain();
                    break;
                case Outcome.Priced c:
                    target.Win1Priced = c.FromDomain();
                    break;
                case Outcome.PricedWithProb c:
                    target.Win1PricedWithProb = c.FromDomain();
                    break;
                case Outcome.Resulted c:
                    target.Win1Resulted = c.FromDomain();
                    break;
            }
            switch (v.Draw) {
                case Outcome.Empty c:
                    target.DrawEmpty = c.FromDomain();
                    break;
                case Outcome.Priced c:
                    target.DrawPriced = c.FromDomain();
                    break;
                case Outcome.PricedWithProb c:
                    target.DrawPricedWithProb = c.FromDomain();
                    break;
                case Outcome.Resulted c:
                    target.DrawResulted = c.FromDomain();
                    break;
            }
            switch (v.Win2) {
                case Outcome.Empty c:
                    target.Win2Empty = c.FromDomain();
                    break;
                case Outcome.Priced c:
                    target.Win2Priced = c.FromDomain();
                    break;
                case Outcome.PricedWithProb c:
                    target.Win2PricedWithProb = c.FromDomain();
                    break;
                case Outcome.Resulted c:
                    target.Win2Resulted = c.FromDomain();
                    break;
            }
            return target;
        }

        public static Winner3Way ToDomain(this Protobuf.Winner3Way v)
            => new Winner3Way (
                v.Win1Case switch {
                    Protobuf.Winner3Way.Win1OneofCase.Win1Empty => v.Win1Empty.ToDomain(),
                    Protobuf.Winner3Way.Win1OneofCase.Win1Priced => v.Win1Priced.ToDomain(),
                    Protobuf.Winner3Way.Win1OneofCase.Win1PricedWithProb => v.Win1PricedWithProb.ToDomain(),
                    Protobuf.Winner3Way.Win1OneofCase.Win1Resulted => v.Win1Resulted.ToDomain(),
                    _ => Outcome.UnknownValue
                },
                v.DrawCase switch {
                    Protobuf.Winner3Way.DrawOneofCase.DrawEmpty => v.DrawEmpty.ToDomain(),
                    Protobuf.Winner3Way.DrawOneofCase.DrawPriced => v.DrawPriced.ToDomain(),
                    Protobuf.Winner3Way.DrawOneofCase.DrawPricedWithProb => v.DrawPricedWithProb.ToDomain(),
                    Protobuf.Winner3Way.DrawOneofCase.DrawResulted => v.DrawResulted.ToDomain(),
                    _ => Outcome.UnknownValue
                },
                v.Win2Case switch {
                    Protobuf.Winner3Way.Win2OneofCase.Win2Empty => v.Win2Empty.ToDomain(),
                    Protobuf.Winner3Way.Win2OneofCase.Win2Priced => v.Win2Priced.ToDomain(),
                    Protobuf.Winner3Way.Win2OneofCase.Win2PricedWithProb => v.Win2PricedWithProb.ToDomain(),
                    Protobuf.Winner3Way.Win2OneofCase.Win2Resulted => v.Win2Resulted.ToDomain(),
                    _ => Outcome.UnknownValue
                });
    }

    public static class Json {

        public static void WriteJson(this Outcome.Empty v, Utf8JsonWriter writer){
            writer.WriteStartObject();
            writer.WriteEndObject();
        }
        public static void WriteJson(this Outcome.Priced v, Utf8JsonWriter writer){
            writer.WriteStartObject();
            writer.WriteNumber("price", v.Price);
            writer.WriteEndObject();
        }
        public static void WriteJson(this Outcome.PricedWithProb v, Utf8JsonWriter writer){
            writer.WriteStartObject();
            writer.WriteNumber("price", v.Price);
            writer.WriteNumber("prob", v.Prob);
            writer.WriteEndObject();
        }
        public static void WriteJson(this Outcome.Resulted v, Utf8JsonWriter writer){
            writer.WriteStartObject();
            writer.WriteString("result", v.Result.ToString());
            writer.WriteEndObject();
        }

        private static void AssertIsObject(JsonElement el, string targetType){
            if (el.ValueKind != JsonValueKind.Object)
                throw new Exception($"{targetType} supposed be object, but here is {el.ValueKind}");
        }

        private static decimal ReadDecimal(JsonElement el, string propertyName, decimal defaultValue){
            if (el.TryGetProperty(propertyName, out var propEl))
                if (propEl.ValueKind == JsonValueKind.Number) return propEl.GetDecimal();
                else throw new Exception($"Property {propertyName} supposed be numeric, but here is {el.ValueKind}");
            else
                return defaultValue;
        }

        private static float ReadFloat(JsonElement el, string propertyName, float defaultValue){
            if (el.TryGetProperty(propertyName, out var propEl))
                if (propEl.ValueKind == JsonValueKind.Number) return (float)propEl.GetDouble();
                else throw new Exception($"Property {propertyName} supposed be numeric, but here is {el.ValueKind}");
            else
                return defaultValue;
        }

        private static T ReadEnum<T>(JsonElement el, string propertyName, T defaultValue, params (string,T)[] cases){
            if (el.TryGetProperty(propertyName, out var propEl))
                if (propEl.ValueKind == JsonValueKind.String){
                    var caseName = propEl.GetString();
                    foreach (var (name, val) in cases)
                        if (name == caseName) return val;
                    return defaultValue;
                }
                else {
                    throw new Exception($"Property {propertyName} supposed be numeric, but here is {el.ValueKind}");
                }
            else
                return defaultValue;
        }

        public static Outcome ReadOutcomeEmpty(JsonElement el){
            AssertIsObject(el, "Outcome.Empty");
            return Outcome.EmptyValue;
        }

        public static Outcome ReadOutcomePriced(JsonElement el){
            AssertIsObject(el, "Outcome.Priced");
            return new Outcome.Priced(ReadDecimal(el, "price", 0m));
        }

        public static Outcome ReadOutcomePricedWithProb(JsonElement el){
            AssertIsObject(el, "Outcome.PricedWithProb");
            return new Outcome.PricedWithProb(
                ReadDecimal(el, "price", 0m),
                ReadFloat(el, "prob", 0f));
        }

        public static Outcome ReadOutcomeResulted(JsonElement el){
            AssertIsObject(el, "Outcome.Resulted");
            return new Outcome.Resulted(
                ReadEnum(el, "result", OutcomeResult.Unknown,
                    ("Win", OutcomeResult.Win),
                    ("Lose", OutcomeResult.Lose),
                    ("Void", OutcomeResult.Void),
                    ("Canceled", OutcomeResult.Canceled)));
        }

        public static void WriteJson(this Outcome v, Utf8JsonWriter writer, params string[] propNames){
            switch (v){
                case Outcome.Empty c:
                    writer.WritePropertyName (propNames[0]);
                    c.WriteJson(writer);
                    break;
                case Outcome.Priced c:
                    writer.WritePropertyName (propNames[1]);
                    c.WriteJson(writer);
                    break;
                case Outcome.PricedWithProb c:
                    writer.WritePropertyName (propNames[2]);
                    c.WriteJson(writer);
                    break;
                case Outcome.Resulted c:
                    writer.WritePropertyName (propNames[3]);
                    c.WriteJson(writer);
                    break;
            }
        }

        public static void WriteJson(this Winner3Way v, Utf8JsonWriter writer){
            writer.WriteStartObject();
            v.Win1.WriteJson(writer, "win1Empty", "win1Priced", "win1PricedWithProb", "win1Resulted");
            v.Draw.WriteJson(writer, "drawEmpty", "drawPriced", "drawPricedWithProb", "drawResulted");
            v.Win2.WriteJson(writer, "win2Empty", "win2Priced", "win2PricedWithProb", "win2Resulted");
            writer.WriteEndObject();
        }

        private static T ReadUnion<T>(JsonElement el, T unknownValue, params (string, Func<JsonElement, T>)[] cases){
            foreach (var (caseName, f) in cases)
                if (el.TryGetProperty(caseName, out var caseEl))
                    return f(caseEl);

            return unknownValue;
        }

        public static Winner3Way ReadWinner3Way(JsonElement el){
            AssertIsObject(el, "Winner3Way");

            return new Winner3Way(
                ReadUnion(el, Outcome.UnknownValue,
                    ("win1Empty", caseEl => ReadOutcomeEmpty(caseEl)),
                    ("win1Priced", caseEl => ReadOutcomePriced(caseEl)),
                    ("win1PricedWithProb", caseEl => ReadOutcomePricedWithProb(caseEl)),
                    ("win1Resulted", caseEl => ReadOutcomeResulted(caseEl))),
                ReadUnion(el, Outcome.UnknownValue,
                    ("drawEmpty", caseEl => ReadOutcomeEmpty(caseEl)),
                    ("drawPriced", caseEl => ReadOutcomePriced(caseEl)),
                    ("drawPricedWithProb", caseEl => ReadOutcomePricedWithProb(caseEl)),
                    ("drawResulted", caseEl => ReadOutcomeResulted(caseEl))),
                ReadUnion(el, Outcome.UnknownValue,
                    ("win2Empty", caseEl => ReadOutcomeEmpty(caseEl)),
                    ("win2Priced", caseEl => ReadOutcomePriced(caseEl)),
                    ("win2PricedWithProb", caseEl => ReadOutcomePricedWithProb(caseEl)),
                    ("win2Resulted", caseEl => ReadOutcomeResulted(caseEl))));
        }
    }
}