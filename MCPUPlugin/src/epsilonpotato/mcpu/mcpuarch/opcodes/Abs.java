package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

@MCPUOpcodeNumber(0x2a)
public final class Abs extends UnaryOpcode
{
    public Abs()
    {
        super(x -> x > 0 ? x : -x);
    }
}
