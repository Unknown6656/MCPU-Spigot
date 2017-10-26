package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;

@MCPUOpcodeNumber(0x20)
public final class Add extends BinaryOpcode
{
    public Add()
    {
        super((x, y) -> x + y);
    }
}
