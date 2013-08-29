using System;

namespace Pierce.Net
{
    public class RetryPolicy
    {
        public int CurrentRetryCount { get; private set; }
        public int CurrentTimeoutMs { get; private set; } // XXX TimeSpan?

        private readonly int _max_retries;
        private readonly double _backoff_multiplier;

        public RetryPolicy(int initial_timeout_ms = 2500, int max_retries = 1, double backoff_multiplier = 1)
        {
            CurrentTimeoutMs = initial_timeout_ms;
            _max_retries = max_retries;
            _backoff_multiplier = backoff_multiplier;
        }

        public void Retry(Error error)
        {
            CurrentRetryCount++;
            CurrentTimeoutMs += (int)Math.Round(CurrentTimeoutMs * _backoff_multiplier);

            if (CurrentRetryCount > _max_retries)
            {
                throw error;
            }
        }
    }
}

