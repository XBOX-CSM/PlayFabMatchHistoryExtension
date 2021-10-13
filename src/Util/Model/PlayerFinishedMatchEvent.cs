using System;
using System.Collections.Generic;

namespace Util.Model
{
    public class PlayerFinishedMatchEvent
    {
        public string EventName { get; set; }
        public string EventNamespace { get; set; }
        public string Source { get; set; }
        public string EntityType { get; set; }
        public string TitleId { get; set; }
        public string EntityId { get; set; }
        public string EventId { get; set; }
        public string SourceType { get; set; }
        public DateTime Timestamp { get; set; }
        public IList<object> History { get; set; }
        public PlayFabEnvironment PlayFabEnvironment { get; set; }
        public MatchData MatchData { get; set; }
    }
}