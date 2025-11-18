namespace CorarlERP
{
    public interface IAppFolders
    {
        string SampleProfileImagesFolder { get; }

        string WebLogsFolder { get; set; }


        string TempFileDownloadFolder { get; }
        string TemplateFolder { get; }
        string DownloadBaseUrl { get; }
    }
}