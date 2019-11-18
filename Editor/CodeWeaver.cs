using System.Collections.Generic;
using System.IO;
using Mono.Cecil;


public static class CodeWeaver
{
    public static readonly ReaderParameters ReaderParams = new ReaderParameters
    {
        ReadSymbols = true,
        AssemblyResolver = new WeaverAssemblyResolver(),
    };

    public static List<IWeaverProcessor> WeaverProcessors = new List<IWeaverProcessor>();

    public static void AddProcessor(IWeaverProcessor processor) => WeaverProcessors.Add(processor);

    public static bool WeaveDllStream(Stream stream)
    {
        var assemblyDefinition = AssemblyDefinition.ReadAssembly(stream, ReaderParams);

        var changed = false;
        foreach (var weaverProcessor in WeaverProcessors)
            changed |= weaverProcessor.Execute(assemblyDefinition);

        if (changed)
        {
            assemblyDefinition.Write(stream, new WriterParameters
            {
                WriteSymbols = true
            });
        }

        return changed;
    }


}