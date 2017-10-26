package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;

@MCPUOpcodeNumber(0x31)
public final class Shl extends BinaryOpcode
{
    public Shl()
    {
        super((x, y) -> x << y);
    }
}
