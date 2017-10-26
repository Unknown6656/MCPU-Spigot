package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;

@MCPUOpcodeNumber(0x25)
public final class Pow extends BinaryOpcode
{
    public Pow()
    {
        super((x, y) -> (int)Math.pow(x, y));
    }
}
