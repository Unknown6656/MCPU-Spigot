package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;

@MCPUOpcodeNumber(0x23)
public final class Div extends BinaryOpcode
{
    public Div()
    {
        super((x, y) -> x / y);
    }
}
