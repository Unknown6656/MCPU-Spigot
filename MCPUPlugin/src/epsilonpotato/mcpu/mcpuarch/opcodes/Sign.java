package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

@MCPUOpcodeNumber(0x2b)
public final class Sign extends UnaryOpcode
{
    public Sign()
    {
        super(x -> x == 0 ? 0 : x < 0 ? -1 : 1);
    }
}
