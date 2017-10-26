package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;

@MCPUOpcodeNumber(0x24)
public final class Mod extends BinaryOpcode
{
    public Mod()
    {
        super((x, y) -> x % y);
    }
}
