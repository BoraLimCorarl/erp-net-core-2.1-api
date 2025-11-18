using Abp.Dependency;

namespace CorarlERP
{
    public class AppFolders : IAppFolders, ISingletonDependency
    {
        public string SampleProfileImagesFolder { get; set; }

        public string WebLogsFolder { get; set; }


        public string TempFileDownloadFolder { get; set; }
        public string TemplateFolder { get; set; }
        public string FontFolder { get; set; }
        public string DownloadBaseUrl { get; set; }
    }
}