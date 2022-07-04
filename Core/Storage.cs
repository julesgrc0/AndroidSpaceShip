using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

using AndroidEnv = Android.OS.Environment;

namespace AndroidSpaceShip.Core
{
    public class Storage
    {
        public string gameFile { get; private set; }
        public bool Loading { get; private set; } = true;
        public bool StorageOK { get; private set; } = false;
        

        public Storage()
        {
            this.gameFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "game_data.dat");
        }

        public bool Init()
        {
            bool isReadonly = AndroidEnv.MediaMountedReadOnly.Equals(AndroidEnv.ExternalStorageState);
            bool isWriteable = AndroidEnv.MediaMounted.Equals(AndroidEnv.ExternalStorageState);

            if (!isReadonly && isWriteable)
            {
                this.StorageOK = true;

                if (!File.Exists(this.gameFile))
                {
                    File.WriteAllText(this.gameFile, JsonConvert.SerializeObject(new JObject()));
                    this.Loading = false;

                    return true;
                }
                else
                {
                    this.Loading = false;

                    return false;
                }
            }
            else
            {
                try
                {
                    File.WriteAllText(this.gameFile, JsonConvert.SerializeObject(new JObject()));
                    this.Loading = false;
                    this.StorageOK = true;
                    return true;
                }
                catch
                {
                    this.Loading = false;
                    this.StorageOK = false;
                    return false;
                }
            }
        }

        public void Write(GameSettings settings)
        {
            if(this.StorageOK && !this.Loading)
            {
                File.WriteAllText(this.gameFile, JsonConvert.SerializeObject(settings));
            }
        }

        public JObject Read()
        {
            if (this.StorageOK && !this.Loading)
            {
                try
                {
                    string content = File.ReadAllText(this.gameFile);
                    return JObject.Parse(content);
                }
                catch
                {
                    this.StorageOK = false;
                    File.Delete(this.gameFile);
                }
            }

            return new JObject();
        }
    }
}