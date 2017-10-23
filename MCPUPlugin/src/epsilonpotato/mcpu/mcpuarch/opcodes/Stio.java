package epsilonpotato.mcpu.mcpuarch.opcodes;

import static java.lang.Math.*;

import epsilonpotato.mcpu.mcpuarch.MCPUCallframe;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUProcessor;


public final class Stio extends MCPUOpcode
{
    // stio port val
    @Override
    public int MinimumStackSize()
    {
        return 2;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int value = frame.Pop();
        int port = frame.Pop();
        
        proc.setIOValue(port, (byte)max(0, min(value, 15)));
    }
}
