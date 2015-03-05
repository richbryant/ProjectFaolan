using System;
using System.Linq;
using LibFaolan.Database;
using LibFaolan.Network;
using LibFaolan.Other;

namespace LibFaolan.Data
{
    public sealed class Account
    {
        public static readonly string AuthChallenge = "2bac5cd78ee0e5a37395991bfbeeeab8";
        public UInt32 AuthStatus;
        public Character Character;
        public UInt32 ClientInstance;
        public UInt32 Cookie;
        public int Counter, State;
        public UInt32 Id; // PlayerInstance
        public byte Kind; // 0 = user, 1 = admin
        public string Username;
        public UInt64 LongId => (0x0000271200000000u + Id); // As used by the client (why?)

        public void LoadDetailsFromDatabase(IDatabase database)
        {
            dynamic obj;
            if (Id != 0)
                obj = database.ExecuteDynamic("SELECT * FROM accounts WHERE account_id='" + Id + "'").FirstOrDefault();
            else if (Username != null)
                obj =
                    database.ExecuteDynamic("SELECT * FROM accounts WHERE username='" + Username + "'").FirstOrDefault();
            else
                throw new Exception("Id == 0 && Username == null");

            if (obj == null)
                throw new Exception("obj == null");

            Id = obj["account_id"];
            // AuthStatus = 
            Cookie = obj["cookie"];
            Username = obj["username"];
            Kind = obj["kind"];
            ClientInstance = 0x0802E5D4;
        }

        public bool IsBanned(IDatabase database)
        {
            return database.ExecuteScalar<int>("SELECT state FROM accounts WHERE account_id = '" + Id + "'") == 1;
        }

        public Character[] GetCharacters(IDatabase database)
        {
            var chars = database.ExecuteDynamic("SELECT character_id FROM characters WHERE account_id = '" + Id + "'")
                .Select(c => new Character((UInt32) c["character_id"])).ToArray();

            foreach (var c in chars)
                c.LoadDetailsFromDatabase(database);

            return chars;
        }

        public bool UpdateLastInfo(IDatabase database, NetworkClient client)
        {
            return database.ExecuteNonQuery("UPDATE accounts SET last_connection='" +
                                            Functions.SecondsSindsEpoch() + "', last_ip='" + client.IpAddress +
                                            "' WHERE " +
                                            "account_id='" + Id + "'") == 1;
        }
    }
}