using Faolan.Core.Database;
using Faolan.Core.Math;

namespace Faolan.Core.Data
{
    public sealed class ConanMap
    {
        public uint Id;
        public string Name;
        public Vector3 Position;
        public Vector3 Rotation;
        public static ConanMap[] AllMaps { get; private set; }

        public static void Init(IDatabase database)
        {
            /*AllMaps = database.ExecuteReader("SELECT * FROM maps").ToIEnumerable().Select(c => new ConanMap
            {
                Id = (uint) c["map_id"],
                Name = (string) c["map_name"],
                Position = new Vector3((float) c["pos_x"], (float) c["pos_y"], (float) c["pos_z"]),
                Rotation = new Vector3((float) c["rot_x"], (float) c["rot_y"], (float) c["rot_z"])
            }).ToArray();*/
        }

        public override string ToString()
        {
            return $"{Id}: {Name}";
        }
    }
}