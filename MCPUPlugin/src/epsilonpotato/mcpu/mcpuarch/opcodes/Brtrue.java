package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUCallframe;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.MCPUProcessor;


@MCPUOpcodeNumber(0x13)
public final class Brtrue extends MCPUOpcode
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
        if (frame.Pop() != 0)
            proc.InstructionPointer = arguments[0] - 1;
    }
}
