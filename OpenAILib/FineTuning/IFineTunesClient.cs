// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.FineTuning
{
    /// <summary>
    /// Represents a high level client for fine-tuning operations - interacts with OpenAI files, fine-tunes and models endpoints
    /// </summary>
    public interface IFineTunesClient
    {
        /// <summary>
        /// Creates a new fine-tuning with the specified training data.
        /// </summary>
        /// <param name="trainingData">The training data to use for the fine-tuning operation.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains information about the created fine-tuning operation.</returns>
        Task<FineTuneInfo> CreateFineTuneAsync(List<FineTunePair> trainingData, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new fine-tuning with the specified training data and custom specifications.
        /// </summary>
        /// <param name="trainingData">The training data to use for the fine-tuning operation.</param>
        /// <param name="spec">The custom fine-tuning specifications to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains information about the created fine-tuning operation.</returns>
        Task<FineTuneInfo> CreateFineTuneAsync(List<FineTunePair> trainingData, Action<IFineTuneSpec01> spec, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the events associated with the specified fine-tune.
        /// </summary>
        /// <param name="fineTune">The fine-tuning operation to retrieve events for.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of fine-tuning events associated with the specified operation.</returns>
        Task<List<FineTuneEvent>> GetEventsAsync(FineTuneInfo fineTune, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a stream of events associated with the specified fine-tune creation operation - enumeration does not complete until the fine-tune job has ended.
        /// </summary>
        /// <param name="fineTune">The fine-tuning operation to retrieve events for.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An asynchronous stream of fine-tuning events associated with the specified operation.</returns>
        IAsyncEnumerable<FineTuneEvent> GetEventStreamAsync(FineTuneInfo fineTune, CancellationToken cancellationToken = default);


        /// <summary>
        /// Gets the status of the specified fine-tuning operation.
        /// </summary>
        /// <param name="fineTune">The fine-tuning operation to retrieve the status for.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the status of the specified fine-tuning operation.</returns>
        Task<FineTuneStatus> GetStatusAsync(FineTuneInfo fineTune, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new variant of the specified fine-tuning operation with the specified custom specifications.
        /// </summary>
        /// <param name="fineTune">The fine-tuning operation to create a variant of.</param>
        /// <param name="spec">The custom fine-tuning specifications to use" </param>
        /// <returns>A task that represents the asynchronous operation. The task result contains information about the created fine-tuning operation.</returns>
        Task<FineTuneInfo> CreateFineTuneVariantAsync(FineTuneInfo fineTune, Action<IFineTuneSpec01> spec, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the specified fine-tuning operation.
        /// </summary>
        /// <param name="fineTune">The fine-tuning operation to delete.</param>
        /// <param name="deleteTrainingFiles">When true, any training or validation files associated with the fine tune are also deleted</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteFineTuneAsync(FineTuneInfo fineTune, bool deleteTrainingFiles = false, CancellationToken cancellationToken = default);
    }
}
