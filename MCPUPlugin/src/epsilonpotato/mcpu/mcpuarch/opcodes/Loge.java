package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;
import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

@MCPUOpcodeNumber(0x2e)
public final class Loge extends UnaryOpcode
{
    public Loge()
    {
        super(x -> (int)Math.log(x));
    }
}
