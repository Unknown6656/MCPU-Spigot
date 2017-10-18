package epsilonpotato.mcpu.mcpuarch;

public interface MCPUProcessorEvent<T>
{
    public void Raise(MCPUProcessor proc, T data);
}
