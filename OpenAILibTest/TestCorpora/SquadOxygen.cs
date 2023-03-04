// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.FineTuning;

namespace OpenAILib.Tests.TestCorpora
{
    internal class SquadOxygen
    {
        public static List<FineTunePair> Create()
        {
            return new List<FineTunePair>
            {
                new (prompt: "What is the atomic number of oxygen?", completion: "8"),
                new (prompt: "What is the electron configuration of oxygen?", completion: "1s² 2s² 2p⁴"),
                new (prompt: "What is the most common isotope of oxygen?", completion: "¹⁶O"),
                new (prompt: "What is the boiling point of oxygen?", completion: "-183.0 °C"),
                new (prompt: "What is the density of oxygen at STP?", completion: "1.429 g/L"),
                new (prompt: "What is the molecular formula of dioxygen?", completion: "O₂"),
                new (prompt: "What is the oxidation state of oxygen in H₂O?", completion: "-2"),
                new (prompt: "What is the molar mass of oxygen?", completion: "15.999 g/mol"),
                new (prompt: "What is the largest commercial use of oxygen?", completion: "steel production"),
                new (prompt: "What is the symbol for oxygen?", completion: "O")
            };
        }

        public static string CreateText()
        {
            return @"
                {""prompt"": ""What is the atomic number of oxygen?"", ""completion"": ""8""}
                {""prompt"": ""What is the electron configuration of oxygen?"", ""completion"": ""1s² 2s² 2p⁴""}
                {""prompt"": ""What is the most common isotope of oxygen?"", ""completion"": ""¹⁶O""}
                {""prompt"": ""What is the boiling point of oxygen?"", ""completion"": ""-183.0 °C""}
                {""prompt"": ""What is the density of oxygen at STP?"", ""completion"": ""1.429 g/L""}
                {""prompt"": ""What is the molecular formula of dioxygen?"", ""completion"": ""O₂""}
                {""prompt"": ""What is the oxidation state of oxygen in H₂O?"", ""completion"": ""-2""}
                {""prompt"": ""What is the molar mass of oxygen?"", ""completion"": ""15.999 g/mol""}
                {""prompt"": ""What is the largest commercial use of oxygen?"", ""completion"": ""steel production""}
                {""prompt"": ""What is the symbol for oxygen?"", ""completion"": ""O""}";
        }
    }
}
