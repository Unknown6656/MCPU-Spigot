package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;

@MCPUOpcodeNumber(0x32)
public final class Shr extends BinaryOpcode
{
    public Shr()
    {
        super((x, y) -> x >>> y);
    }
}
