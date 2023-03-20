// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json.Serialization;

namespace OpenAILib.FineTuning
{
    internal class FineTuneRequest
    {
        [JsonPropertyName("training_file")]
        public string TrainingFile { get; set; }

        [JsonPropertyName("validation_file")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ValidationFile { get; set; }

        [JsonPropertyName("model")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Model { get; set; }

        [JsonPropertyName("n_epocs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? NEpochs { get; set; }

        [JsonPropertyName("batch_size")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? BatchSize { get; set; }

        [JsonPropertyName("learning_rate_multiplier")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? LearningRateMultiplier { get; set; }

        [JsonPropertyName("prompt_loss_weight")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? PromptLossWeight { get; set; }

        [JsonPropertyName("compute_classification_metrics")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ComputeClassificationMetrics { get; set; }

        [JsonPropertyName("classification_n_classes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ClassificationNClasses { get; set; }

        [JsonPropertyName("classification_positive_class")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ClassificationPositiveClass { get; set; }

        [JsonPropertyName("classification_betas")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double[]? ClassificationBetas { get; set; }

        [JsonPropertyName("suffix")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Suffix { get; set; }

        public FineTuneRequest(string trainingFile)
        {
            TrainingFile = trainingFile;
        }
    }
}
