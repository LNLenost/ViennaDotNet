using Newtonsoft.Json;
using ViennaDotNet.Common.Utils;

namespace ViennaDotNet.DB.Models.Global;

public sealed class SharedBuildplates
{
    [JsonProperty]
    private readonly Dictionary<string, SharedBuildplate> sharedBuildplates = [];
    public void addSharedBuildplate(string id, SharedBuildplate buildplate)
    {
        sharedBuildplates[id] = buildplate;
    }

    public SharedBuildplate? getSharedBuildplate(string id)
    {
        return sharedBuildplates.GetOrDefault(id);
    }

    public sealed class SharedBuildplate
    {
        public readonly string playerId;

        public readonly int size;
        public readonly int offset;
        public readonly int scale;

        public readonly bool night;

        public readonly long created;
        public readonly long buildplateLastModifed;
        public long lastViewed;
        public int numberOfTimesViewed;

        public readonly HotbarItem?[] hotbar;

        public readonly string serverDataObjectId;

        public SharedBuildplate(string playerId, int size, int offset, int scale, bool night, long created, long buildplateLastModifed, string serverDataObjectId)
        {
            this.playerId = playerId;

            this.size = size;
            this.offset = offset;
            this.scale = scale;

            this.night = night;

            this.created = created;
            this.buildplateLastModifed = buildplateLastModifed;
            this.lastViewed = 0;
            this.numberOfTimesViewed = 0;

            this.hotbar = new HotbarItem[7];

            this.serverDataObjectId = serverDataObjectId;
        }

        public sealed record HotbarItem(
                string uuid,
                int count,
                string? instanceId,
                int wear
        );
    }
}
