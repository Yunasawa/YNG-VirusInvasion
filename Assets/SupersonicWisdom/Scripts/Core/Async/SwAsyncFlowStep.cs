namespace SupersonicWisdomSDK
{
    internal sealed class SwAsyncFlowStep
    {
        #region --- Private Members ---

        private readonly ISwAsyncRunnable _runnable;

        #endregion


        #region --- Properties ---

        public int ExecutionIndex { get; }
        public int? MaxExecutionIndex { get; }
        public SwAsyncMethod RunMethod { get; }
        public bool IsEndless { get; } // Endless is not supported if IsBlocking is true
        public bool IsBlocking { get;}
        public float TerminationTimeout { get; } = float.MaxValue;

        #endregion


        #region --- Construction ---

        internal SwAsyncFlowStep(ISwAsyncRunnable runnable, int executionIndex, bool isBlocking = false, int? maxExecutionIndex = null, bool isEndless = true)
        {
            _runnable = runnable;
            RunMethod = runnable.Run;
            ExecutionIndex = executionIndex;
            MaxExecutionIndex = maxExecutionIndex;
            IsBlocking = isBlocking;
            IsEndless = !isBlocking && isEndless;
        }

        internal SwAsyncFlowStep(SwAsyncMethod customRun, int executionIndex, bool isBlocking = false, int? maxExecutionIndex = null, bool isEndless = true)
        {
            RunMethod = customRun;
            ExecutionIndex = executionIndex;
            MaxExecutionIndex = maxExecutionIndex;
            IsBlocking = isBlocking;
            IsEndless = !isBlocking && isEndless;
        }

        internal void OnTimeout()
        {
            _runnable?.OnTimeout();
        }

        #endregion
    }
}