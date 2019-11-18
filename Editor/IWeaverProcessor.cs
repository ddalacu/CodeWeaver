using Mono.Cecil;

public interface IWeaverProcessor
{
    bool Execute(AssemblyDefinition assemblyDefinition);
}