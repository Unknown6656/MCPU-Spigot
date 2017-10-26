package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

@MCPUOpcodeNumber(0x2c)
public final class Exp extends UnaryOpcode
{
    public Exp()
    {
        super(x -> (int)Math.pow(Math.E, x));
    }
}
