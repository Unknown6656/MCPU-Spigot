package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

@MCPUOpcodeNumber(0x30)
public final class Log10 extends UnaryOpcode
{
    public Log10()
    {
        super(x -> (int)Math.log10(x));
    }
}
