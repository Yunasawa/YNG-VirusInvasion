using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwAsyncFlow : ISwAsyncCatchableRunnable
    {
        #region --- Members ---

        protected readonly SwTimeoutHandler _timeoutHandler;
        private readonly Dictionary<int, int> _runningStepsByIndex = new Dictionary<int, int>();
        private readonly int _maxStepIndex;
        private readonly List<SwAsyncFlowStep> _steps;
        private int _currentIndex;
        private int _backgroundTasksRunning;
        private Exception _runtimeException;
        private bool _isBlockingStepRunning;
        private readonly List<string> _timeoutObjectsNames;

        #endregion
        
        
        #region --- Properties ---
        
        internal SwTimeoutHandler TimeoutHandler
        {
            get { return _timeoutHandler; }
        }

        #endregion


        #region --- Construction ---

        public SwAsyncFlow(GameObject mono, SwAsyncFlowStep[] steps)
        {
            _timeoutObjectsNames = new List<string>();
            _steps = steps.OrderBy(step => step.ExecutionIndex).ToList();
            _currentIndex = _steps.FirstOrDefault()?.ExecutionIndex ?? 0;
            _maxStepIndex = _steps.LastOrDefault()?.ExecutionIndex ?? 0;
            _timeoutHandler = new SwTimeoutHandler(mono);
        }

        #endregion


        #region --- Public Methods ---

        public IEnumerator Run(SwAsyncCallbackWithException callback = null)
        {
            _runtimeException = null;
            var didFail = false;

            try
            {
                Validate();
            }
            catch (SwException ex)
            {
                didFail = true;
                _runtimeException = ex;
            }

            _timeoutHandler.StartTimer();

            if (!didFail)
            {
                var steps = new List<SwAsyncFlowStep>(_steps);

                while (steps.Any())
                {
                    var stepsToRunNow = steps.Where(step =>
                        step.ExecutionIndex == _currentIndex).ToList();

                    steps = steps.Except(stepsToRunNow).ToList();

                    if (stepsToRunNow.Any())
                    {
                        while (_timeoutHandler.IsTimerPaused || _isBlockingStepRunning) // Certain steps may pause the timeout-timer (like UTK, att, blocking steps, etc.) in such a case we will not start new steps.
                        {
                            yield return null;
                        }
                        
                        StartStepsInParallel(stepsToRunNow);
                        yield return WaitForCurrentIndexStepsToComplete();
                    }

                    _currentIndex++;
                }

                while (_backgroundTasksRunning > 0)
                {
                    yield return null;
                }
            }

            _timeoutHandler.ResetTimer();

            if (didFail)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Async, $"{nameof(SwAsyncFlow)} | Run | Error");
            }

            callback?.Invoke(_runtimeException);
        }
        
        public string GetTimeoutObjectsStatus()
        {
            if(_timeoutObjectsNames.SwIsNullOrEmpty()) return string.Empty;
            
            try 
            {
                var timeoutObjectNamesStringify = SwUtils.JsonHandler.SerializeObject(_timeoutObjectsNames);
                
                return timeoutObjectNamesStringify;
            }
            catch (Exception ex)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Async, $"Failed to stringify timeout objects names with exception {ex}");
                
                return string.Empty;
            }
        }

        #endregion


        #region --- Private Methods ---

        private void StartStepsInParallel(List<SwAsyncFlowStep> steps)
        {
            foreach (var step in steps)
            {
                _backgroundTasksRunning++;
                IncrementRunningStepsByIndex(_currentIndex);

                if (step.IsBlocking) // Blocking steps are executed without timeout.
                {
                    _isBlockingStepRunning = true;
                    SwInfra.Logger.Log(EWisdomLogType.Async, $"Step at index {step.ExecutionIndex} ({step.RunMethod.Target}) started in blocking mode.");
                    _timeoutHandler.PauseTimer();

                    SwInfra.CoroutineService.StartCoroutineWithCallback(() => RunStep(step), exception =>
                    {
                        OnStepComplete(step, exception);
                    });

                    continue;
                }

                var coroutine = SwInfra.CoroutineService.StartWithTimeout(
                    () => RunStep(step),
                    () => _timeoutHandler.TimeUntilTimeout,
                    SwTimeoutHandler.GRACE_TIME_AFTER_TIMEOUT,
                    step.TerminationTimeout,
                    () => HandleStepTimeout(step),
                    ex => OnStepComplete(step, ex));

                SwInfra.Logger.Log(EWisdomLogType.Async, $"Step at index {step.ExecutionIndex} ({step.RunMethod.Target}) started with timeout {_timeoutHandler.TimeUntilTimeout} seconds.");
            }
        }

        private IEnumerator RunStep(SwAsyncFlowStep step)
        {
            IEnumerator method = null;

            try
            {
                method = step.RunMethod.Invoke();
            }
            catch (Exception ex)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Async,
                    $"Failed to run step at index={step.ExecutionIndex} with exception {ex}");
            }

            while (method != null && method.MoveNext())
            {
                yield return method.Current;
            }
        }

        private void OnStepComplete(SwAsyncFlowStep step, Exception exception)
        {
            if (exception != null)
            {
                SwInfra.Logger.LogWarning(EWisdomLogType.Async, $"Step at index {step.ExecutionIndex} ({step.RunMethod.Target}) completed with exception {exception}");
                _runtimeException = exception;
            }
            else
            {
                SwInfra.Logger.Log(EWisdomLogType.Async, $"Step at index {step.ExecutionIndex} ({step.RunMethod.Target}) completed.");
            }
            
            if (step.IsBlocking)
            {
                SwInfra.Logger.Log(EWisdomLogType.Async, $"Step at index {step.ExecutionIndex} ({step.RunMethod.Target}) completed in blocking mode.");
                _timeoutHandler.ResumeTimer();
                _isBlockingStepRunning = false;
            }

            _backgroundTasksRunning--;
            DecrementRunningStepsByIndex(step.ExecutionIndex);
        }

        private void HandleStepTimeout(SwAsyncFlowStep step)
        {
            var index = step.ExecutionIndex;
            var stepName = step.RunMethod.Target.GetType().Name;
            var errorMsg = $"Step at index {index} ({stepName}) timed out.";
            
            SwInfra.Logger.LogError(EWisdomLogType.Async, errorMsg);
            _timeoutObjectsNames.Add($"{stepName},{index}");
            _backgroundTasksRunning--;

            if (step.IsEndless)
            {
                SwInfra.Logger.LogWarning(EWisdomLogType.Async, $"Starting to run {stepName} endlessly");
                step.OnTimeout();
            }

            DecrementRunningStepsByIndex(index);
        }

        private void IncrementRunningStepsByIndex(int index)
        {
            _runningStepsByIndex[index] = _runningStepsByIndex.SwSafelyGet(index, 0) + 1;
        }

        private void DecrementRunningStepsByIndex(int index)
        {
            _runningStepsByIndex[index] = _runningStepsByIndex.SwSafelyGet(index, 0) - 1;
            
            if (_runningStepsByIndex[index] <= 0)
            {
                _runningStepsByIndex.Remove(index);
            }
        }

        private IEnumerator WaitForCurrentIndexStepsToComplete()
        {
            while (_runningStepsByIndex.SwSafelyGet(_currentIndex, 0) > 0)
            {
                yield return null;
            }
        }

        private void Validate()
        {
            var stepIndexWithInvalidRange = _steps.FindIndex(step =>
                step.MaxExecutionIndex != null && step.ExecutionIndex >= step.MaxExecutionIndex);

            if (stepIndexWithInvalidRange > -1)
            {
                var invalidStep = _steps[stepIndexWithInvalidRange];

                throw new SwException(
                    $"Invalid range for step at index={stepIndexWithInvalidRange} Range={invalidStep.ExecutionIndex}-{invalidStep.MaxExecutionIndex}");
            }

            var stepIndexWithRangeOutOfBounds = _steps.FindIndex(step => step.MaxExecutionIndex > _maxStepIndex);

            if (stepIndexWithRangeOutOfBounds > -1)
            {
                var invalidStep = _steps[stepIndexWithRangeOutOfBounds];

                throw new SwException(
                    $"Range out of bounds for step at index={stepIndexWithRangeOutOfBounds} Range={invalidStep.ExecutionIndex}-{invalidStep.MaxExecutionIndex}");
            }
        }

        #endregion
    }
}