package com.unknown6656.opcodes;

import com.unknown6656.MCPUCallframe;
import com.unknown6656.MCPUOpcode;
import com.unknown6656.MCPUProcessor;


public final class Stloc extends MCPUOpcode
{
    @Override
    public int MinimumArgumentCount()
    {
        return 1;
    }
    
    @Override
    public int MinimumStackSize()
    {
        return 1;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int local = frame.Pop();
        
        frame.Locals[arguments[0]] = local;
    }
}
