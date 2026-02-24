namespace AktWeb.Functions.Extensions;

public static class SemaphoreSlimExtensions
{
    private sealed class Releaser : IDisposable
    {
        private SemaphoreSlim? _toRelease;
        public Releaser(SemaphoreSlim toRelease) => _toRelease = toRelease;
        public void Dispose() => Interlocked.Exchange(ref _toRelease, null)?.Release();
    }

    public static async Task<IDisposable> LockAsync(this SemaphoreSlim sem, CancellationToken ct)
    {
        await sem.WaitAsync(ct).ConfigureAwait(false);
        return new Releaser(sem);
    }
}

