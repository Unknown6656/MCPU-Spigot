package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryBooleanOpcode;
import epsilonpotato.mcpu.mcpuarch.MCPUOpcodeNumber;

@MCPUOpcodeNumber(0x1b)
public final class Lt extends BinaryBooleanOpcode
{
    public Lt()
    {
        super((x, y) -> x < y);
    }
}
