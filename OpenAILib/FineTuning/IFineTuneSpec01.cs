// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.FineTuning
{
    /// <summary>
    /// Represents specifications associated with creating fine-tuned models
    /// </summary>
    public interface IFineTuneSpec01
    {
        /// <summary>
        /// Sets the name of the base model to fine-tune. Defaults to "curie".
        /// </summary>
        IFineTuneSpec01 Model(string model);

        /// <summary>
        /// Sets the number of epochs to train the model for. An epoch refers to one full cycle through the training dataset. Defaults to 4.
        /// </summary>
        IFineTuneSpec01 Epochs(int epochs);

        /// <summary>
        /// Sets the batch size to use for training. The batch size is the number of training examples used to train a single forward and backward pass.
        /// </summary>
        IFineTuneSpec01 BatchSize(int batchSize);

        /// <summary>
        /// Sets the learning rate multiplier to use for training. The fine-tuning learning rate is the original learning rate used for pretraining multiplied by this value. 
        /// </summary>
        IFineTuneSpec01 LearningRateMultiplier(double learningRateMultiplier);

        /// <summary>
        /// Sets the weight to use for loss on the prompt tokens. This controls how much the model tries to learn to generate the prompt (as compared to the completion which always has a weight of 1.0), and can add a stabilizing effect to training when completions are short. Defaults to 0.01.
        /// </summary>
        IFineTuneSpec01 PromptLossWeight(double promptLossWeight);

        /// <summary>
        /// Sets whether to compute classification-specific metrics such as accuracy and F-1 score using the validation set at the end of every epoch. These metrics can be viewed in the results file. Defaults to false.
        /// </summary>
        IFineTuneSpec01 ComputeClassificationMetrics(bool computeClassificationMetrics);

        /// <summary>
        /// Sets the number of classes in a classification task. This parameter is required for multiclass classification. Defaults to null.
        /// </summary>
        IFineTuneSpec01 ClassificationNClasses(int classificationNClasses);

        /// <summary>
        /// Sets the positive class in binary classification. This parameter is needed to generate precision, recall, and F1 metrics when doing binary classification. Defaults to null.
        /// </summary>
        IFineTuneSpec01 ClassificationPositiveClass(string classificationPositiveClass);

        /// <summary>
        /// Sets the beta values to use for calculating F-beta scores. The F-beta score is a generalization of F-1 score. This is only used for binary classification. Defaults to null.
        /// </summary>
        IFineTuneSpec01 ClassificationBetas(params double[] betas);

        /// <summary>
        /// Sets a string of up to 40 characters (must be lower case) that will be added to your fine-tuned model name. For example, a suffix of "custom-model-name" would produce a model name like ada:ft-your-org:custom-model-name-2022-02-15-04-21-04. Defaults to null.
        /// </summary>
        /// <remarks>OpenAI API converts the suffix to lowercase</remarks>
        IFineTuneSpec01 ModelSuffix(string modelSuffix);

        /// <summary>
        /// If you provide validation data, the data is used to generate validation metrics periodically during fine-tuning. These metrics can 
        /// be viewed in the fine-tuning results file. Your training and validation data should be mutually exclusive.
        /// </summary>
        IFineTuneSpec01 ValidationData(List<FineTunePair> validationData);

        /// <summary>
        /// Sets the prompt suffix to use with the training and validation data. Defaults to '\n\n###\n\n' (see remarks)
        /// </summary>
        /// <remarks> See https://platform.openai.com/docs/guides/fine-tuning/preparing-your-dataset for more details</remarks>
        IFineTuneSpec01 PromptSuffix(string promptSuffix);

        /// <summary>
        /// Sets the completion suffix to use with training and validation data. All completions are automatically prefixed with a space and suffixed with the completion suffix. Defaults to '###' (see remarks)
        /// </summary>
        /// <remarks> See https://platform.openai.com/docs/guides/fine-tuning/preparing-your-dataset for more details</remarks>
        IFineTuneSpec01 CompletionSuffix(string completionSuffix);
    }
}
