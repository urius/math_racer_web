using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Extensions
{
    public static class UniTaskExtensions
    {
        // Method that throws an exception if the token is canceled
        public static async UniTask WithCancellation(this UniTask task, CancellationToken token)
        {
            var tcs = new UniTaskCompletionSource();
            await using (token.Register(() => tcs.TrySetResult()))
            {
                var completedTaskIndex = await UniTask.WhenAny(tcs.Task, task);
                if (completedTaskIndex == 0)
                {
                    throw new OperationCanceledException(token);
                }
                await task; // Ensure the original task completes
            }
        }

        // Method that completes the UniTask without throwing an exception if the token is canceled
        public static async UniTask WithCancellationSafe(this UniTask task, CancellationToken token)
        {
            var tcs = new UniTaskCompletionSource();
            await using (token.Register(() => tcs.TrySetResult()))
            {
                await UniTask.WhenAny(tcs.Task, task).SuppressCancellationThrow();
            }
        }

        // Method that throws an exception if the token is canceled for UniTask<T>
        public static async UniTask<T> WithCancellation<T>(this UniTask<T> task, CancellationToken token)
        {
            var tcs = new UniTaskCompletionSource<T>();
            await using (token.Register(() => tcs.TrySetResult(default)))
            {
                var completedTaskResult = await UniTask.WhenAny(tcs.Task, task);
                if (completedTaskResult.winArgumentIndex == 0)
                {
                    throw new OperationCanceledException(token);
                }
                return await task; // Ensure the original task completes
            }
        }

        // Method that completes the UniTask<T> without throwing an exception if the token is canceled
        public static async UniTask<T> WithCancellationSafe<T>(this UniTask<T> task, CancellationToken token)
        {
            var tcs = new UniTaskCompletionSource<T>();
            await using (token.Register(() => tcs.TrySetResult(default)))
            {
                var completedTaskResult = await UniTask.WhenAny(tcs.Task, task).SuppressCancellationThrow();
                if (completedTaskResult.Result.winArgumentIndex == 0)
                {
                    return default; // Task was canceled, complete safely
                }
                return completedTaskResult.Result.result2; // Ensure the original task completes
            }
        }
    }
}
