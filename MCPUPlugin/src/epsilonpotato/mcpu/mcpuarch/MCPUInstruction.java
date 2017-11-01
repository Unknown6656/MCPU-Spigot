package epsilonpotato.mcpu.mcpuarch;

import java.io.Serializable;

public final class MCPUInstruction implements Serializable
{
    private static final long serialVersionUID = 7079972155059307515L;
    private int[] arguments;
    private MCPUOpcode opcode;
    
    
    public MCPUInstruction(MCPUOpcode opc, int... args)
    {
        arguments = args;
        opcode = opc;
    }
    
    public void Execute(MCPUProcessor proc, MCPUCallframe frame)
    {
        if (arguments.length >= opcode.MinimumArgumentCount())
            opcode.Execute(arguments, frame, proc);
        else
            proc.stop();
    }
    
    public MCPUOpcode getOPCode()
    {
        return opcode;
    }
    
    public int[] getArguments()
    {
        return arguments;
    }
    
    @Override
    public String toString()
    {
        StringBuilder sb = new StringBuilder();
        boolean first = true;
        
        sb.append(opcode.getClass().getName()).append('(');
        
        for (int arg : arguments)
        {
            if (!first)
                sb.append(", ");

            sb.append(String.format("0x%08x", arg));
        }
            
        return sb.append(')').toString();
    }
}
