// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.FineTuning
{
    internal interface IFineTuneSpec01
    {
        IFineTuneSpec01 Model(string model);
        IFineTuneSpec01 ValidationData(List<FineTunePair> validationData);
        IFineTuneSpec01 PromptSuffix(string promptSuffix);
        IFineTuneSpec01 CompletionSuffix(string completionSuffix);
    }
}
