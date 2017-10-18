package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUCallframe;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUProcessor;


public final class Starg extends MCPUOpcode
{
    @Override
    public int MinimumArgumentCount()
    {
        return 1;
    }

    @Override
    public int MinimumStackSize()
    {
        return 1;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int arg = arguments[0];
        int val = frame.Pop();
        
        frame.Arguments[arg] = val;
    }
}
