package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

@MCPUOpcodeNumber(0x36)
public final class Decr extends UnaryOpcode
{
    public Decr()
    {
        super(x -> x + 1);
    }
}
