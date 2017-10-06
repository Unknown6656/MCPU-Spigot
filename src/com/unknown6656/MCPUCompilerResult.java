package com.unknown6656;

public final class MCPUCompilerResult
{
    public final MCPUInstruction[] Instructions;
    public final String ErrorMessage;
    public final boolean Success;
    
    
    public MCPUCompilerResult(MCPUInstruction[] instr, String err)
    {
        Success = instr != null;
        Instructions = instr;
        ErrorMessage = err;
    }
}
