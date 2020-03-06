using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEditor.Compilation;
using Debug = UnityEngine.Debug;


[InitializeOnLoad]
public static class UnityAssemblyHooks
{
    public const int WaitMillisecondsForFileHandle = 50000;

    static UnityAssemblyHooks()
    {
        CompilationPipeline.assemblyCompilationFinished += (path, compilerMessages) =>
        {
            var messageLength = compilerMessages.Length;
            for (var index = 0; index < messageLength; index++)
                if (compilerMessages[index].type == CompilerMessageType.Error)
                    return;

            if (File.Exists(path))
                ProcessDll(path);
        };
    }

    private static bool TryToOpenFile(string path, FileMode fileMode, FileAccess access, int millisecondsToWait, out FileStream stream)
    {
        var watch = Stopwatch.StartNew();

        while (watch.Elapsed.TotalMilliseconds < millisecondsToWait)
        {
            try
            {
                stream = File.Open(path, fileMode, access);
                return true;
            }
            catch
            {
                Thread.Sleep(10);
            }
        }

        stream = null;
        return false;
    }

    private static void ProcessDll(string dllPath)
    {
        var watch = Stopwatch.StartNew();

        if (TryToOpenFile(dllPath, FileMode.Open, FileAccess.ReadWrite, WaitMillisecondsForFileHandle, out var stream))
        {

            try
            {
                var pdbPath = Path.ChangeExtension(Path.GetFullPath(stream.Name), ".pdb");

                if (CodeWeaver.WeaveDllStream(stream, File.Exists(pdbPath)))
                {
                    watch.Stop();
                    Debug.Log($"Processed {dllPath} in {watch.Elapsed.TotalMilliseconds} milliseconds.");
                }
            }
            finally
            {
                stream.Close();
            }
        }
        else
        {
            throw new Exception(dllPath + "  was used by other process and could not be prepared for serialization.");
        }
    }


}