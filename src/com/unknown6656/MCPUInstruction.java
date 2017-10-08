package com.unknown6656;


public final class MCPUInstruction
{
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
            proc.Stop();
    }
    
    public MCPUOpcode GetOPCode()
    {
        return opcode;
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
