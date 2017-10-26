package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

@MCPUOpcodeNumber(0x1f)
public final class Neg extends UnaryOpcode
{
    public Neg()
    {
        super(x -> -x);
    }
}
