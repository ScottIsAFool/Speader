using PropertyChanged;

namespace Speader.Model
{
    [ImplementPropertyChanged]
    public class TeamMember
    {
        public string AvatarUri { get; set; }
        public string Name { get; set; }
        public string Twitter { get; set; }
        public string TwitterUrl { get; set; }
    }
}
