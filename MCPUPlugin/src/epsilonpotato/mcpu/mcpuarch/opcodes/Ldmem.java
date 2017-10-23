package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUCallframe;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUProcessor;


public final class Ldmem extends MCPUOpcode
{
    @Override
    public final int MinimumStackSize()
    {
        return 1;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int addr = frame.Pop();
        int val = proc.getMemory(addr);
        
        frame.Push(val);
    }
}
