using System;
using System.Threading;
using System.Threading.Tasks;

namespace CheckUrls
{
    /// <summary>
    /// Queue to tasks
    /// </summary>
    public class TaskQueue
    {
        private SemaphoreSlim semaphore;
        public TaskQueue(int maxThreads)
        {
            semaphore = new SemaphoreSlim(maxThreads);
        }

        public async Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
        {
            await semaphore.WaitAsync();
            try
            {
                return await taskGenerator();
            }
            finally
            {
                semaphore.Release();
            }
        }

        /// <summary>
        /// Set to queue action
        /// </summary>
        /// <param name="taskGenerator">Action</param>
        public async Task Enqueue(Func<Task> taskGenerator)
        {
            await semaphore.WaitAsync();
            try
            {
                await taskGenerator();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
