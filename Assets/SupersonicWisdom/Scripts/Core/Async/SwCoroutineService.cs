using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal delegate IEnumerator SwAsyncMethod ();

    internal class SwCoroutineService
    {
        #region --- Members ---

        private readonly MonoBehaviour _runner;

        #endregion


        #region --- Construction ---

        public SwCoroutineService(MonoBehaviour runner)
        {
            _runner = runner;
        }

        #endregion


        #region --- Public Methods ---

        public IEnumerator Try(IEnumerator enumerator, SwAsyncCallbackWithException callback)
        {
            while (true)
            {
                object current;

                try
                {
                    if (enumerator.MoveNext() == false)
                    {
                        callback?.Invoke(null);

                        break;
                    }

                    current = enumerator.Current;
                }
                catch (Exception ex)
                {
                    SwInfra.Logger.LogException(ex, EWisdomLogType.CoroutineService, $"{nameof(enumerator.MoveNext)} - {nameof(Try)}");
                    callback?.Invoke(ex);

                    yield break;
                }

                yield return current;
            }
        }

        public Coroutine RunThrottledForever(Action callback, int frameInterval = 10)
        {
            return _runner.StartCoroutine(RunThrottledForeverCoroutine(callback, frameInterval));
        }

        public Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return _runner.StartCoroutine(coroutine);
        }
        
        public Coroutine StartCoroutineAfterDelay(IEnumerator coroutine, float delay)
        {
            return _runner.StartCoroutine(delay == 0 ? coroutine : DelayCoroutine(coroutine, delay));
        }

        private IEnumerator DelayCoroutine(IEnumerator coroutine, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            _runner.StartCoroutine(coroutine);
        }

        public Coroutine StartCoroutineWithCallback(SwAsyncMethod getCoroutine, Action callback)
        {
            var startedCoroutine = StartCoroutine(StartCoroutineAndRunCallback(getCoroutine.Invoke(), callback));

            return startedCoroutine;
        }

        public Coroutine StartCoroutineWithCallback(SwAsyncMethod getCoroutine, SwAsyncCallbackWithException callback)
        {
            var startedCoroutine = StartCoroutine(StartCoroutineAndRunCallback(getCoroutine.Invoke(), callback));

            return startedCoroutine;
        }

        public void StopCoroutine(Coroutine coroutine)
        {
            try
            {
                if (coroutine != null)
                { 
                    _runner.StopCoroutine(coroutine);
                }
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogException(e, EWisdomLogType.CoroutineService, $"{nameof(Try)} - {nameof(StopCoroutine)}");
            }
        }
        
        public Coroutine RunActionEndlessly(Action action, float secondsInterval, Func<bool> exitCondition)
        {
            return _runner.StartCoroutine(RunEndlessly(action, secondsInterval, exitCondition));
        }
        
        public Coroutine StartWithTimeout(SwAsyncMethod getCoroutine, Func<float> getReleaseTimeout, float gracePeriod, float terminationTimeout = float.MaxValue, Action onTimeout = null, SwAsyncCallbackWithException onComplete = null)
        {
            return _runner.StartCoroutine(WithTimeout(getCoroutine.Invoke(), getReleaseTimeout, gracePeriod, terminationTimeout, onTimeout, onComplete));
        }
        
        #endregion


        #region --- Private Methods ---

        private static IEnumerator StartCoroutineAndRunCallback(IEnumerator coroutine, Action callback)
        {
            yield return coroutine;
            callback?.Invoke();
        }

        private IEnumerator StartCoroutineAndRunCallback(IEnumerator coroutine, SwAsyncCallbackWithException callback)
        {
            yield return Try(coroutine, callback);
        }

        private IEnumerator WaitForSecondsInternal(float seconds, [NotNull] Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback.Invoke();
        }

        internal Coroutine WaitForSeconds(float seconds, [NotNull] Action action)
        {
            return StartCoroutine(WaitForSecondsInternal(seconds, action));
        }

        private IEnumerator RunThrottledForeverCoroutine(Action callback, int frameInterval = 10)
        {
            do
            {
                callback.Invoke();

                for (var i = 0; i < frameInterval; i++)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            while (true);
            // ReSharper disable once IteratorNeverReturns
        }
        
        private IEnumerator RunEndlessly(Action action, float secondsInterval, Func<bool> exitCondition)
        {
            var waitForSeconds = new WaitForSeconds(secondsInterval);
            
            while (!exitCondition())
            {
                action?.Invoke();

                yield return waitForSeconds;
            }
        }
        
        private IEnumerator WithTimeout(IEnumerator coroutine, Func<float> getReleaseTimeout, float gracePeriod, float terminationTimeout = float.MaxValue, Action onTimeout = null, SwAsyncCallbackWithException onComplete = null)
        {
            var finished = false;
            Exception exception = null;

            var runningCoroutine = _runner.StartCoroutine(DoCoroutineWithException(coroutine, ex =>
            {
                finished = true;
                exception = ex;
            }));

            var terminationTimer = 0f;
            var gracePeriodTimer = 0f;
            var gracePeriodActive = false;

            try
            {
                while (terminationTimer <= terminationTimeout && !finished)
                {
                    var releaseTimeout = getReleaseTimeout?.Invoke();

                    if (releaseTimeout <= 0 && !finished)
                    {
                        if (!gracePeriodActive)
                        {
                            gracePeriodActive = true;
                            gracePeriodTimer = 0f;
                        }

                        gracePeriodTimer += Time.deltaTime;

                        if (gracePeriodTimer >= gracePeriod && onTimeout != null)
                        {
                            onTimeout();
                            onTimeout = null;
                        }
                    }
                    else
                    {
                        gracePeriodActive = false;
                    }

                    terminationTimer += Time.deltaTime;
                    yield return null;
                }
            }
            finally
            {
                // IMPORTANT - Stop the running coroutine if it has not finished within the timeout period
                if (!finished)
                {
                    _runner.StopCoroutine(runningCoroutine);
                    exception = new SwException("Coroutine exceeded termination timeout.");
                }

                onComplete?.Invoke(exception);
            }
        }

        private IEnumerator DoCoroutineWithException(IEnumerator coroutine, SwAsyncCallbackWithException onComplete)
        {
            yield return Try(coroutine, onComplete);
        }

        #endregion
    }
}