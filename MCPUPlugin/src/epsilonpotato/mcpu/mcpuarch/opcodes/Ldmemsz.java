package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUCallframe;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.MCPUProcessor;


@MCPUOpcodeNumber(0x09)
public final class Ldmemsz extends MCPUOpcode
{
    @Override
    public final int MinimumStackSize()
    {
        return 0;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int val = proc.getMemorySize();
        
        frame.Push(val);
    }
}
