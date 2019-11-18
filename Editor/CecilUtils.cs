using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

public static class CecilUtils
{
    public static bool AreEqual(MethodDefinition a, MethodDefinition b)
    {
        var parameters = a.Parameters;
        var paramsCount = parameters.Count;

        var methodDefinitionParameters = b.Parameters;
        if (paramsCount != methodDefinitionParameters.Count)
            return false;

        for (var index = 0; index < paramsCount; index++)
            if (AreEqual(methodDefinitionParameters[index].ParameterType, parameters[index].ParameterType) == false)
                return false;

        if (AreEqual(a.ReturnType, b.ReturnType) == false)
            return false;

        return true;
    }

    public static bool AreEqual(TypeReference a, TypeReference b)
    {
        if (string.Equals(a.FullName, b.FullName, StringComparison.Ordinal) == false)
            return false;
        if (string.Equals(a.Namespace, b.Namespace, StringComparison.Ordinal) == false)
            return false;

        return true;
    }

    public static void EmitLoadArgForParam(ILProcessor processor, ParameterDefinition parameterDefinition)
    {
        switch (parameterDefinition.Index)
        {
            case 0:
                processor.Emit(OpCodes.Ldarg_0);
                break;
            case 1:
                processor.Emit(OpCodes.Ldarg_1);
                break;
            case 2:
                processor.Emit(OpCodes.Ldarg_2);
                break;
            case 3:
                processor.Emit(OpCodes.Ldarg_3);
                break;
            default:
                processor.Emit(OpCodes.Ldarg_S, parameterDefinition);
                break;
        }
    }

    public static bool ModuleHaveReferencesTo(AssemblyDefinition assemblyDefinition, HashSet<string> assemblyFullNames)
    {
        var fullAssemblyNames = new HashSet<string>(assemblyFullNames);

        foreach (var assemblyDefinitionModule in assemblyDefinition.Modules)
        {
            foreach (var assemblyNameReference in assemblyDefinitionModule.AssemblyReferences)
                if (fullAssemblyNames.Contains(assemblyNameReference.FullName))
                    return true;
        }

        return false;
    }

    public static bool AssemblyHaveReferencesTo(AssemblyDefinition assemblyDefinition, HashSet<string> assemblyFullNames)
    {
        foreach (var assemblyDefinitionModule in assemblyDefinition.Modules)
        {
            foreach (var assemblyNameReference in assemblyDefinitionModule.AssemblyReferences)
                if (assemblyFullNames.Contains(assemblyNameReference.FullName))
                    return true;
        }

        return false;
    }

    public static IEnumerable<(ModuleDefinition Module, TypeDefinition Type, MethodDefinition Method)> IterateMethods(AssemblyDefinition assemblyDefinition)
    {
        foreach (var assemblyDefinitionModule in assemblyDefinition.Modules)
        {
            foreach (var typeDefinition in assemblyDefinitionModule.Types)
            {
                foreach (var methodDefinition in typeDefinition.Methods)
                {
                    yield return (assemblyDefinitionModule, typeDefinition, methodDefinition);
                }
            }
        }
    }

}
