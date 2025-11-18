using System;
using System.Collections.Generic;

namespace CorarlERP.Sessions.Dto
{
    public class ApplicationInfoDto
    {
        public string Version { get; set; }

        public DateTime ReleaseDate { get; set; }

        public Dictionary<string, bool> Features { get; set; }

        public string Name { get; set; }
        public string Logo { get; set; }
        public string Icon { get; set; }
        public string TermAgreement { get; set; }
        public string PrivacyPolicy { get; set; }
        public string SetupImage { get; set; }
        public bool EnableSetup { get; set; }
        public string SignUpBackroundImageColor { get; set; }
        public string FacebookURL { get; set; }
        public string TelegramURL { get; set; }
        public string YoutubeURL { get; set; }
        public string TutorialURL { get; set; }
        public bool TawkEnable { get; set; }
    }
}