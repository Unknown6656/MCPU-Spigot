package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUCallframe;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.MCPUProcessor;


@MCPUOpcodeNumber(0x40)
public final class Mov extends MCPUOpcode
{
    @Override
    public int MinimumStackSize()
    {
        return 2;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int dst = frame.Pop();
        int src = frame.Pop();
        int val = proc.getMemory(src);

        proc.setMemory(dst, val);
    }
}
