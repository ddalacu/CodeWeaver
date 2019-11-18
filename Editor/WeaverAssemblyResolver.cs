using System.Collections.Generic;
using System.IO;
using Mono.Cecil;
using UnityEditor.Compilation;

public class WeaverAssemblyResolver : DefaultAssemblyResolver
{

    public WeaverAssemblyResolver()
    {
        var paths = new HashSet<string>();
        var alreadyChecked = new HashSet<string>();

        foreach (var assembly in CompilationPipeline.GetAssemblies())
        {
            paths.Add(Path.GetDirectoryName(assembly.outputPath));

            foreach (var assemblyCompiledAssemblyReference in assembly.allReferences)
            {
                if (alreadyChecked.Add(assemblyCompiledAssemblyReference) == false)
                    continue;

                paths.Add(Path.GetDirectoryName(assemblyCompiledAssemblyReference));
            }
        }

        foreach (var path in paths)
            AddSearchDirectory(path);
    }
}