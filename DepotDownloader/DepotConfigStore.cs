﻿using System;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;
using System.IO.Compression;

namespace DepotDownloader
{
    [ProtoContract]
    public class DepotConfigStore
    {
        [ProtoMember(1)]
        public Dictionary<uint, ulong> InstalledManifestIDs { get; private set; }

        string FileName = null;

        DepotConfigStore()
        {
            InstalledManifestIDs = new Dictionary<uint, ulong>();
        }

        static bool Loaded
        {
            get { return Instance != null; }
        }

        public static DepotConfigStore Instance = null;

        public static void LoadFromFile(string filename)
        {
            if (Loaded)
                return;

            if (File.Exists(filename))
            {
                using (FileStream fs = File.Open(filename, FileMode.Open))
                using (DeflateStream ds = new DeflateStream(fs, CompressionMode.Decompress))
                    Instance = ProtoBuf.Serializer.Deserialize<DepotConfigStore>(ds);
            }
            else
            {
                Instance = new DepotConfigStore();
            }

            Instance.FileName = filename;
        }

        public static void Save()
        {
            if (!Loaded)
                throw new Exception("Saved config before loading");

            using (FileStream fs = File.Open(Instance.FileName, FileMode.Create))
            using (DeflateStream ds = new DeflateStream(fs, CompressionMode.Compress))
                ProtoBuf.Serializer.Serialize<DepotConfigStore>(ds, Instance);
        }
    }
}