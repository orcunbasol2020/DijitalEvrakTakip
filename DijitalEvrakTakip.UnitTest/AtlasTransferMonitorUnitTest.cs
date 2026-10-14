using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Infrastructure.Atlas;

namespace DijitalEvrakTakip.UnitTest
{
    public class AtlasTransferMonitorUnitTest
    {
        private static readonly AtlasTransferRunResultDto Result = new(true, 2, 1, 1, 0, 0, "2 evrak işlendi");

        [Fact]
        public void IsEmpty_BeforeFirstRun()
        {
            var snapshot = new AtlasTransferMonitor().GetSnapshot();

            Assert.False(snapshot.IsRunning);
            Assert.Null(snapshot.RunningSince);
            Assert.Null(snapshot.LastRun);
            Assert.Null(snapshot.NextRunAt);
        }

        [Fact]
        public void ShowsRunningRun_AndKeepsPreviousLastRun()
        {
            var monitor = new AtlasTransferMonitor();
            monitor.RunStarted(manual: false);
            monitor.RunCompleted(Result);

            monitor.RunStarted(manual: true);
            var snapshot = monitor.GetSnapshot();

            Assert.True(snapshot.IsRunning);
            Assert.NotNull(snapshot.RunningSince);
            Assert.False(snapshot.LastRun!.Manual);
            Assert.Same(Result, snapshot.LastRun.Result);
        }

        [Fact]
        public void RecordsCompletedRun()
        {
            var monitor = new AtlasTransferMonitor();
            monitor.RunStarted(manual: true);
            monitor.RunCompleted(Result);

            var snapshot = monitor.GetSnapshot();

            Assert.False(snapshot.IsRunning);
            Assert.True(snapshot.LastRun!.Manual);
            Assert.Same(Result, snapshot.LastRun.Result);
            Assert.Null(snapshot.LastRun.Error);
            Assert.True(snapshot.LastRun.FinishedAt >= snapshot.LastRun.StartedAt);
        }

        [Fact]
        public void RecordsFailedRun_WithoutResult()
        {
            var monitor = new AtlasTransferMonitor();
            monitor.RunStarted(manual: false);
            monitor.RunFailed("Veritabanına bağlanılamadı");

            var lastRun = monitor.GetSnapshot().LastRun!;

            Assert.Null(lastRun.Result);
            Assert.Equal("Veritabanına bağlanılamadı", lastRun.Error);
        }

        [Fact]
        public void KeepsNextRun()
        {
            var monitor = new AtlasTransferMonitor();
            var next = new DateTime(2026, 10, 14, 9, 0, 0, DateTimeKind.Utc);

            monitor.NextRunScheduled(next);

            Assert.Equal(next, monitor.GetSnapshot().NextRunAt);
        }
    }
}
