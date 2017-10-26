package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

@MCPUOpcodeNumber(0x26)
public final class Not extends UnaryOpcode
{
    public Not()
    {
        super(x -> ~x);
    }
}
