using System.Collections.Generic;
using System.IO;
using Mono.Cecil;


public static class CodeWeaver
{
    public static WeaverAssemblyResolver Resolver = new WeaverAssemblyResolver();

    public static List<IWeaverProcessor> WeaverProcessors = new List<IWeaverProcessor>();

    public static void AddProcessor(IWeaverProcessor processor) => WeaverProcessors.Add(processor);

    public static bool WeaveDllStream(FileStream stream, bool havePdb)
    {
        var assemblyDefinition = AssemblyDefinition.ReadAssembly(stream, new ReaderParameters
        {
            ReadSymbols = havePdb,
            AssemblyResolver = Resolver,
            ThrowIfSymbolsAreNotMatching = false
        });

        var changed = false;
        foreach (var weaverProcessor in WeaverProcessors)
            changed |= weaverProcessor.Execute(assemblyDefinition);

        if (changed)
        {
            assemblyDefinition.Write(stream, new WriterParameters
            {
                WriteSymbols = havePdb
            });
        }

        return changed;
    }


}