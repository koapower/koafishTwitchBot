namespace Koapower.KoafishTwitchBot.UI.BeatmapQueue
{
    enum SlotPhase
    {
        Reset,
        Setup,
        WaitForDownload,
        Downloading,
        DownloadFinished,
        DownloadFailed,
        FileOpened,
        Canceled,
    }
}
