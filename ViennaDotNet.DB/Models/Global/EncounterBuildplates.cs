using Newtonsoft.Json;
using ViennaDotNet.Common.Utils;

namespace ViennaDotNet.DB.Models.Global;

public sealed class EncounterBuildplates
{
    [JsonProperty]
    private readonly Dictionary<string, EncounterBuildplate> encounterBuildplates = [];

    public EncounterBuildplates()
    {
    }

    public EncounterBuildplate? getEncounterBuildplate(string id)
    {
        return encounterBuildplates.GetOrDefault(id);
    }

    public sealed class EncounterBuildplate
    {
        public readonly int size;
        public readonly int offset;
        public readonly int scale;

        public readonly string serverDataObjectId;

        public EncounterBuildplate(int size, int offset, int scale, string serverDataObjectId)
        {
            this.size = size;
            this.offset = offset;
            this.scale = scale;

            this.serverDataObjectId = serverDataObjectId;
        }
    }
}
