using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Torch.Server.Utils
{
    public sealed class ThreadPoolTask : INotifyCompletion
    {
        public ThreadPoolTask()
        {
        }

        public bool IsCompleted { get; private set; }
        public ThreadPoolTask GetAwaiter() => this;

        public void GetResult()
        {
        }

        public void OnCompleted(Action continuation)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                continuation?.Invoke();
                IsCompleted = true;
            });
        }
    }
}