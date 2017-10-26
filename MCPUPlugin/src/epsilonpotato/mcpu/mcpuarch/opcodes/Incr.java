package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

@MCPUOpcodeNumber(0x35)
public final class Incr extends UnaryOpcode
{
    public Incr()
    {
        super(x -> x - 1);
    }
}
