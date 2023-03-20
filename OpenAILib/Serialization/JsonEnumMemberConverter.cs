// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAILib.Serialization
{
    // Though I wondered initially why such a mechanism is not built in, I can see there are actually
    // a few edge cases with this. Though rare, enum values do not need to be unique and (naturally)
    // EnumMemberAttribute Value properties similarily do not need to be unique. This class works for normal
    // scenarios but will fail on edge cases. It is only used internally so deemed acceptable.
    // Microsoft could likely do something similar but perhaps use attributes within the System.Text.Json namespace
    // and perhaps have a nicer way to handle the edge cases
    internal class JsonEnumMemberConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        private static Lazy<StringValueLookup<T>> EnumValueToNameLookup = new(CreateStringValueMap);

        private static StringValueLookup<T> CreateStringValueMap()
        {
            var type = typeof(T);
            // Values are ordered by their unsigned magnitude, 
            // this works fine for "normal" enums with unique non-negative values
            // but there are edge cases where it will not work
            var valueToNameMap = Enum.GetValues<T>()
                .Zip(Enum.GetNames<T>())
                .ToDictionary(pair => pair.First, pair => pair.Second);

            var pairs = new List<(string name, T value)>(valueToNameMap.Count);
            foreach (var (value, name) in valueToNameMap)
            {
                var enumMemberAttribute = type.GetMember(name).First().GetCustomAttribute<EnumMemberAttribute>();
                // if the EnumMemberAttribute is associated with the property, 
                // we use the associated name [EnumMember(Value = "my-value")]
                // otherwise the original identifier is used in camelCase
                if (enumMemberAttribute?.Value != null)
                {
                    pairs.Add((enumMemberAttribute.Value, value));
                }
                else
                {
                    string camelCaseName = char.ToLowerInvariant(name[0]) + name[1..];
                    pairs.Add((camelCaseName, value));
                }
            }
            return new StringValueLookup<T>(pairs);
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var enumString = reader.GetString();
            if (string.IsNullOrEmpty(enumString))
            {
                throw new JsonException($"Found empty string when attempting to convert json text to '{typeToConvert.FullName}'.");
            }
            var lookup = EnumValueToNameLookup.Value;
            return lookup.GetValue(enumString);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var lookup = EnumValueToNameLookup.Value;
            var enumString = lookup.GetName(value);
            writer.WriteStringValue(enumString);
        }

        private class StringValueLookup<TEnum> where TEnum : struct, Enum
        {
            private readonly Dictionary<TEnum, string> _valueToString;
            private readonly Dictionary<string, TEnum> _stringToValue;

            public StringValueLookup(List<(string name, TEnum value)> pairs)
            {
                _valueToString = new Dictionary<TEnum, string>();
                _stringToValue = new Dictionary<string, TEnum>();

                foreach (var (name, value) in pairs)
                {

                    if (!_valueToString.TryAdd(value, name))
                    {
                        throw new ArgumentException($"Duplicate value '{value}' in {nameof(pairs)}", nameof(pairs));
                    }

                    if (!_stringToValue.TryAdd(name, value))
                    {
                        throw new ArgumentException($"Duplicate name '{name}' in {nameof(pairs)}", nameof(pairs));
                    };
                }
            }

            public string GetName(TEnum value)
            {
                return _valueToString[value];
            }

            public TEnum GetValue(string name)
            {
                return _stringToValue[name];
            }
        }
    }
}
