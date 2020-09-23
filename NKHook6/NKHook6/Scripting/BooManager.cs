﻿using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using BTD_Backend.IO;
using Ionic.Zip;
using NKHook6.Api.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NKHook6.Scripting
{
    //👻 boo
    class BooManager
    {
        static string scriptPath = "Mods";
        private static List<Thread> scriptThreads = new List<Thread>();

        /// <summary>
        /// Executes all boo scripts
        /// </summary>
        public static void ExecuteAllScripts()
        {
            if (!Directory.Exists(scriptPath))
                Directory.CreateDirectory(scriptPath);

            string[] files = Directory.GetFiles(scriptPath);
            Logger.Log("Script count: " + files.Length);
            foreach (string file in files)
            {
                if (!file.EndsWith(".boo") && !file.EndsWith(".wbp") && !file.EndsWith(".zip"))
                    continue;

                if (file.EndsWith(".boo"))
                {
                    Logger.Log("Found Boo: " + file);
                    AddToScriptThreads(file);
                    continue;
                }
                else if (file.EndsWith(".wbp") || file.EndsWith(".zip"))
                {
                    foreach (var item in GetBooFilesFromZip(file))
                        AddToScriptThreads(item.Key, item.Value);
                }
            }
        }

        public static Dictionary<string, string> GetBooFilesFromZip(string zipPath)
        {
            var result = new Dictionary<string, string>();

            Zip z = new Zip(zipPath);
            var entries = z.Archive.Entries;
            foreach (var entry in entries)
            {
                if (!entry.FileName.ToLower().EndsWith(".boo") && !entry.FileName.ToLower().EndsWith(".boo.disabled"))
                    continue;

                string name = Thread.CurrentThread.CurrentCulture.TextInfo.
                    ToTitleCase(Path.GetFileNameWithoutExtension(entry.FileName));

                string code = z.ReadFileInZip(entry.FileName);
                result.Add(name, code);
            }

            return result;
        }

        /// <summary>
        /// Add a script to the 
        /// </summary>
        /// <param name="path"></param>
        public static void AddToScriptThreads(string path)
        {
            string name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Path.GetFileNameWithoutExtension(path));
            Logger.Log("Name: " + name);
            string code = File.ReadAllText(path);
            AddToScriptThreads(name, code);
        }


        /// <summary>
        /// Compiles and executed given .boo code
        /// </summary>
        /// <param name="name">Name of the script (Same as file name if reading from a file)</param>
        /// <param name="code">Code of the script</param>
        /// <returns></returns>
        public static void AddToScriptThreads(string name, string code)
        {
            Logger.Log("Loading script: " + name);

            scriptThreads.Add(new Thread(() =>
            {
                if (!Execute(name, code))
                {
                    Logger.Log("Failed to load script " + name);
                }
            }));
            scriptThreads[scriptThreads.Count - 1].Start();
        }

        /// <summary>
        /// Executes a boo script from a file
        /// </summary>
        /// <param name="path">Path to the script</param>
        /// <returns></returns>
        public static bool ExecuteFile(string path)
        {
            string name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Path.GetFileNameWithoutExtension(path));
            Logger.Log("Name: " + name);
            string code = File.ReadAllText(path);
            return Execute(name, code);
        }

        /// <summary>
        /// Compiles and executed given .boo code
        /// </summary>
        /// <param name="name">Name of the script (Same as file name if reading from a file)</param>
        /// <param name="code">Code of the script</param>
        /// <returns></returns>
        public static bool Execute(string name, string code)
        {
            try
            {
                var compiler = new BooCompiler();
                compiler.Parameters.Input.Add(new StringInput(name, code));
                compiler.Parameters.Pipeline = new CompileToMemory();
                compiler.Parameters.Ducky = true;

                CompilerContext context = compiler.Run();
                if (context.GeneratedAssembly == null)
                {
                    foreach (CompilerError error in context.Errors)
                        Logger.Log(error.ToString());
                    return false;
                }
                Type entryModule = context.GeneratedAssembly.GetType(name + "Module");
                EventRegistry.subscriber.register(entryModule);
                MethodInfo entryMethod = entryModule.GetMethod("Entry");
                return (bool)entryMethod.Invoke(null, null);
            }
            catch (Exception ex)
            {
                Logger.Log("Exception: " + ex.Message);
                Logger.Log("StackTrace: " + ex.StackTrace);
                return false;
            }
            //return true;
        }
    }
}
