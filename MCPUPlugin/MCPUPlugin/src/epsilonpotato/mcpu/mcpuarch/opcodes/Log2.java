package epsilonpotato.mcpu.mcpuarch.opcodes;

import epsilonpotato.mcpu.mcpuarch.UnaryOpcode;

public final class Log2 extends UnaryOpcode
{
    /** fuck java */
    public static final double ε = 1e-11;
    
    
    public Log2()
    {   
        // fucking java strikes again ... not having a log_2-function defined and error correction with a definition of epsilon
        super(x -> (int)(Math.log(x) / (Math.log(2) + ε)));
    }
}
