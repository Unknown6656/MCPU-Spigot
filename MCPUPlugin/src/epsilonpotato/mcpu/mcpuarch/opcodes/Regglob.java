package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUCallframe;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.MCPUProcessor;


@MCPUOpcodeNumber(0x11)
public final class Regglob extends MCPUOpcode
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
        int oldsz = proc.globalscount;
        int newsz = proc.globalscount = arguments[0];
        int memlen = proc.getMemorySize();
        
        if (oldsz < newsz)
            for (int i = oldsz; i < newsz; ++i)
                proc.setGlobal(i, 0);
        else
            for (int i = newsz; i < oldsz; ++i)
                proc.setMemory(memlen - i, 0);
    }
}
