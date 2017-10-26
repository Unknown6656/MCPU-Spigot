package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;

@MCPUOpcodeNumber(0x28)
public final class Or extends BinaryOpcode
{
    public Or()
    {
        super((x, y) -> x | y);
    }
}
