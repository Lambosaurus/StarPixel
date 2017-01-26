using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigLoader
{
    public static class Configuration
    {
        static public ConfigModule global = new ConfigModule();
    }

    public class ConfigModule
    {
        private static Dictionary<Type, ConfigModule> modules = new Dictionary<Type, ConfigModule>();

        private static string configDirectory = @"..\StarPixelContent";

        //Purely cos he's the only figure that sounds like a watchman
        private static configWatcher batMan = new configWatcher(configDirectory);

        public static StarPixelDirectories Directories = new StarPixelDirectories(); 

        private static object modLock = new object();
        public ConfigModule()
        {
            //Lock, add module to dictionary if it's not already there, and register with config watcher
            lock (modLock)
            {
                if(!modules.ContainsKey(GetType()))
                {
                    modules.Add(GetType(), this);
                    loadConfigModule();
                }
                else
                {
                    throw new Exception("You've already loaded one of these bruv" + GetType().ToString());
                }
            }
        }

        private static object loadLock = new object();
        private void loadConfigModule()
        {
            lock(loadLock)
            {
                FieldInfo[] fields = this.GetType().GetFields();
                foreach (FieldInfo f in fields)
                {
                    string file = JsonLoader.loadField(Directories.config + configDirectory, this, f);
                    try
                    {
                        //Register the watcher
                        batMan.Register(file, this, f);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("File loading exploded", ex);
                    }
                }
            }
        }

        public string ConfigDir
        {
            get
            {
                return configDirectory;
            }
        }

        private class configWatcher
        {
            private struct ModFieldPair
            {
                public ConfigModule mod;
                public FieldInfo field;

                public ModFieldPair(ConfigModule m, FieldInfo f)
                {
                    mod = m;
                    field = f;
                }
            }

            private FileSystemWatcher sentinel;
            private Dictionary<string, ModFieldPair> configReloadDir;
            private string configDir;
            public configWatcher(string confDir, string ext = ".json")
            {
                configReloadDir = new Dictionary<string, ModFieldPair>();
                sentinel = new FileSystemWatcher(confDir, "*" + ext);
                configDir = confDir;
                sentinel.Changed += ConfigFileChanged;
                sentinel.Deleted += ConfigFileChanged;
                sentinel.EnableRaisingEvents = true;
            }

            public void Register(string file, ConfigModule module, FieldInfo field)
            {
                if (configReloadDir.ContainsKey(file))
                {
                    throw new Exception("File is already being watched: " + module.GetType() + "." + field.FieldType);
                }
                else
                {
                    configReloadDir.Add(file, new ModFieldPair(module, field));
                }
            }

            private void ConfigFileChanged(object sender, FileSystemEventArgs e)
            {
                ModFieldPair p;
                if (configReloadDir.TryGetValue(e.Name, out p))
                {
                    lock (modules)
                    {
                        JsonLoader.loadField(configDirectory, p.mod, p.field);
                    }
                }
                else
                {
                    /// No config file, do nothing.
                }
            }
        }

        public class StarPixelDirectories : ConfigGroup
        {
            public string config = Environment.CurrentDirectory;

            protected override void SetDefaults()
            {
                if (!Directory.Exists(config + configDirectory))
                {
                    Directory.CreateDirectory(config + configDirectory);
                }
            }
        }
    }

    /// <summary>
    /// A base class for storing a minimal group of configuration variables.
    /// </summary>
    public abstract class ConfigGroup
    {

        /// <summary>
        /// Set to true when generating the default set of values for configuration
        /// </summary>
        public bool DefaultConfig = true;

        /// <summary>
        /// Call to reset the configuration to hard-coded defaults
        /// </summary>
        public void SetToDefault()
        {
            SetDefaults();
            DefaultConfig = true;
        }

        /// <summary>
        /// Genrate default values for the configuration
        /// </summary>
        protected abstract void SetDefaults();

        /// <summary>
        /// Invoked when the file is [Re]Loaded so that things can be instantiated with loaded data.
        /// </summary>
        public virtual void ConfigChanged()
        {

        }
    }

    public static class JsonLoader
    {
        //Get ready for dark magic, we want this to be able to dig into a sub folder based
        //on the field type, and then use the config group for individual files.
        //Hold onto your hats.
        public static string loadField(string dir, ConfigModule confMod, FieldInfo field)
        {
            //TODO: The actual loading part
            return "";
        }

        private static void JsonToConfGroup(string dir, ConfigModule m, FieldInfo field)
        {

        }

        //TODO: Determine if we want to do the other way and save to a new file as well
    }
}
