using System;
using System.Threading;
using System.Threading.Tasks;

namespace DoubanSpider.Library
{
    public sealed class ActionScheduler : IScheduler
    {
        private CancellationTokenSource _token = null;
        public void Start(TimeSpan interval, Action action)
        {
            Start(interval, t =>
            {
                if (!t.IsCancellationRequested)
                {
                    action();
                }
            });
        }

        public void Start(TimeSpan interval, Action<CancellationToken> action)
        {
            Start(interval, t =>
            {
                action(t);
                return Task.FromResult(true);
            });
        }

        public void Start(TimeSpan interval, Func<Task> action)
        {
            Start(interval, t => t.IsCancellationRequested ? action() : Task.FromResult(true));
        }

        public void Start(TimeSpan interval, Func<CancellationToken, Task> action)
        {
            if (interval.TotalSeconds == 0)
            {
                throw new ArgumentException("interval must be > 0 seconds", "interval");
            }

            if (_token != null)
            {
                throw new InvalidOperationException("Scheduler is already started.");
            }

            _token = new CancellationTokenSource();

            RunScheduler(interval, action, _token);
        }

        private static void RunScheduler(TimeSpan interval, Func<CancellationToken, Task> action, CancellationTokenSource token)
        {
            Task.Factory.StartNew(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(interval, token.Token).ConfigureAwait(false);
                        try
                        {
                            await action(token.Token).ConfigureAwait(false);
                        }
                        catch (Exception x)
                        {
                            token.Cancel();

                            //TODO 异常处理
                        }
                    }
                    catch (TaskCanceledException) { }
                }
            }, token.Token);
        }

        public void Stop()
        {
            if (_token != null)
            {
                _token.Cancel();
            }
        }

        public void Dispose()
        {
            if (_token != null)
            {
                _token.Cancel();
                _token.Dispose();
            }
        }
    }
}
