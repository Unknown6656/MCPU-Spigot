package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUCallframe;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.MCPUProcessor;


@MCPUOpcodeNumber(0x43)
public final class Clr extends MCPUOpcode
{
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        proc.clearMemory();
    }
}
