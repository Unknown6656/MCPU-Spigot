package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryBooleanOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;

@MCPUOpcodeNumber(0x1d)
public final class Gt extends BinaryBooleanOpcode
{
    public Gt()
    {
        super((x, y) -> x > y);
    }
}
