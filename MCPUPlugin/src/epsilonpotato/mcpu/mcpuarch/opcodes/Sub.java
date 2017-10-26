package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;

@MCPUOpcodeNumber(0x21)
public final class Sub extends BinaryOpcode
{
    public Sub()
    {
        super((x, y) -> x - y);
    }
}
