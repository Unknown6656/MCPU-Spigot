package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUCallframe;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.MCPUProcessor;


@MCPUOpcodeNumber(0x10)
public final class Regloc extends MCPUOpcode
{
    @Override
    public int MinimumStackSize()
    {
        return 0;
    }
    
    @Override
    public int MinimumArgumentCount()
    {
        return 1;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int[] tmp = new int[Math.max(arguments[0], 0)];
        
        for (int i = 0, l = Math.min(tmp.length, frame.Arguments.length); i < l; ++i)
            tmp[i] = frame.Arguments[i];
        
        frame.Arguments = tmp;
    }
}
