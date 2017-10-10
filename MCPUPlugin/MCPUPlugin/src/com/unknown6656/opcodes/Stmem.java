package com.unknown6656.opcodes;

import com.unknown6656.MCPUCallframe;
import com.unknown6656.MCPUOpcode;
import com.unknown6656.MCPUProcessor;


public final class Stmem extends MCPUOpcode
{   
    // stmem addr val
    @Override
    public final int MinimumStackSize()
    {
        return 2;
    }
    
    @Override
    public final void Execute(int[] arguments, MCPUCallframe frame, MCPUProcessor proc)
    {
        int val = frame.Pop();
        int addr = frame.Pop();
        
        proc.Memory(addr, val);
    }
}
