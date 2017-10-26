package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;

@MCPUOpcodeNumber(0x22)
public final class Mul extends BinaryOpcode
{
    public Mul()
    {
        super((x, y) -> x * y);
    }
}
