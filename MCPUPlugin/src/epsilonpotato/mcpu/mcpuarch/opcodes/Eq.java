package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryBooleanOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;

@MCPUOpcodeNumber(0x18)
public final class Eq extends BinaryBooleanOpcode
{
    public Eq()
    {
        super((x, y) -> x == y);
    }
}
