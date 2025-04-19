using System;
using System.Diagnostics;

namespace SupersonicWisdomSDK
{
    // This class is used to measure the time taken for a block of code to execute.
    // Usage Example - 'using var timer = new SwLoggerTimer(EWisdomLogType.Test, "Some message");'
    internal class SwLoggerTimer : IDisposable
    {
        #region --- Members ---

        private readonly string _message;
        private readonly Stopwatch _stopwatch;
        private readonly EWisdomLogType _logType;

        #endregion


        #region --- Construction ---

        public SwLoggerTimer(EWisdomLogType logType, string message)
        {
            _message = message;
            _logType = logType;
            _stopwatch = Stopwatch.StartNew();
            SwInfra.Logger.Log(logType, $"Start: {_message}");
        }

        #endregion


        #region --- Public Methods ---

        public void Dispose()
        {
            _stopwatch.Stop();
            SwInfra.Logger.Log(_logType, $"End: {_message}, Time: {_stopwatch.ElapsedMilliseconds}ms");
        }

        #endregion
    }
}