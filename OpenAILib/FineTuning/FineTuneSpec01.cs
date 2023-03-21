// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.FineTuning
{
    internal class FineTuneSpec01 : IFineTuneSpec01
    {
        public const string DefaultPromptSuffix = "\n\n###\n\n";
        public const string DefaultCompletionSuffix = "###";

        // Fine tune request related items - directly translated into the request
        private string? _model;
        private int? _epochs;
        private int? _batchSize;
        private double? _learningRateMultiplier;
        private double? _promptLossWeight;
        private bool? _computeClassificationMetrics;
        private int? _classificationNClasses;
        private string? _classificationPositiveClass;
        private double[]? _classificationBetas;
        private string? _modelSuffix;

        // Suffixes and validation data handling
        private string _promptSuffix = DefaultPromptSuffix;
        private string _completionSuffix = DefaultCompletionSuffix;
        private List<FineTunePair> _validationData = new List<FineTunePair>(0);

        public IFineTuneSpec01 Model(string model)
        {
            _model = model;
            return this;
        }

        public IFineTuneSpec01 Epochs(int epochs)
        {
            _epochs = epochs;
            return this;
        }

        public IFineTuneSpec01 BatchSize(int batchSize)
        {
            _batchSize = batchSize;
            return this;
        }

        public IFineTuneSpec01 LearningRateMultiplier(double learningRateMultiplier)
        {
            _learningRateMultiplier = learningRateMultiplier;
            return this;
        }

        public IFineTuneSpec01 PromptLossWeight(double promptLossWeight)
        {
            _promptLossWeight = promptLossWeight;
            return this;
        }


        public IFineTuneSpec01 ComputeClassificationMetrics(bool computeClassificationMetrics)
        {
            _computeClassificationMetrics = computeClassificationMetrics;
            return this;
        }

        public IFineTuneSpec01 ClassificationNClasses(int classificationNClasses)
        {
            _classificationNClasses = classificationNClasses;
            return this;
        }

        public IFineTuneSpec01 ClassificationPositiveClass(string classificationPositiveClass)
        {
            _classificationPositiveClass = classificationPositiveClass;
            return this;
        }

        public IFineTuneSpec01 ClassificationBetas(params double[] betas)
        {
            _classificationBetas = betas;
            return this;
        }

        public IFineTuneSpec01 ModelSuffix(string modelSuffix)
        {
            _modelSuffix = modelSuffix;
            return this;
        }

        public IFineTuneSpec01 PromptSuffix(string promptSuffix)
        {
            _promptSuffix = promptSuffix;
            return this;
        }

        public IFineTuneSpec01 CompletionSuffix(string completionSuffix)
        {
            _completionSuffix = completionSuffix;
            return this;
        }

        public IFineTuneSpec01 ValidationData(List<FineTunePair> validationData)
        {
            _validationData = validationData;
            return this;
        }

        public FineTuneRequest ToRequest(string trainingFileId, string? validationFileId)
        {
            var request = new FineTuneRequest(trainingFileId)
            {
                ValidationFile = validationFileId,
                Model = _model,
                NEpochs = _epochs,
                BatchSize =_batchSize,
                LearningRateMultiplier = _learningRateMultiplier,
                PromptLossWeight = _promptLossWeight,
                ComputeClassificationMetrics = _computeClassificationMetrics,
                ClassificationNClasses = _classificationNClasses,
                ClassificationPositiveClass = _classificationPositiveClass,
                ClassificationBetas = _classificationBetas,
                Suffix = _modelSuffix
            };
            return request;
        }

        public string GetPromptSuffix() => _promptSuffix;
        public string GetCompletionSuffix() => _completionSuffix;
        public List<FineTunePair> GetValidationData() => _validationData;
    }
}
