// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.FineTuning
{
    internal static class FineTuneTrainingDataProcessor
    {
        // Excerpt from https://platform.openai.com/docs/guides/fine-tuning/preparing-your-dataset
        // To fine-tune a model, you'll need a set of training examples that each consist of a single input ("prompt") and its associated output ("completion").
        // This is notably different from using our base models, where you might input detailed instructions or multiple examples in a single prompt.

        // 1. Each prompt should end with a fixed separator to inform the model when the prompt ends and the completion begins.A simple separator
        // which generally works well is \n\n###\n\n. The separator should not appear elsewhere in any prompt.

        // 2. Each completion should start with a whitespace due to our tokenization, which tokenizes most words with a preceding whitespace.

        // 3. Each completion should end with a fixed stop sequence to inform the model when the completion ends.A stop sequence could be \n, ###, or any
        // other token that does not appear in any completion.
        public static List<FineTunePair> ProcessFineTuneData(List<FineTunePair> data, string promptSuffix, string completionSuffix)
        {
            var result = new List<FineTunePair>(data.Count);
            foreach (var pair in data)
            {                        
                // Note that the single space prefix in the completion addresses point 2
                result.Add(new FineTunePair(prompt: pair.Prompt + promptSuffix, completion: " " + pair.Completion + completionSuffix));
            }
            return result;
        }
    }
}
