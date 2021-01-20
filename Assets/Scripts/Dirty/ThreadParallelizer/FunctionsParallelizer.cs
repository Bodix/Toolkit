using System;
using System.Threading;
using System.Threading.Tasks;

public static class FunctionsParallelizer
{
    public static async Task Process(Action[] operations, int threadCount = 0)
    {
        if (threadCount == 0)
            threadCount = operations.Length;

        for (int i = 0; i < threadCount; i++)
        {
            Task task = Task.Run(operations[i]);
        }
    }


    public static async Task<TResult[]> Process<TResult>(Func<TResult> operations, uint threadCount = 0)
    {
        return null;
    }
}