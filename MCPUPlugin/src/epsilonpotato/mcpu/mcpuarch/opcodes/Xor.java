package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;

@MCPUOpcodeNumber(0x29)
public final class Xor extends BinaryOpcode
{
    public Xor()
    {
        super((x, y) -> x ^ y);
    }
}
