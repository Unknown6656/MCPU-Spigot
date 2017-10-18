package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.BinaryOpcode;

public final class Log extends BinaryOpcode
{
    public static final double e = 1e-11;
    
    public Log()
    {
        super((x, y) -> (int)(Math.log(x) / (Math.log(y) + e)));
    }
}
